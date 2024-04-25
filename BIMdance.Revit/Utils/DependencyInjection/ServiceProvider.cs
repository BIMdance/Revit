using BIMdance.Revit.Utils.DependencyInjection.ServiceLookup;

namespace BIMdance.Revit.Utils.DependencyInjection;

public sealed class ServiceProvider : IServiceProvider, IDisposable, IServiceProviderEngineCallback
{
    private readonly IServiceProviderEngine _engine;

    private readonly CallSiteValidator _callSiteValidator;

    internal ServiceProvider(IEnumerable<ServiceDescriptor> serviceDescriptors, ServiceProviderOptions options)
    {
        IServiceProviderEngineCallback callback = null;
        if (options.ValidateScopes)
        {
            callback = this;
            _callSiteValidator = new CallSiteValidator();
        }
        switch (options.Mode)
        {
            case ServiceProviderMode.Dynamic:
                _engine = new DynamicServiceProviderEngine(serviceDescriptors, callback);
                break;
            case ServiceProviderMode.Runtime:
                _engine = new RuntimeServiceProviderEngine(serviceDescriptors, callback);
                break;
#if IL_EMIT
                case ServiceProviderMode.ILEmit:
                    _engine = new ILEmitServiceProviderEngine(serviceDescriptors, callback);
                    break;
#endif
            case ServiceProviderMode.Expressions:
                _engine = new ExpressionsServiceProviderEngine(serviceDescriptors, callback);
                break;
            default:
                throw new NotSupportedException(nameof(options.Mode));
        }
    }

    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public object GetService(Type serviceType) => _engine.GetService(serviceType);

    /// <inheritdoc />
    public void Dispose() => _engine.Dispose();

    void IServiceProviderEngineCallback.OnCreate(IServiceCallSite callSite)
    {
        _callSiteValidator.ValidateCallSite(callSite);
    }

    void IServiceProviderEngineCallback.OnResolve(Type serviceType, IServiceScope scope)
    {
        _callSiteValidator.ValidateResolution(serviceType, scope, _engine.RootScope);
    }
}