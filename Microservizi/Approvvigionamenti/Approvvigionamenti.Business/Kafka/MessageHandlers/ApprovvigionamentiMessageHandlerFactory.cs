using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Utility.Kafka.Abstractions.MessageHandlers;

namespace Approvvigionamenti.Business.Kafka.MessageHandlers;

public class ApprovvigionamentiMessageHandlerFactory(IOptions<KafkaTopicsInput> optionsTopics) : IMessageHandlerFactory<string, string>
{
	public IMessageHandler<string, string> Create(string topic, IServiceProvider serviceProvider)
	{
		if (topic == optionsTopics.Value.ScortaSottoSoglia)
			return ActivatorUtilities.CreateInstance<ScortaSottoSogliaMessageHandler>(serviceProvider);

		throw new ArgumentOutOfRangeException(nameof(topic), $"Nessun handler configurato per il topic '{topic}'");
	}
}