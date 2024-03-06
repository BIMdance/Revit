namespace BIMdance.Revit.DependencyInjection.ServiceLookup;

internal interface IServiceProviderEngineCallback
{
    void OnCreate(IServiceCallSite callSite);
    void OnResolve(Type serviceType, IServiceScope scope);
}