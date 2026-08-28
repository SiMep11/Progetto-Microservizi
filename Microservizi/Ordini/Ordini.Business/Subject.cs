using Microsoft.Extensions.DependencyInjection;
using Ordini.Business.Abstractions;
using System.Reactive.Subjects;

namespace Ordini.Business
{
    [method: ActivatorUtilitiesConstructor]
    public class Subject() : ISubject
    {
        private Subject<long> nuovoMessaggioOutbox { get; } = new Subject<long>();

        IObservable<long> IOrdiniObservable.NuovoMessaggioOutbox => nuovoMessaggioOutbox;

        IObserver<long> IOrdiniObserver.NuovoMessaggioOutbox => nuovoMessaggioOutbox;
    }
}