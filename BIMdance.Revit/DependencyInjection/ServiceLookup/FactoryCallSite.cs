namespace BIMdance.Revit.DependencyInjection.ServiceLookup;

internal class FactoryCallSite : IServiceCallSite
{
    public Func<IServiceProvider, object> Factory { get; }

    public FactoryCallSite(Type serviceType, Func<IServiceProvider, object> factory)
    {
        Factory = factory;
        ServiceType = serviceType;
    }

    public Type ServiceType { get; }
    public Type ImplementationType => null;

    public CallSiteKind Kind { get; } = CallSiteKind.Factory;
}