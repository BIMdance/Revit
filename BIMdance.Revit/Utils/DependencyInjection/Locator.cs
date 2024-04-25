// ReSharper disable InconsistentNaming

namespace BIMdance.Revit.Utils.DependencyInjection;

public static class Locator
{
    public static IServiceProvider ServiceProvider { get; private set; }
    public static IServiceProvider Initialize(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;
    
    public static T Get<T>() where T : class
    {
        if (ServiceProvider is null) throw new NullReferenceException($"{nameof(ServiceProvider)} was not defined. Run {nameof(Initialize)} method before calling {nameof(Get)}<{nameof(T)}>");
        return ServiceProvider.Get<T>();
    }
}