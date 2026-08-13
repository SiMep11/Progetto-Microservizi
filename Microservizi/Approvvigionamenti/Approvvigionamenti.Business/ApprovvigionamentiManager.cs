using Approvvigionamenti.Business.Abstraction;
using Approvvigionamenti.Business.Kafka;
using Approvvigionamenti.Repository.Abstraction;
using Approvvigionamenti.Repository.Models;
using Approvvigionamenti.Shared;
using Approvvigionamenti.Shared.Events;
using Magazzino.ClientHttp.Abstraction;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Utility.Kafka.Abstractions.Clients;

namespace Approvvigionamenti.Business;

// Costruttore primario stile C# 12, come negli altri microservizi
public class ApprovvigionamentiManager(
    IApprovvigionamentiRepository repository,
    IMagazzinoClient magazzinoClient,
    IProducerClient<string, string> producerClient,
    ILogger<ApprovvigionamentiManager> logger) : IApprovvigionamentiManager
{
    #region Fornitori

    public async Task<IEnumerable<FornitoreDto>> GetAllFornitoriAsync(CancellationToken cancellationToken = default)
    {
        var fornitori = await repository.GetAllFornitoriAsync(cancellationToken);
        return fornitori.Select(MapFornitore);
    }

    public async Task<FornitoreDto?> GetFornitoreByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var fornitore = await repository.GetFornitoreByIdAsync(id, cancellationToken);
        return fornitore is null ? null : MapFornitore(fornitore);
    }

    public async Task<FornitoreDto> CreaFornitoreAsync(FornitoreDto fornitoreDto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fornitoreDto.RagioneSociale))
            throw new ArgumentException("La ragione sociale è obbligatoria", nameof(fornitoreDto.RagioneSociale));

        var fornitore = new Fornitore
        {
            RagioneSociale = fornitoreDto.RagioneSociale,
            Email = fornitoreDto.Email
        };

        await repository.CreaFornitoreAsync(fornitore, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        fornitoreDto.Id = fornitore.Id;
        return fornitoreDto;
    }

    public async Task<ProdottoFornitoDto> AggiungiProdottoFornitoAsync(ProdottoFornitoDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.QuantitaRiordino <= 0)
            throw new ArgumentException("La quantità di riordino deve essere positiva", nameof(dto.QuantitaRiordino));

        var fornitore = await repository.GetFornitoreByIdAsync(dto.FornitoreId, cancellationToken);
        if (fornitore is null)
            throw new ArgumentException($"Il fornitore {dto.FornitoreId} non esiste", nameof(dto.FornitoreId));

        // CHIAMATA SINCRONA AL MAGAZZINO: non ha senso associare un fornitore a un prodotto che non esiste a catalogo.
        var prodotto = await magazzinoClient.GetProdottoByIdAsync(dto.ProdottoId, cancellationToken);
        if (prodotto is null)
            throw new ArgumentException($"Il prodotto {dto.ProdottoId} non esiste in magazzino", nameof(dto.ProdottoId));

        // Ipotesi di progetto: un solo fornitore per tipo di prodotto.
        // Controlliamo qui per dare un messaggio leggibile; l'indice unico sul DB resta come rete di sicurezza.
        var esistente = await repository.GetProdottoFornitoAsync(dto.ProdottoId, cancellationToken);
        if (esistente is not null)
            throw new ArgumentException($"Il prodotto {dto.ProdottoId} è già assegnato al fornitore {esistente.FornitoreId}", nameof(dto.ProdottoId));

        var prodottoFornito = new ProdottoFornito
        {
            FornitoreId = dto.FornitoreId,
            ProdottoId = dto.ProdottoId,
            PrezzoAcquisto = dto.PrezzoAcquisto,
            QuantitaRiordino = dto.QuantitaRiordino
        };

        await repository.AggiungiProdottoFornitoAsync(prodottoFornito, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        dto.Id = prodottoFornito.Id;
        return dto;
    }

    #endregion

    #region OrdiniFornitore

    public async Task<IEnumerable<OrdineFornitoreDto>> GetAllOrdiniFornitoreAsync(CancellationToken cancellationToken = default)
    {
        var ordini = await repository.GetAllOrdiniFornitoreAsync(cancellationToken);
        return ordini.Select(MapOrdine);
    }

    public async Task<OrdineFornitoreDto?> GetOrdineFornitoreByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var ordine = await repository.GetOrdineFornitoreByIdAsync(id, cancellationToken);
        return ordine is null ? null : MapOrdine(ordine);
    }

    public async Task<OrdineFornitoreDto> CreaOrdineFornitoreAsync(int prodottoId, int quantita, CancellationToken cancellationToken = default)
    {
        if (quantita <= 0)
            throw new ArgumentException("La quantità deve essere positiva", nameof(quantita));

        // CHIAMATA SINCRONA AL MAGAZZINO: il prodotto deve esistere a catalogo
        var prodotto = await magazzinoClient.GetProdottoByIdAsync(prodottoId, cancellationToken);
        if (prodotto is null)
            throw new ArgumentException($"Il prodotto {prodottoId} non esiste in magazzino", nameof(prodottoId));

        var prodottoFornito = await repository.GetProdottoFornitoAsync(prodottoId, cancellationToken);
        if (prodottoFornito is null)
            throw new ArgumentException($"Nessun fornitore configurato per il prodotto {prodottoId}", nameof(prodottoId));

        var ordine = await CreaOrdineInternoAsync(prodottoFornito, quantita, cancellationToken);
        return MapOrdine(ordine);
    }

    public async Task<bool> ConfermaRicezioneAsync(int ordineFornitoreId, CancellationToken cancellationToken = default)
    {
        var ordine = await repository.GetOrdineFornitoreByIdAsync(ordineFornitoreId, cancellationToken);

        if (ordine is null)
            return false;

        // Idempotenza: confermare due volte non deve raddoppiare la giacenza
        if (ordine.Stato == StatiOrdineFornitore.Ricevuto)
            return true;

        ordine.Stato = StatiOrdineFornitore.Ricevuto;
        ordine.DataRicezione = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);

        // --- KAFKA EVENT (Tramite UtilityKafka!) ---
        // Solo ORA la merce è fisicamente arrivata: il magazzino può ricaricare.
        var evento = new MerceRicevutaEvent
        {
            OrdineFornitoreId = ordine.Id,
            ProdottoId = ordine.ProdottoId,
            Quantita = ordine.Quantita
        };
        string messaggioJson = JsonSerializer.Serialize(evento);

        await producerClient.ProduceAsync(KafkaTopics.MerceRicevuta, ordine.Id.ToString(), messaggioJson, cancellationToken);

        logger.LogInformation("Ordine fornitore {OrdineId} ricevuto: {Quantita} pezzi del prodotto {ProdottoId}",
            ordine.Id, ordine.Quantita, ordine.ProdottoId);

        return true;
    }

    #endregion

    public async Task RiordinaSeNecessarioAsync(int prodottoId, int quantitaAttuale, CancellationToken cancellationToken = default)
    {
        // Ogni vendita sotto soglia genera un evento: senza questo controllo ci ritroveremmo decine di ordini di acquisto per lo stesso prodotto.
        if (await repository.EsisteOrdineApertoAsync(prodottoId, cancellationToken))
        {
            logger.LogInformation("Prodotto {ProdottoId} sotto soglia ma c'è già un ordine in attesa di consegna: non riordino",
                prodottoId);
            return;
        }

        var prodottoFornito = await repository.GetProdottoFornitoAsync(prodottoId, cancellationToken);
        if (prodottoFornito is null)
        {
            logger.LogWarning("Prodotto {ProdottoId} sotto soglia (giacenza {Quantita}) ma non ha un fornitore configurato: riordino impossibile",
                prodottoId, quantitaAttuale);
            return;
        }

        var ordine = await CreaOrdineInternoAsync(prodottoFornito, prodottoFornito.QuantitaRiordino, cancellationToken);

        logger.LogInformation("Riordino automatico: ordine {OrdineId} di {Quantita} pezzi del prodotto {ProdottoId} al fornitore {FornitoreId}",
            ordine.Id, ordine.Quantita, prodottoId, prodottoFornito.FornitoreId);
    }

    #region Helper privati

    private async Task<OrdineFornitore> CreaOrdineInternoAsync(ProdottoFornito prodottoFornito, int quantita, CancellationToken cancellationToken)
    {
        var ordine = new OrdineFornitore
        {
            FornitoreId = prodottoFornito.FornitoreId,
            ProdottoId = prodottoFornito.ProdottoId,
            Quantita = quantita,
            PrezzoUnitario = prodottoFornito.PrezzoAcquisto,
            DataOrdine = DateTime.UtcNow,
            Stato = StatiOrdineFornitore.Richiesto
        };

        await repository.CreaOrdineFornitoreAsync(ordine, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return ordine;
    }

    private static FornitoreDto MapFornitore(Fornitore f) => new FornitoreDto
    {
        Id = f.Id,
        RagioneSociale = f.RagioneSociale,
        Email = f.Email,
        ProdottiForniti = f.ProdottiForniti.Select(p => new ProdottoFornitoDto
        {
            Id = p.Id,
            FornitoreId = p.FornitoreId,
            ProdottoId = p.ProdottoId,
            PrezzoAcquisto = p.PrezzoAcquisto,
            QuantitaRiordino = p.QuantitaRiordino
        }).ToList()
    };

    private static OrdineFornitoreDto MapOrdine(OrdineFornitore o) => new OrdineFornitoreDto
    {
        Id = o.Id,
        FornitoreId = o.FornitoreId,
        ProdottoId = o.ProdottoId,
        Quantita = o.Quantita,
        PrezzoUnitario = o.PrezzoUnitario,
        DataOrdine = o.DataOrdine,
        DataRicezione = o.DataRicezione,
        Stato = o.Stato
    };

    #endregion
}