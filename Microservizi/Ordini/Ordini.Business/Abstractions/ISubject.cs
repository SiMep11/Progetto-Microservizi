namespace Ordini.Business.Abstractions
{
    public interface IOrdiniObservable
    {
        IObservable<long> NuovoMessaggioOutbox { get; }
    }

    public interface IOrdiniObserver
    {
        IObserver<long> NuovoMessaggioOutbox { get; }
    }

    public interface ISubject : IOrdiniObservable, IOrdiniObserver
    {
    }
}