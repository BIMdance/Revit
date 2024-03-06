namespace BIMdance.Revit.DependencyInjection.ServiceLookup;

internal class TransientCallSite : IServiceCallSite
{
    internal IServiceCallSite ServiceCallSite { get; }

    public TransientCallSite(IServiceCallSite serviceCallSite)
    {
        ServiceCallSite = serviceCallSite;
    }

    public Type ServiceType => ServiceCallSite.ServiceType;
    public Type ImplementationType => ServiceCallSite.ImplementationType;
    public CallSiteKind Kind { get; } = CallSiteKind.Transient;
}