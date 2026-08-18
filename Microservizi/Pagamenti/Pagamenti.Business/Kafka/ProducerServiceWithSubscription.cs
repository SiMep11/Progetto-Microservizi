using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pagamenti.Repository.Abstraction;
using Pagamenti.Repository.Model;
using Pagamenti.Business.Abstraction;
using Utility.Kafka.Abstractions.Clients;
using Utility.Kafka.Services;

namespace Pagamenti.Business.Kafka; 

public class ProducerServiceWithSubscription(
    ILogger<ProducerServiceWithSubscription> logger,
    IProducerClient<string, string> producerClient,
    IOptions<KafkaTopicsOutput> optionsTopics,
    IServiceProvider serviceProvider,
    IServiceScopeFactory serviceScopeFactory,
    IPagamentiObservable observable)
    : AbstractProducerServiceWithSubscription(logger, serviceProvider)
{
    /// <summary>
    /// subscribe dell'observable NuovoVersamento: ogni volta che viene inoltrata una notifica tramite l'observer segno come completato il tcs invocando il metodo TrySetResult()
    /// </summary>
    protected override IDisposable Subscribe(TaskCompletionSource tcs)
    {
        return observable.NuovoVersamento.Subscribe((change) => tcs.TrySetResult());
    }

    protected override IEnumerable<string> GetTopics()
    {
        return optionsTopics.Value.GetTopics();
    }

    protected override async Task OperationsAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();

        logger.LogInformation("Acquisizione dei TransactionalOutbox da elaborare...");
        IEnumerable<TransactionalOutbox> transactionalOutboxList = (await repository.GetAllTransactionalOutboxAsync(cancellationToken)).OrderBy(x => x.Id);

        if (!transactionalOutboxList.Any())
        {
            logger.LogInformation("Non ci sono TransactionalOutbox da elaborare");
            return;
        }

        logger.LogInformation("Ci sono {Count} TransactionalOutbox da elaborare", transactionalOutboxList.Count());

        foreach (TransactionalOutbox tran in transactionalOutboxList)
        {
            string groupMsg = $"del record {nameof(TransactionalOutbox)} con " +
                              $"{nameof(TransactionalOutbox.Id)} = {tran.Id}, " +
                              $"{nameof(TransactionalOutbox.Tabella)} = '{tran.Tabella}' e " +
                              $"{nameof(TransactionalOutbox.Messaggio)} = '{tran.Messaggio}'";

            logger.LogInformation("Elaborazione {groupMsg}...", groupMsg);

            try
            {
                logger.LogInformation("Determinazione del topic...");
                string topic = tran.Tabella switch
                {
                    nameof(Versamento) => optionsTopics.Value.Versamenti,
                    _ => throw new ArgumentOutOfRangeException($"La tabella {tran.Tabella} non è prevista come topic nel Producer")
                };

                logger.LogInformation("Scrittura del messaggio Kafka sul topic '{topic}'...", topic);
                await producerClient.ProduceAsync(topic, tran.Id.ToString(), tran.Messaggio, cancellationToken);

                logger.LogInformation("Eliminazione {groupMsg}...", groupMsg);
                await repository.DeleteTransactionalOutboxAsync(tran.Id, cancellationToken);

                await repository.SaveChangesAsync(cancellationToken);
                logger.LogInformation("Record eliminato");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Si è verificata un'eccezione nel metodo ProducerService.OperationsAsync durante l'elaborazione {groupMsg}: {ex}", groupMsg, ex);
            }
        }
    }
}
