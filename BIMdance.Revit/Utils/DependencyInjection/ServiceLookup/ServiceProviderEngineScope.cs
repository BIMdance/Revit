#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace BIMdance.Revit.Utils.DependencyInjection.ServiceLookup;

internal class ServiceProviderEngineScope : IServiceScope, IServiceProvider
{
    // For testing only
    internal Action<object> CaptureDisposableCallback;

    private List<IDisposable> _disposables;

    private bool _disposed;

    public ServiceProviderEngineScope(ServiceProviderEngine engine)
    {
        Engine = engine;
    }

    internal Dictionary<object, object> ResolvedServices { get; } = new();

    public ServiceProviderEngine Engine { get; }

    public object GetService(Type serviceType)
    {
        if (_disposed)
        {
            ThrowHelper.ThrowObjectDisposedException();
        }

        return Engine.GetService(serviceType, this);
    }

    public IServiceProvider ServiceProvider => this;

    public void Dispose()
    {
        lock (ResolvedServices)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            if (_disposables != null)
            {
                for (var i = _disposables.Count - 1; i >= 0; i--)
                {
                    var disposable = _disposables[i];
                    disposable.Dispose();
                }

                _disposables.Clear();
            }

            ResolvedServices.Clear();
        }
    }

    internal object CaptureDisposable(object service)
    {
        CaptureDisposableCallback?.Invoke(service);

        if (ReferenceEquals(this, service))
            return service;
        
        if (service is not IDisposable disposable)
            return service;
            
        lock (ResolvedServices)
        {
            _disposables ??= new List<IDisposable>();
            _disposables.Add(disposable);
        }
        return service;
    }
}