// ReSharper disable InconsistentNaming

namespace BIMdance.Revit.Application;

public class ExternalApplicationBuilder
{
    private static readonly Func<IServiceProvider, object> CurrentScope = provider => provider.Get<ServiceScope>()?.Current;

    public string ApplicationName { get; }
    public UIControlledApplication UIControlledApplication { get; }
    public IServiceProvider ServiceProvider { get; private set; }

    public ExternalApplicationBuilder(string applicationName, UIControlledApplication uiControlledApplication)
    {
        ApplicationName = applicationName;
        UIControlledApplication = uiControlledApplication;
    }
    
    public ExternalApplicationBuilder AddServices()
    {
        BuildServiceProvider(CreateServiceCollection());
        return this;
    }
    
    public ExternalApplicationBuilder AddServices(
        Action<IServiceCollection> addServicesFunc)
    {
        var serviceCollection = CreateServiceCollection();
        addServicesFunc(serviceCollection);
        BuildServiceProvider(serviceCollection);
        return this;
    }
    
    public ExternalApplicationBuilder AddServices(
        Action<IServiceCollection, Func<IServiceProvider, object>> addServicesFunc)
    {
        var serviceCollection = CreateServiceCollection();
        addServicesFunc(serviceCollection, CurrentScope);
        BuildServiceProvider(serviceCollection);
        return this;
    }

    public ExternalApplicationBuilder AddServices(
        Action<ExternalApplicationBuilder, IServiceCollection, Func<IServiceProvider, object>> addServicesFunc)
    {
        var serviceCollection = CreateServiceCollection();
        addServicesFunc(this, serviceCollection, CurrentScope);
        BuildServiceProvider(serviceCollection);
        return this;
    }

    public ExternalApplicationBuilder AddRevitAsync()
    {
        RevitTask.Initialize(UIControlledApplication);
        return this;
    }
    
    public ExternalApplicationBuilder AddRibbon(
        Action<RibbonFactory> addRibbonAction)
    {
        addRibbonAction(CreateRibbonFactory());
        return this;
    }
    
    public ExternalApplicationBuilder AddRibbon(
        Action<ExternalApplicationBuilder, RibbonFactory> addRibbonAction)
    {
        addRibbonAction(this, CreateRibbonFactory());
        return this;
    }

    public ExternalApplicationBuilder RegisterUpdaters(
        Action registerUpdatersAction)
    {
        registerUpdatersAction();
        return this;
    }

    public ExternalApplicationBuilder RegisterUpdaters(
        Action<ExternalApplicationBuilder> registerUpdatersAction)
    {
        registerUpdatersAction(this);
        return this;
    }

    public ExternalApplicationBuilder RegisterUpdaters(
        Action<ExternalApplicationBuilder, UpdaterManager> registerUpdatersAction)
    {
        registerUpdatersAction(this, ServiceProvider.Get<UpdaterManager>());
        return this;
    }

    private ServiceCollection CreateServiceCollection()
    {
        var serviceCollection = new ServiceCollection();
        var revitVersion = UIControlledApplication.ControlledApplication.VersionNumber;
        
        serviceCollection.AddSingleton(x => x);
        serviceCollection.AddSingleton<ServiceScope>();
        serviceCollection.AddSingleton(_ => new AppInfo(ApplicationName, revitVersion));
        serviceCollection.AddSingleton(_ => new AppPaths(ApplicationName, revitVersion));
        serviceCollection.AddSingleton(_ => UIControlledApplication);
        serviceCollection.AddSingleton(_ => UIControlledApplication.ActiveAddInId);
        serviceCollection.AddSingleton<RevitContext>();
        
        serviceCollection.AddTransient(x => x.Get<RevitContext>().Document);    
        serviceCollection.AddTransient(x => x.Get<RevitContext>().UIDocument);
        serviceCollection.AddTransient(x => x.Get<RevitContext>().UIApplication);
        
        return serviceCollection;
    }

    private void BuildServiceProvider(IServiceCollection serviceCollection)
    {
        ServiceProvider = serviceCollection.BuildServiceProvider();
        Locator.Initialize(ServiceProvider);
    }

    private RibbonFactory CreateRibbonFactory()
    {
        var ribbonVisibleUtils = new RibbonVisibleUtils(UIControlledApplication);
        return new RibbonFactory(UIControlledApplication, ribbonVisibleUtils);
    }
}