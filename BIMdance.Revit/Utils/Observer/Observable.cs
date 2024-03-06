namespace BIMdance.Revit.Utils.Observer;

public class Observable<T> : IObservable<T>
{
    private readonly List<IObserver<T>> _observers;
    private T _state;

    public Observable(T state)
    {
        _state = state;
        _observers = new List<IObserver<T>>();
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);

        return new Unsubscriber<T>(_observers, observer);
    }

    public void Notify(T newState)
    {
        _state = newState;
        _observers.ForEach(n => n.OnNext(_state));
    }

    public void NotifyPropertyChanged()
    {
        _observers.ForEach(n => n.OnNext(_state));
    }
}