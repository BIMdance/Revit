namespace BIMdance.Revit.Utils.DependencyInjection.ServiceLookup;

internal interface IServiceProviderEngineCallback
{
    void OnCreate(IServiceCallSite callSite);
    void OnResolve(Type serviceType, IServiceScope scope);
}