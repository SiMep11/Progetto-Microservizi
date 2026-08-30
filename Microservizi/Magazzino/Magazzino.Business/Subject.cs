using Magazzino.Business.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using System.Reactive.Subjects;

namespace Magazzino.Business
{
    [method: ActivatorUtilitiesConstructor]
    public class Subject() : ISubject
    {
        private Subject<long> nuovoMessaggioOutbox { get; } = new Subject<long>();

        IObservable<long> IMagazzinoObservable.NuovoMessaggioOutbox => nuovoMessaggioOutbox;

        IObserver<long> IMagazzinoObserver.NuovoMessaggioOutbox => nuovoMessaggioOutbox;
    }
}