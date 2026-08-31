using Approvvigionamenti.Business.Abstraction;
using Approvvigionamenti.Repository.Abstraction;
using Approvvigionamenti.Repository.Models;
using Approvvigionamenti.Shared.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Utility.Kafka.Abstractions.Clients;
using Utility.Kafka.Services;

namespace Approvvigionamenti.Business.Kafka;

public class ProducerServiceWithSubscription(
    ILogger<ProducerServiceWithSubscription> logger,
    IProducerClient<string, string> producerClient,
    IOptions<KafkaTopicsOutput> optionsTopics,
    IServiceProvider serviceProvider,
    IServiceScopeFactory serviceScopeFactory,
    IApprovvigionamentiObservable observable)
    : AbstractProducerServiceWithSubscription(logger, serviceProvider)
{
    /// <summary>
    /// Ogni volta che il Business segnala un nuovo messaggio in outbox completiamo
    /// il tcs, e il loop del background service riparte subito.
    /// </summary>
    protected override IDisposable Subscribe(TaskCompletionSource tcs)
        => observable.NuovoMessaggioOutbox.Subscribe(_ => tcs.TrySetResult());

    protected override IEnumerable<string> GetTopics() => optionsTopics.Value.GetTopics();

    protected override async Task OperationsAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IApprovvigionamentiRepository repository = scope.ServiceProvider.GetRequiredService<IApprovvigionamentiRepository>();

        IEnumerable<TransactionalOutbox> lista =
            (await repository.GetAllTransactionalOutboxAsync(cancellationToken)).OrderBy(x => x.Id);

        if (!lista.Any())
        {
            logger.LogInformation("Nessun TransactionalOutbox da elaborare");
            return;
        }

        foreach (TransactionalOutbox tran in lista)
        {
            try
            {
                string topic = tran.Tabella switch
                {
                    nameof(MerceRicevutaEvent) => optionsTopics.Value.MerceRicevuta,
                    _ => throw new ArgumentOutOfRangeException(nameof(tran.Tabella),
                            $"Nessun topic previsto per '{tran.Tabella}'")
                };

                await producerClient.ProduceAsync(topic, tran.Id.ToString(), tran.Messaggio, cancellationToken);

                // Cancelliamo SOLO dopo la conferma di consegna: se moriamo qui in mezzo
                // la riga resta e il messaggio riparte al giro dopo. Il duplicato è
                // innocuo perché il consumer è idempotente.
                await repository.DeleteTransactionalOutboxAsync(tran.Id, cancellationToken);
                await repository.SaveChangesAsync(cancellationToken);

                logger.LogInformation("TransactionalOutbox {Id} pubblicato su '{topic}'", tran.Id, topic);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Errore pubblicando il TransactionalOutbox {Id}: {msg}", tran.Id, ex.Message);
            }
        }
    }
}