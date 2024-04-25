// ReSharper disable InconsistentNaming

namespace BIMdance.Revit.Utils.DependencyInjection;

internal enum ServiceProviderMode
{
    Dynamic,
    Runtime,
    Expressions,
    ILEmit
}