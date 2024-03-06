// ReSharper disable InconsistentNaming

namespace BIMdance.Revit.DependencyInjection;

internal enum ServiceProviderMode
{
    Dynamic,
    Runtime,
    Expressions,
    ILEmit
}