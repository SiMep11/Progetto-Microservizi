using Microsoft.Extensions.DependencyInjection;

namespace Ordini.Business.Kafka;

/// <summary>
/// Topic che Ordini ASCOLTA. Bindata dalla sezione "Kafka:Topics".
/// </summary>
public class KafkaTopicsInput : AbstractKafkaTopics
{
    public string Versamenti { get; set; } = "Versamenti";

    public override IEnumerable<string> GetTopics() => [Versamenti];
}

/// <summary>
/// Topic su cui Ordini PUBBLICA. Bindata dalla stessa sezione "Kafka:Topics".
/// </summary>
public class KafkaTopicsOutput : AbstractKafkaTopics
{
    public string OrdineConfermato { get; set; } = "ordine-confermato";

    public override IEnumerable<string> GetTopics() => [OrdineConfermato];
}