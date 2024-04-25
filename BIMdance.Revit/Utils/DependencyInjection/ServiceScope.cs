#nullable enable

namespace BIMdance.Revit.Utils.DependencyInjection;

public class ServiceScope
{
    public object? Current { get; set; }
        
    private readonly Dictionary<object, Dictionary<Type, object>> _dictionary = new();

    public bool TryGetScoped<T>(object scopeObject, out T? result) where T : class
    {
        result = _dictionary.TryGetValue(scopeObject, out var scope) &&
                 scope.TryGetValue(typeof(T), out var value)
            ? value as T
            : default;

        return result != default;
    }

    public void Add<T>(object scope, T add) where T : class
    {
        var type = typeof(T);
            
        if (_dictionary.TryGetValue(scope, out var scopes))
        {
            if (!scopes.ContainsKey(type))
            {
                scopes.Add(type, add);
            }
        }
        else
        {
            _dictionary.Add(scope, new Dictionary<Type, object>
            {
                [type] = add
            });
        }
    }

    public void Remove(object scope)
    {
        _dictionary.Remove(scope);
            
        if (Current == scope)
            Current = null;
    }
}