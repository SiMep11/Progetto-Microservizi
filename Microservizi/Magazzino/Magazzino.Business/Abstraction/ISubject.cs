namespace Magazzino.Business.Abstraction
{
    public interface IMagazzinoObservable
    {
        IObservable<long> NuovoMessaggioOutbox { get; }
    }

    public interface IMagazzinoObserver
    {
        IObserver<long> NuovoMessaggioOutbox { get; }
    }

    public interface ISubject : IMagazzinoObservable, IMagazzinoObserver
    {
    }
}