namespace BIMdance.Revit.Utils.DependencyInjection.ServiceLookup;

internal interface IServiceProviderEngine : IDisposable, IServiceProvider
{
    IServiceScope RootScope { get; }
}