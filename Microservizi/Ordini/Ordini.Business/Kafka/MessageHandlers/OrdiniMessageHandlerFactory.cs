using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Utility.Kafka.Abstractions.MessageHandlers;

namespace Ordini.Business.Kafka.MessageHandlers;

public class OrdiniMessageHandlerFactory(IOptions<KafkaTopicsInput> optionsTopics) : IMessageHandlerFactory<string, string>
{
    public IMessageHandler<string, string> Create(string topic, IServiceProvider serviceProvider)
    {
        if (topic == optionsTopics.Value.Versamenti)
            return ActivatorUtilities.CreateInstance<VersamentoMessageHandler>(serviceProvider);

        throw new ArgumentOutOfRangeException(nameof(topic), $"Nessun handler configurato per il topic '{topic}'");
    }
}