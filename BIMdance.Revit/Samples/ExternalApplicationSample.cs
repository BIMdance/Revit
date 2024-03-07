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
            .AddRibbon((builder, ribbonFactory) =>
            {
                ribbonFactory.CreateRibbonPanel(builder.ApplicationName, RibbonVisible.All);
                ribbonFactory.AddPushButton(new CommandSample());
            })
            .RegisterUpdaters(() => { });
        
        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application) => Result.Succeeded;
}