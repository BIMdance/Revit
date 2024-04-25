namespace BIMdance.Revit.Utils.DependencyInjection.ServiceLookup;

internal interface IServiceCallSite
{
    Type ServiceType { get; }
    Type ImplementationType { get; }
    CallSiteKind Kind { get; }
}