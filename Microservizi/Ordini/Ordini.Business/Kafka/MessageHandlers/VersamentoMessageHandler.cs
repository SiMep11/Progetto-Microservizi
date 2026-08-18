using Microsoft.Extensions.Logging;
using Ordini.Business.Abstractions;
using Pagamenti.Shared;
using Utility.Kafka.MessageHandlers;

namespace Ordini.Business.Kafka.MessageHandlers;

public class VersamentoMessageHandler(
    ILogger<VersamentoMessageHandler> logger,
    IOrdiniService ordiniService)
    : AbstractOperationMessageHandlerBase<VersamentoReadDto>(logger)
{
    /// <summary>
    /// Un nuovo versamento è stato registrato in Pagamenti (Operation = "I").
    /// </summary>
    protected override async Task InsertAsync(VersamentoReadDto messageDto, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Ricevuto versamento {Id} di {Importo} per l'ordine {OrdineId}",
            messageDto.Id, messageDto.Importo, messageDto.ordineId);

        await ordiniService.ConfermaOrdineSePagatoAsync(messageDto.ordineId, messageDto.Importo, cancellationToken);
    }

    // Pagamenti produce solo Insert
    protected override Task UpdateAsync(VersamentoReadDto messageDto, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    protected override Task DeleteAsync(VersamentoReadDto messageDto, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}