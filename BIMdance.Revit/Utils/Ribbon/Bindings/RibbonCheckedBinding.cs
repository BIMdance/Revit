namespace BIMdance.Revit.Utils.Ribbon.Bindings;

public class RibbonCheckedBinding : IObserver<bool>, INotifyPropertyChanged
{
    private readonly IDisposable _unsubscriber;
    private readonly Observable<bool> _observable;
    private bool _isChecked;

    public RibbonCheckedBinding(Observable<bool> observable)
    {
        _observable = observable;
        _unsubscriber = _observable.Subscribe(this);
    }

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (Equals(_isChecked, value))
                return;
                
            _isChecked = value;
            _observable.Notify(value);

            OnPropertyChanged();
        }
    }

    public void OnNext(bool value) => IsChecked = value;
    public void OnError(Exception error) { }
    public void OnCompleted() => _unsubscriber.Dispose();

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}