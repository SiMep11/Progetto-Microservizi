using Ordini.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordini.Business.Kafka
{
    public interface IEventProducer
    {
        Task ProduceOrdineCreatoAsync(OrdineCreatoEvent evento);
    }
}
