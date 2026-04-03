using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityKafka.Abstractions
{
    public interface IKafkaProducer
    {
        Task ProduceAsync<T>(string topic, T message);
    }
}
