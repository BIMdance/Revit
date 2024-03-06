namespace BIMdance.Revit.DependencyInjection.ServiceLookup;

internal class CreateInstanceCallSite : IServiceCallSite
{
    public Type ServiceType { get; }

    public Type ImplementationType { get; }
    public CallSiteKind Kind { get; } = CallSiteKind.CreateInstance;

    public CreateInstanceCallSite(Type serviceType, Type implementationType)
    {
        ServiceType = serviceType;
        ImplementationType = implementationType;
    }
}