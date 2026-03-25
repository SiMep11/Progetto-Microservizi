using Microsoft.Extensions.Configuration;
using Ordini.Shared.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordini.Business.Abstractions
{
    public interface IEventProducer
    {
        Task ProduceOrdineCreatoAsync(OrdineCreatoEvent evento);
    }
}
