using Magazzino.ClientHttp.Abstraction;
using Ordini.Business.Abstractions;
using Ordini.Business.Kafka;
using Ordini.Repository.Abstractions;
using Ordini.Repository.Models;
using Ordini.Shared;
using Ordini.Shared.Events;
using Utility.Kafka.Abstractions.Clients;

namespace Ordini.Business;

// Costruttore primario stile C# 12: via le variabili private
public class OrdiniService(
    IOrdiniRepository repository,
    IMagazzinoClient magazzinoClient,
    IProducerClient<string, string> producerClient) : IOrdiniService
{
    public async Task<IEnumerable<OrdineDto>> GetAllOrdiniAsync()
    {
        var ordini = await repository.GetAllOrdiniAsync();

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
        var ordine = await repository.GetOrdineByIdAsync(id);
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
            var prodottoMagazzino = await magazzinoClient.GetProdottoByIdAsync(riga.ProdottoId);

            if (prodottoMagazzino == null)
            {
                throw new Exception($"Impossibile creare l'ordine: il prodotto con ID {riga.ProdottoId} non esiste in magazzino.");
            }

            // Sicurezza: forziamo il prezzo unitario a quello UFFICIALE del magazzino
            riga.PrezzoUnitario = prodottoMagazzino.Prezzo;

            // Calcoliamo il totale riga per riga
            totaleCalcolato += (riga.Quantita * riga.PrezzoUnitario);
        }

        // 2. Mapping: DTO -> Entity
        var nuovoOrdine = new Ordine // Assicurati che 'Ordine' sia la tua entità DB
        {
            Cliente = nuovoOrdineDto.Cliente,
            DataOrdine = DateTime.UtcNow,
            Stato = StatiOrdine.InElaborazione,
            Totale = totaleCalcolato,
            Prodotti = nuovoOrdineDto.Prodotti.Select(p => new ProdottoOrdinato // Assicurati che 'ProdottoOrdinato' sia l'entità
            {
                ProdottoId = p.ProdottoId,
                Quantita = p.Quantita,
                PrezzoUnitario = p.PrezzoUnitario
            }).ToList()
        };

        await repository.CreaOrdineAsync( nuovoOrdine );
        await repository.SaveChangesAsync();

        // 4. Aggiorniamo il DTO
        nuovoOrdineDto.Id = nuovoOrdine.Id; // L'ID è generato da EF dopo il SaveChanges
        nuovoOrdineDto.DataOrdine = nuovoOrdine.DataOrdine;
        nuovoOrdineDto.Stato = nuovoOrdine.Stato;
        nuovoOrdineDto.Totale = nuovoOrdine.Totale;

        // --- NUOVO: KAFKA EVENT (Tramite UtilityKafka!) ---
        var evento = new OrdineCreatoEvent
        {
            OrdineId = nuovoOrdine.Id,
            Prodotti = nuovoOrdineDto.Prodotti
        };
        string messaggioJson = System.Text.Json.JsonSerializer.Serialize(evento);

        // Usiamo l'interfaccia generica IProducerClient
        await producerClient.ProduceAsync(KafkaTopics.OrdineCreato, nuovoOrdine.Id.ToString(), messaggioJson);

        return nuovoOrdineDto;
    }

    public async Task<bool> ConfermaOrdineSePagatoAsync(int ordineId, decimal importoPagato, CancellationToken cancellationToken = default)
    {
        var ordine = await repository.GetOrdineByIdAsync(ordineId, cancellationToken);

        if (ordine is null)
            return false;   // pagamento riferito a un ordine inesistente

        // Idempotenza: Kafka può riconsegnare lo stesso messaggio (at-least-once).
        if (ordine.Stato == StatiOrdine.Confermato)
            return true;

        // Confermiamo solo ordini ancora in lavorazione (non annullati/rifiutati).
        if (ordine.Stato != StatiOrdine.InElaborazione)
            return false;

        // ---- IL CONTROLLO CHIESTO: l'importo deve coincidere col totale ----
        if (importoPagato != ordine.Totale)
            return false;

        await repository.AggiornaStatoOrdineAsync(ordineId, StatiOrdine.Confermato, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}