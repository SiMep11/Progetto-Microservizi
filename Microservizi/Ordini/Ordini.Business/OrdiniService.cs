using Ordini.Business.Abstractions;
using Ordini.Repository.Abstractions;
using Ordini.Repository.Models;
using Ordini.Shared;
using Ordini.Shared.Events;
using Ordini.ClientHttp.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordini.Business
{
    public class OrdiniService : IOrdiniService
    {
        private readonly IOrdiniRepository _repository;
        private readonly IMagazzinoClient _magazzinoClient; // <-- Il nostro nuovo "telefono"
        private readonly IEventProducer _eventProducer; // NUOVO: Aggiunto l'altoparlante

        // Dependency Injection del Repository
        public OrdiniService(IOrdiniRepository repository, IMagazzinoClient magazzinoClient, IEventProducer eventProducer)
        {
            _repository = repository;
            _magazzinoClient = magazzinoClient;
            _eventProducer = eventProducer;
        }

        public async Task<IEnumerable<OrdineDto>> GetAllOrdiniAsync()
        {
            var ordini = await _repository.GetAllOrdiniAsync();

            // Mapping: Entity (DB) -> DTO (Internet)
            return ordini.Select(o => new OrdineDto
            {
                Id = o.Id,
                DataOrdine = o.DataOrdine,
                Cliente = o.Cliente,
                Totale = o.Totale,
                Stato = o.Stato,
                Prodotti = o.Prodotti.Select(p => new ProdottoOrdinatoDto
                {
                    ProdottoId = p.ProdottoId,
                    Quantita = p.Quantita,
                    PrezzoUnitario = p.PrezzoUnitario
                }).ToList()
            });
        }

        public async Task<OrdineDto?> GetOrdineByIdAsync(int id)
        {
            var ordine = await _repository.GetOrdineByIdAsync(id);
            if (ordine == null) return null;

            return new OrdineDto
            {
                Id = ordine.Id,
                DataOrdine = ordine.DataOrdine,
                Cliente = ordine.Cliente,
                Totale = ordine.Totale,
                Stato = ordine.Stato,
                Prodotti = ordine.Prodotti.Select(p => new ProdottoOrdinatoDto
                {
                    ProdottoId = p.ProdottoId,
                    Quantita = p.Quantita,
                    PrezzoUnitario = p.PrezzoUnitario
                }).ToList()
            };
        }

        public async Task<OrdineDto> CreaOrdineAsync(OrdineDto nuovoOrdineDto)
        {
            decimal totaleCalcolato = 0;

            // 1. CHIAMATA AL MAGAZZINO: Controlliamo i prezzi reali!
            foreach (var riga in nuovoOrdineDto.Prodotti)
            {
                // Facciamo la vera chiamata HTTP al microservizio Magazzino
                var prodottoMagazzino = await _magazzinoClient.GetProdottoByIdAsync(riga.ProdottoId);

                if (prodottoMagazzino == null)
                {
                    // Se il prodotto non esiste, blocchiamo l'ordine sollevando un'eccezione
                    throw new Exception($"Impossibile creare l'ordine: il prodotto con ID {riga.ProdottoId} non esiste in magazzino.");
                }

                // Sicurezza: forziamo il prezzo unitario a quello UFFICIALE del magazzino
                riga.PrezzoUnitario = prodottoMagazzino.Prezzo;

                // Calcoliamo il totale riga per riga
                totaleCalcolato += (riga.Quantita * riga.PrezzoUnitario);
            }

            // 2. Mapping: DTO (Internet) -> Entity (DB)
            var nuovoOrdine = new Ordine
            {
                Cliente = nuovoOrdineDto.Cliente,
                DataOrdine = DateTime.UtcNow,
                Stato = "In elaborazione",
                Totale = totaleCalcolato, // <-- Usiamo il totale sicuro e verificato!
                Prodotti = nuovoOrdineDto.Prodotti.Select(p => new ProdottoOrdinato
                {
                    ProdottoId = p.ProdottoId,
                    Quantita = p.Quantita,
                    PrezzoUnitario = p.PrezzoUnitario // Che ora è quello sicuro
                }).ToList()
            };

            // 3. Salviamo nel DB degli Ordini
            var ordineSalvato = await _repository.CreaOrdineAsync(nuovoOrdine);

            // 4. Aggiorniamo il DTO di ritorno per il cliente
            nuovoOrdineDto.Id = ordineSalvato.Id;
            nuovoOrdineDto.DataOrdine = ordineSalvato.DataOrdine;
            nuovoOrdineDto.Stato = ordineSalvato.Stato;
            nuovoOrdineDto.Totale = ordineSalvato.Totale; // Il cliente vede il vero totale pagato

            // --- NUOVO: KAFKA EVENT ---
            // Creiamo il pacchetto da inviare al Magazzino (e a chiunque altro ascolti)
            var evento = new OrdineCreatoEvent
            {
                OrdineId = ordineSalvato.Id,
                Prodotti = nuovoOrdineDto.Prodotti // Passiamo la lista dei prodotti venduti
            };

            // Urliamo nel megafono! (Non blocchiamo l'esecuzione se fallisce, l'ordine è già salvo)
            await _eventProducer.ProduceOrdineCreatoAsync(evento);
            // --------------------------

            return nuovoOrdineDto;
        }
    }
}
