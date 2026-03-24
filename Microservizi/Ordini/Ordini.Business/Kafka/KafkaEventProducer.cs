using Confluent.Kafka;
using Ordini.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Ordini.Business.Kafka
{
    public class KafkaEventProducer : IEventProducer
    {
        private readonly string _bootstrapServers = "localhost:9092";
        private readonly string _topic = "ordini-creati"; // Il "canale radio" su cui trasmettiamo

        public async Task ProduceOrdineCreatoAsync(OrdineCreatoEvent evento)
        {
            var config = new ProducerConfig { BootstrapServers = _bootstrapServers };

            // Creiamo il produttore Kafka
            using var producer = new ProducerBuilder<Null, string>(config).Build();

            try
            {
                // Trasformiamo il nostro evento C# in una stringa JSON
                var messaggioJson = JsonSerializer.Serialize(evento);

                var message = new Message<Null, string> { Value = messaggioJson };

                // Inviamo il messaggio al topic
                var deliveryResult = await producer.ProduceAsync(_topic, message);
                Console.WriteLine($"[KAFKA] Evento inviato con successo al topic: {deliveryResult.TopicPartitionOffset}");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"[KAFKA] Errore durante l'invio del messaggio: {e.Error.Reason}");
            }
        }
    }
}
