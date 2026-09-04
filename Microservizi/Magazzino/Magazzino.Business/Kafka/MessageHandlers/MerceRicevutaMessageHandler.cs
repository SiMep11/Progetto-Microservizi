using Magazzino.Business.Abstraction;
using Magazzino.Shared.Events;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Utility.Kafka.Abstractions.MessageHandlers;

namespace Magazzino.Business.Kafka.MessageHandlers;

public class MerceRicevutaMessageHandler(
    ILogger<MerceRicevutaMessageHandler> logger,
    IProdottiManager manager) : IMessageHandler<string, string>
{
    public async Task OnMessageReceivedAsync(string key, string message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(message))         // controllo se il messaggio è vuoto
        {
            logger.LogError("Il messaggio è null o white space e quindi sarà scartato");
            return;
        }

        MerceRicevutaEvent? evento;
        try
        {
            evento = JsonSerializer.Deserialize<MerceRicevutaEvent>(message);   // controllo che il JSON abbia il formato corretto
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Messaggio non deserializzabile in {tipo}, sarà scartato: {msg}",
                nameof(MerceRicevutaEvent), message);
            return;
        }

        if (evento is null)     // controllo che il tipo sia giusto
        {
            logger.LogError("Il messaggio non è del tipo {tipo}, sarà scartato", nameof(MerceRicevutaEvent));
            return;
        }

        logger.LogInformation("Merce dell'ordine fornitore {OrdineId} arrivata: carico {Quantita} pezzi del prodotto {ProdottoId}",
            evento.OrdineFornitoreId, evento.Quantita, evento.ProdottoId);

        await manager.CaricaGiacenzaAsync(evento.ProdottoId, evento.Quantita, cancellationToken);   // chiamo il manager per fare il lavoro
    }
}