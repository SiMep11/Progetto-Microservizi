using Microsoft.Extensions.DependencyInjection;

namespace Approvvigionamenti.Business.Kafka;

/// <summary>
/// Topic che Approvvigionamenti ASCOLTA. Bindata dalla sezione "Kafka:Topics".
/// </summary>
public class KafkaTopicsInput : AbstractKafkaTopics
{
    public string ScortaSottoSoglia { get; set; } = "scorta-sotto-soglia";

    public override IEnumerable<string> GetTopics() => [ScortaSottoSoglia];
}

/// <summary>
/// Topic su cui Approvvigionamenti PUBBLICA. Bindata dalla stessa sezione "Kafka:Topics".
/// </summary>
public class KafkaTopicsOutput : AbstractKafkaTopics
{
    public string MerceRicevuta { get; set; } = "merce-ricevuta";

    public override IEnumerable<string> GetTopics() => [MerceRicevuta];
}