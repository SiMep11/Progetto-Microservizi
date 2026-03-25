using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Ordini.Business.Abstractions;
using Ordini.Shared.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Ordini.Business.Kafka
{
    public class KafkaEventProducer : IEventProducer
    {
        private readonly string _topic = "ordine-creato"; // Il "canale radio" su cui trasmettiamo
        private readonly string _bootstrapServers; // <-- Creiamo una variabile per salvarci l'indirizzo

        public KafkaEventProducer(IConfiguration configuration)
        {
            // Lo leggiamo una volta sola quando la classe viene creata e lo salviamo
            _bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
        }

        public async Task ProduceOrdineCreatoAsync(OrdineCreatoEvent evento)
        {
            var config = new ProducerConfig { 
                BootstrapServers = _bootstrapServers,
                MessageTimeoutMs = 5000
            };

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
