namespace BIMdance.Revit.Utils.DependencyInjection.ServiceLookup;

internal class ServiceProviderCallSite : IServiceCallSite
{
    public Type ServiceType { get; } = typeof(IServiceProvider);
    public Type ImplementationType { get; } = typeof(ServiceProvider);
    public CallSiteKind Kind { get; } = CallSiteKind.ServiceProvider;
}