namespace BIMdance.Revit.Utils.DependencyInjection.ServiceLookup;

internal class ServiceScopeFactoryCallSite : IServiceCallSite
{
    public Type ServiceType { get; } = typeof(IServiceScopeFactory);
    public Type ImplementationType { get; } = typeof(ServiceProviderEngine);
    public CallSiteKind Kind { get; } = CallSiteKind.ServiceScopeFactory;
}