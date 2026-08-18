using Microsoft.Extensions.DependencyInjection;

namespace Approvvigionamenti.Business.Kafka;

public static class KafkaTopics
{
    // Il canale su cui trasmetto: la merce è arrivata dal fornitore, il magazzino può ricaricare la giacenza.
    public const string MerceRicevuta = "merce-ricevuta";
}

/// <summary>
/// Topic che Approvvigionamenti ASCOLTA. Viene bindata dalla sezione "Kafka:Topics".
/// </summary>
public class KafkaTopicsInput : AbstractKafkaTopics
{
    public string ScortaSottoSoglia { get; set; } = "scorta-sotto-soglia";

    public override IEnumerable<string> GetTopics() => [ScortaSottoSoglia];
}