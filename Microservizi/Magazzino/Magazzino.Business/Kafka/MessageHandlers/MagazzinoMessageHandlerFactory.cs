using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Utility.Kafka.Abstractions.MessageHandlers;

namespace Magazzino.Business.Kafka.MessageHandlers;

public class MagazzinoMessageHandlerFactory(IOptions<KafkaTopicsInput> optionsTopics) : IMessageHandlerFactory<string, string>
{
    public IMessageHandler<string, string> Create(string topic, IServiceProvider serviceProvider)
    {
        if (topic == optionsTopics.Value.OrdineConfermato)
            return ActivatorUtilities.CreateInstance<OrdineConfermatoMessageHandler>(serviceProvider);

        if (topic == optionsTopics.Value.MerceRicevuta)
            return ActivatorUtilities.CreateInstance<MerceRicevutaMessageHandler>(serviceProvider);

        throw new ArgumentOutOfRangeException(nameof(topic), $"Nessun handler configurato per il topic '{topic}'");
    }
}