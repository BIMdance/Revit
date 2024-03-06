namespace BIMdance.Revit.DependencyInjection.ServiceLookup;

internal class ConstructorCallSite : IServiceCallSite
{
    internal ConstructorInfo ConstructorInfo { get; }
    internal IServiceCallSite[] ParameterCallSites { get; }

    public ConstructorCallSite(Type serviceType, ConstructorInfo constructorInfo, IServiceCallSite[] parameterCallSites)
    {
        ServiceType = serviceType;
        ConstructorInfo = constructorInfo;
        ParameterCallSites = parameterCallSites;
    }

    public Type ServiceType { get; }

    public Type ImplementationType => ConstructorInfo.DeclaringType;
    public CallSiteKind Kind { get; } = CallSiteKind.Constructor;
}