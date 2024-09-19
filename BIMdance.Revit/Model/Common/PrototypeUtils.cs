namespace BIMdance.Revit.Model.Common;

public static class PrototypeUtils
{
    public static T CloneAs<T>(this object prototype) where T : class
    {
        return prototype switch
        {
            IPrototype<T> prototypeT => prototypeT.Clone(),
            ICloneable cloneable => cloneable.Clone() as T,
            _ => null,
            // _ => throw new InvalidCastException($"{typeof(T).FullName} doesn't implement ICloneable or IPrototype<{typeof(T).FullName}> interfaces.")
        };
    }
    
    public static void PullProperties<T>(this object obj, object prototype) where T : class
    {
        if (obj is not IPropertyPrototype<T> recipient)
            return;
        // throw new InvalidCastException($"{typeof(T).FullName} doesn't implement IPropertyPrototype<{typeof(T).FullName}> interfaces.");
            
        PullProperties(recipient, prototype);
    }
    
    public static void PullProperties<T>(this IPropertyPrototype<T> obj, object prototype) where T : class
    {
        if (prototype is not T propertyPrototype)
            return;
        // throw new InvalidCastException($"{nameof(prototype)}:{prototype.GetType().FullName} isn't a {typeof(T).FullName}.");
            
        obj.PullProperties(propertyPrototype);
    }
}