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

                serviceCollection.AddSingleton<UpdaterManager>();
                serviceCollection.AddSingleton<UpdateStatus<CableTray>>();
                serviceCollection.AddSingleton<UpdaterSample>();
                serviceCollection.AddSingleton(provider => new List<Updater>
                {
                    provider.Get<UpdaterSample>(),
                });
            })
            .AddRevitAsync()
            .AddRibbon((builder, ribbonFactory) =>
            {
                ribbonFactory.CreateRibbonPanel(builder.ApplicationName, RibbonVisible.All);
                ribbonFactory.AddPushButton(new CommandSample());
            })
            .RegisterUpdaters((builder, updaterManager) =>
            {
                updaterManager.RegisterUpdaters();
            });
        
        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application) => Result.Succeeded;
}