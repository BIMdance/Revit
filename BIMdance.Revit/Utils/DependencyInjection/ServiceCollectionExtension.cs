namespace BIMdance.Revit.Utils.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static void AddToScope<T>(
        this IServiceCollection services,
        Func<IServiceProvider, object> scopeFunc)
        where T : class
    {
        services.AddToScope(
            _ => typeof(T).GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>()) as T,
            scopeFunc);
    }
        
    public static void AddToScope<T>(
        this IServiceCollection services,
        Func<IServiceProvider, T> implementation,
        Func<IServiceProvider, object> scopeFunc)
        where T : class
    {
        services.AddTransient(serviceProvider =>
        {
            var scopeObject = scopeFunc?.Invoke(serviceProvider);

            if (scopeObject == null)
                throw new NullReferenceException($"{nameof(scopeObject)} was not be set.");
                
            var serviceScope = serviceProvider.Get<ServiceScope>();

            if (serviceScope.TryGetScoped<T>(scopeObject, out var existedInScope))
                return existedInScope;

            var newImplementation = implementation?.Invoke(serviceProvider);
            
            if (newImplementation is not null)
                serviceScope.Add(scopeObject, newImplementation);
            
            return newImplementation;
        });
    }
}