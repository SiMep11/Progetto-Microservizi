using Magazzino.Business.Abstraction;
using Magazzino.Shared.Events;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Utility.Kafka.Abstractions.MessageHandlers;

namespace Magazzino.Business.Kafka.MessageHandlers;

public class OrdineConfermatoMessageHandler(
    ILogger<OrdineConfermatoMessageHandler> logger,
    IProdottiManager manager) : IMessageHandler<string, string>
{
    public async Task OnMessageReceivedAsync(string key, string message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            logger.LogError("Il messaggio è null o white space e quindi sarà scartato");
            return;
        }

        OrdineConfermatoEvent? evento;
        try
        {
            evento = JsonSerializer.Deserialize<OrdineConfermatoEvent>(message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Messaggio non deserializzabile in {tipo}, sarà scartato: {msg}",
                nameof(OrdineConfermatoEvent), message);
            return;
        }

        if (evento is null)
        {
            logger.LogError("Il messaggio non è del tipo {tipo}, sarà scartato", nameof(OrdineConfermatoEvent));
            return;
        }

        foreach (var p in evento.Prodotti)
        {
            await manager.ScaricaGiacenzaAsync(p.ProdottoId, p.Quantita, cancellationToken);
            logger.LogInformation("Ordine {OrdineId} pagato: scalati {Quantita} pezzi del prodotto {ProdottoId}",
                evento.OrdineId, p.Quantita, p.ProdottoId);
        }
    }
}