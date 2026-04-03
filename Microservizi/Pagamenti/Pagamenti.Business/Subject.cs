using Microsoft.Extensions.DependencyInjection;
using Pagamenti.Business.Abstraction;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;

namespace Pagamenti.Business
{
    [method: ActivatorUtilitiesConstructor]
    public class Subject() : ISubject
    {
        private Subject<int> nuovoVersamento { get; } = new Subject<int>();

        IObservable<int> IPagamentiObservable.NuovoVersamento => nuovoVersamento;

        IObserver<int> IPagamentiObserver.NuovoVersamento => nuovoVersamento;
    }
}
