namespace BIMdance.Revit.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static void AddToScope<T>(
        this IServiceCollection services,
        Func<IServiceProvider, object> scope)
        where T : class
    {
        services.AddToScope(
            _ => typeof(T).GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>()) as T,
            scope);
    }
        
    public static void AddToScope<T>(
        this IServiceCollection services,
        Func<IServiceProvider, T> implementation,
        Func<IServiceProvider, object> scope)
        where T : class
    {
        services.AddTransient(serviceProvider =>
        {
            var scopeObject = scope?.Invoke(serviceProvider);

            if (scopeObject == null)
                throw new NullReferenceException($"{nameof(scopeObject)} was not be set.");
                
            var serviceScope = serviceProvider.Get<ServiceScope>();

            if (serviceScope.TryGetScoped<T>(scopeObject, out var existedInScope))
                return existedInScope;

            var newImplementation = implementation?.Invoke(serviceProvider);
            serviceScope.Add(scopeObject, newImplementation);
            return newImplementation;
        });
    }
}