using Approvvigionamenti.Business.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using System.Reactive.Subjects;

namespace Approvvigionamenti.Business
{
    [method: ActivatorUtilitiesConstructor]
    public class Subject() : ISubject
    {
        private Subject<long> nuovoMessaggioOutbox { get; } = new Subject<long>();

        IObservable<long> IApprovvigionamentiObservable.NuovoMessaggioOutbox => nuovoMessaggioOutbox;

        IObserver<long> IApprovvigionamentiObserver.NuovoMessaggioOutbox => nuovoMessaggioOutbox;
    }
}