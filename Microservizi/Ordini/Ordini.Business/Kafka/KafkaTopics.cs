using Microsoft.Extensions.DependencyInjection;

namespace Ordini.Business.Kafka;

public static class KafkaTopics
{
    // Il canale su cui trasmetto: l'ordine è stato pagato e confermato, quindi il magazzino può scaricare la giacenza.
    public const string OrdineConfermato = "ordine-confermato";
}

public class KafkaTopicsInput : AbstractKafkaTopics
{
    public string Versamenti { get; set; } = "Versamenti";

    public override IEnumerable<string> GetTopics() => [Versamenti];
}