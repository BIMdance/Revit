// ReSharper disable InconsistentNaming

namespace BIMdance.Revit.Utils.DependencyInjection.ServiceLookup;

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