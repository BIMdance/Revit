namespace BIMdance.Revit.DependencyInjection.ServiceLookup;

internal interface IServiceProviderEngine : IDisposable, IServiceProvider
{
    IServiceScope RootScope { get; }
}