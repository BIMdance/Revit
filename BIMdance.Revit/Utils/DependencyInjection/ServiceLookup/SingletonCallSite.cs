namespace BIMdance.Revit.Utils.DependencyInjection.ServiceLookup;

internal class SingletonCallSite : ScopedCallSite
{
    public SingletonCallSite(IServiceCallSite serviceCallSite, object cacheKey) : base(serviceCallSite, cacheKey)
    {
    }

    public override CallSiteKind Kind { get; } = CallSiteKind.Singleton;
}