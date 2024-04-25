// ReSharper disable InconsistentNaming

namespace BIMdance.Revit.Utils.DependencyInjection;

public static class ServiceProviderExtension
{
    public static T Get<T>(this IServiceProvider serviceProvider) where T : class
    {
        var service = serviceProvider.GetService(typeof(T)) ?? throw new InvalidOperationException($"<{typeof(T).FullName}> was not added to the {nameof(ServiceProvider)}.");
        return service as T ?? throw new InvalidCastException($"Invalid casting '{service}' to <{typeof(T).FullName}>");
    }
}