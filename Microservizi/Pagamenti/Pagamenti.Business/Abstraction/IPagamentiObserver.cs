using System;
using System.Collections.Generic;
using System.Text;

namespace Pagamenti.Business.Abstraction
{
    public interface IPagamentiObserver
    {
        IObserver<int> NuovoVersamento { get; }
    }
}
