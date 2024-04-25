namespace BIMdance.Revit.Utils.DependencyInjection.ServiceLookup;

internal abstract class CompiledServiceProviderEngine : ServiceProviderEngine
{
    public ExpressionResolverBuilder ExpressionResolverBuilder { get; }

    public CompiledServiceProviderEngine(IEnumerable<ServiceDescriptor> serviceDescriptors, IServiceProviderEngineCallback callback) : base(serviceDescriptors, callback)
    {
        ExpressionResolverBuilder = new ExpressionResolverBuilder(RuntimeResolver, this, Root);
    }

    protected override Func<ServiceProviderEngineScope, object> RealizeService(IServiceCallSite callSite)
    {
        var realizedService = ExpressionResolverBuilder.Build(callSite);
        RealizedServices[callSite.ServiceType] = realizedService;
        return realizedService;
    }
}