namespace BIMdance.Revit.Utils.DependencyInjection.ServiceLookup;

internal class DynamicServiceProviderEngine : CompiledServiceProviderEngine
{
    public DynamicServiceProviderEngine(IEnumerable<ServiceDescriptor> serviceDescriptors, IServiceProviderEngineCallback callback) : base(serviceDescriptors, callback)
    {
    }

    protected override Func<ServiceProviderEngineScope, object> RealizeService(IServiceCallSite callSite)
    {
        var callCount = 0;
        return scope =>
        {
            if (Interlocked.Increment(ref callCount) == 2)
            {
                Task.Run(() => base.RealizeService(callSite));
            }
            return RuntimeResolver.Resolve(callSite, scope);
        };
    }
}