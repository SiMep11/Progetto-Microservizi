using Magazzino.ClientHttp.Abstraction;
using Ordini.Business.Abstractions;
using Ordini.Business.Kafka;
using Ordini.Repository.Abstractions;
using Ordini.Repository.Models;
using Ordini.Shared;
using Ordini.Shared.Events;
using Ordini.Business.Factory;

namespace Ordini.Business;

// Costruttore primario stile C# 12
public class OrdiniService(
    IOrdiniRepository repository,
    IMagazzinoClient magazzinoClient,
    IOrdiniObserver observer) : IOrdiniService
{
    public async Task<IEnumerable<OrdineDto>> GetAllOrdiniAsync()
    {
        var ordini = await repository.GetAllOrdiniAsync();

        return ordini.Select(MapOrdine);
    }

    public async Task<OrdineDto?> GetOrdineByIdAsync(int id)
    {
        var ordine = await repository.GetOrdineByIdAsync(id);

        return ordine == null ? null : MapOrdine(ordine);
    }

    public async Task<OrdineDto> CreaOrdineAsync(OrdineInsertDto nuovoOrdineDto)
    {
        if (nuovoOrdineDto.Prodotti is null || nuovoOrdineDto.Prodotti.Count == 0)
            throw new ArgumentException("L'ordine deve contenere almeno un prodotto", nameof(nuovoOrdineDto.Prodotti));

        decimal totaleCalcolato = 0;
        var righe = new List<ProdottoOrdinato>();

        // 1. CHIAMATA AL MAGAZZINO: Controllo i prezzi reali!
        foreach (var riga in nuovoOrdineDto.Prodotti)
        {
            if (riga.Quantita <= 0)
                throw new ArgumentException($"La quantità del prodotto {riga.ProdottoId} deve essere positiva", nameof(nuovoOrdineDto.Prodotti));

            var prodottoMagazzino = await magazzinoClient.GetProdottoByIdAsync(riga.ProdottoId);    // qui inizia la comunicazione sincrona

            if (prodottoMagazzino == null)
            {
                throw new ArgumentException($"Impossibile creare l'ordine: il prodotto con ID {riga.ProdottoId} non esiste in magazzino.", nameof(nuovoOrdineDto.Prodotti));
            }

            // Sicurezza: il prezzo unitario è quello UFFICIALE del magazzino.
            // Per questo non compare nel DTO in ingresso: il client non può deciderlo.
            righe.Add(new ProdottoOrdinato
            {
                ProdottoId = riga.ProdottoId,
                Quantita = riga.Quantita,
                PrezzoUnitario = prodottoMagazzino.Prezzo
            });

            // Calcolo il totale riga per riga
            totaleCalcolato += riga.Quantita * prodottoMagazzino.Prezzo;
        }

        // 2. Mapping: DTO -> Entity
        var nuovoOrdine = new Ordine
        {
            Cliente = nuovoOrdineDto.Cliente,
            DataOrdine = DateTime.UtcNow,
            Stato = StatiOrdine.InElaborazione,
            Totale = totaleCalcolato,
            Prodotti = righe
        };

        await repository.CreaOrdineAsync(nuovoOrdine);
        await repository.SaveChangesAsync();

        // NB: qui NON avviso il magazzino. L'ordine è solo "In elaborazione":
        // la giacenza verrà scaricata solo dopo l'incasso, in ConfermaOrdineSePagatoAsync.

        return MapOrdine(nuovoOrdine);
    }

    public async Task<bool> ConfermaOrdineSePagatoAsync(int ordineId, decimal importoPagato, CancellationToken cancellationToken = default)
    {
        var ordine = await repository.GetOrdineByIdAsync(ordineId, cancellationToken);

        if (ordine is null)
            return false;   // pagamento riferito a un ordine inesistente

        // Idempotenza: Kafka può riconsegnare lo stesso messaggio (at-least-once).
        if (ordine.Stato == StatiOrdine.Confermato)
            return true;

        // Confermo solo ordini ancora in lavorazione (non annullati/rifiutati).
        if (ordine.Stato != StatiOrdine.InElaborazione)
            return false;

        // l'importo deve coincidere col totale ----
        if (importoPagato != ordine.Totale)
            return false;

        long idOutbox = 0;

        // Stato dell'ordine e messaggio Kafka nella STESSA transazione:
        // o si salvano entrambi, o non si salva nulla.
        await repository.BeginTransactionAsync(async (CancellationToken ct) =>
        {
            await repository.AggiornaStatoOrdineAsync(ordineId, StatiOrdine.Confermato, ct);

            var evento = new OrdineConfermatoEvent
            {
                OrdineId = ordine.Id,
                Prodotti = ordine.Prodotti.Select(MapRiga).ToList()
            };

            var outbox = TransactionalOutboxFactory.Create(evento);
            await repository.InsertTransactionalOutboxAsync(outbox, ct);

            await repository.SaveChangesAsync(ct);

            idOutbox = outbox.Id;
        }, cancellationToken);

        // Svegliamo il producer: pubblicherà subito invece di attendere il giro successivo
        if (idOutbox > 0)
            observer.NuovoMessaggioOutbox.OnNext(idOutbox);

        return true;
    }

    #region Helper privati

    private static OrdineDto MapOrdine(Ordine o) => new OrdineDto
    {
        Id = o.Id,
        DataOrdine = o.DataOrdine,
        Cliente = o.Cliente,
        Totale = o.Totale,
        Stato = o.Stato,
        Prodotti = o.Prodotti.Select(MapRiga).ToList()
    };

    private static ProdottoOrdinatoDto MapRiga(ProdottoOrdinato p) => new ProdottoOrdinatoDto
    {
        ProdottoId = p.ProdottoId,
        Quantita = p.Quantita,
        PrezzoUnitario = p.PrezzoUnitario
    };

    #endregion
}