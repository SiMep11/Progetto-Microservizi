using Microsoft.Extensions.DependencyInjection;

namespace Magazzino.Business.Kafka;

public static class KafkaTopics
{
    // Il "canale radio" su cui trasmetto: un prodotto è sceso sotto la scorta minima
    public const string ScortaSottoSoglia = "scorta-sotto-soglia";
}

/// <summary>
/// Topic che Magazzino ASCOLTA. Viene bindata dalla sezione "Kafka:Topics".
/// </summary>
public class KafkaTopicsInput : AbstractKafkaTopics
{
    public string OrdineConfermato { get; set; } = "ordine-confermato";
    public string MerceRicevuta { get; set; } = "merce-ricevuta";

    public override IEnumerable<string> GetTopics() => [OrdineConfermato, MerceRicevuta];
}