// ReSharper disable InconsistentNaming

namespace BIMdance.Revit.DependencyInjection.ServiceLookup;

internal enum CallSiteKind
{
    Factory,
    Constructor,
    Constant,
    IEnumerable,
    ServiceProvider,
    Scope,
    Transient,
    CreateInstance,
    ServiceScopeFactory,
    Singleton
}