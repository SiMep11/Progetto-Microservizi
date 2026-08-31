namespace Approvvigionamenti.Business.Abstraction
{
    public interface IApprovvigionamentiObservable
    {
        IObservable<long> NuovoMessaggioOutbox { get; }
    }

    public interface IApprovvigionamentiObserver
    {
        IObserver<long> NuovoMessaggioOutbox { get; }
    }

    public interface ISubject : IApprovvigionamentiObservable, IApprovvigionamentiObserver
    {
    }
}