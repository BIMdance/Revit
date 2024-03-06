namespace BIMdance.Revit.DependencyInjection.ServiceLookup;

internal class RuntimeServiceProviderEngine : ServiceProviderEngine
{
    public RuntimeServiceProviderEngine(IEnumerable<ServiceDescriptor> serviceDescriptors, IServiceProviderEngineCallback callback) : base(serviceDescriptors, callback)
    {
    }

    protected override Func<ServiceProviderEngineScope, object> RealizeService(IServiceCallSite callSite)
    {
        return scope =>
        {
            Func<ServiceProviderEngineScope, object> realizedService = p => RuntimeResolver.Resolve(callSite, p);

            RealizedServices[callSite.ServiceType] = realizedService;
            return realizedService(scope);
        };
    }
}