using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using UtilityKafka.Abstractions;

namespace UtilityKafka
{
    internal class KafkaProducer : IKafkaProducer
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducer(string bootstrapServers)
        {
            var config = new ProducerConfig { BootstrapServers = bootstrapServers };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task ProduceAsync<T>(string topic, T message)
        {
            // Trasforma il tuo oggetto C# in una stringa JSON
            var messageString = JsonSerializer.Serialize(message);

            // Invia il messaggio a Kafka
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = messageString });
        }
    }
}
