using Approvvigionamenti.Business.Abstraction;
using Approvvigionamenti.Shared.Events;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Utility.Kafka.Abstractions.MessageHandlers;

namespace Approvvigionamenti.Business.Kafka.MessageHandlers;

// NB: Magazzino pubblica l'evento non incapsulato in un OperationMessage, quindi implementiamo direttamente IMessageHandler invece di derivare da
// AbstractOperationMessageHandlerBase come fa Ordini sul topic dei versamenti.
public class ScortaSottoSogliaMessageHandler(
    ILogger<ScortaSottoSogliaMessageHandler> logger,
    IApprovvigionamentiManager manager) : IMessageHandler<string, string>
{
    public async Task OnMessageReceivedAsync(string key, string message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            logger.LogError("Il messaggio è null o white space e quindi sarà scartato");
            return;
        }

        ScortaSottoSogliaEvent? evento;
        try
        {
            evento = JsonSerializer.Deserialize<ScortaSottoSogliaEvent>(message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Messaggio non deserializzabile in {tipo}, sarà scartato: {msg}",
                nameof(ScortaSottoSogliaEvent), message);
            return;
        }

        if (evento is null)
        {
            logger.LogError("Il messaggio non è del tipo {tipo}, sarà scartato", nameof(ScortaSottoSogliaEvent));
            return;
        }

        logger.LogInformation("Prodotto {ProdottoId} sceso a {Quantita} (soglia {Soglia})",
            evento.ProdottoId, evento.QuantitaAttuale, evento.ScortaMinima);

        await manager.RiordinaSeNecessarioAsync(evento.ProdottoId, evento.QuantitaAttuale, cancellationToken);
    }
}