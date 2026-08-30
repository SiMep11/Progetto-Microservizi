using Microsoft.Extensions.DependencyInjection;

namespace Magazzino.Business.Kafka;

/// <summary>
/// Topic che Magazzino ASCOLTA. Bindata dalla sezione "Kafka:Topics".
/// </summary>
public class KafkaTopicsInput : AbstractKafkaTopics
{
    public string OrdineConfermato { get; set; } = "ordine-confermato";
    public string MerceRicevuta { get; set; } = "merce-ricevuta";

    public override IEnumerable<string> GetTopics() => [OrdineConfermato, MerceRicevuta];
}

/// <summary>
/// Topic su cui Magazzino PUBBLICA. Bindata dalla stessa sezione "Kafka:Topics".
/// </summary>
public class KafkaTopicsOutput : AbstractKafkaTopics
{
    public string ScortaSottoSoglia { get; set; } = "scorta-sotto-soglia";

    public override IEnumerable<string> GetTopics() => [ScortaSottoSoglia];
}