namespace BIMdance.Revit.Samples;

public class ExternalApplicationSample : IExternalApplication
{
    public Result OnStartup(UIControlledApplication application)
    {
        application
            .StartBuilding("Sample")
            .AddServices((serviceCollection, currentScope) =>
            {
                serviceCollection.AddToScope<AppPaths>(currentScope);
            })
            .AddRevitAsync()
            .AddRibbon((builder, ribbonFactory) => { })
            .RegisterUpdaters(() => { });
        
        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application) => Result.Succeeded;
}