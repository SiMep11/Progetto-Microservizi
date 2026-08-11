using Microsoft.Extensions.DependencyInjection;

namespace Ordini.Business.Kafka;

public static class KafkaTopics
{
    // Il "canale radio" su cui trasmettiamo
    public const string OrdineCreato = "ordine-creato";
}

public class KafkaTopicsInput : AbstractKafkaTopics
{
    public string Versamenti { get; set; } = "Versamenti";

    public override IEnumerable<string> GetTopics() => [Versamenti];
}