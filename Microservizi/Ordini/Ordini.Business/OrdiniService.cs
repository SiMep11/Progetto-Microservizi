using Ordini.Business.Abstractions;
using Ordini.Repository.Abstractions;
using Ordini.Repository.Models;
using Ordini.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordini.Business
{
    public class OrdiniService : IOrdiniService
    {
        private readonly IOrdiniRepository _repository;

        // Dependency Injection del Repository
        public OrdiniService(IOrdiniRepository repository)
        {
            _repository = repository;
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
            // Mapping: DTO (Internet) -> Entity (DB)
            var nuovoOrdine = new Ordine
            {
                Cliente = nuovoOrdineDto.Cliente,
                DataOrdine = System.DateTime.UtcNow,
                Stato = "In elaborazione",
                Totale = nuovoOrdineDto.Totale,
                Prodotti = nuovoOrdineDto.Prodotti.Select(p => new ProdottoOrdinato
                {
                    ProdottoId = p.ProdottoId,
                    Quantita = p.Quantita,
                    PrezzoUnitario = p.PrezzoUnitario
                }).ToList()
            };

            // Salviamo nel DB
            var ordineSalvato = await _repository.CreaOrdineAsync(nuovoOrdine);

            // Aggiorniamo l'ID e i dati del DTO con quelli generati dal database
            nuovoOrdineDto.Id = ordineSalvato.Id;
            nuovoOrdineDto.DataOrdine = ordineSalvato.DataOrdine;
            nuovoOrdineDto.Stato = ordineSalvato.Stato;

            return nuovoOrdineDto;
        }
    }
}
