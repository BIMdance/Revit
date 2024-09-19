namespace BIMdance.Revit.Utils.Common;

public static class ObjectExtension
{
    public static string AllValuesToString(this object obj)
    {
        if (obj is null) 
            return null;
        
        var nonGenericProperties = obj.GetType().GetProperties().Where(n => n.PropertyType.IsGenericType == false);
        var genericProperties = obj.GetType().GetProperties().Where(n => n.PropertyType.IsGenericType);
        var result = string.Empty;

        result += $"{nonGenericProperties.Select(n => { try { return $"{n.Name}: {n.GetValue(obj)}"; } catch { return $"{n.Name}:"; } }).JoinToString()}";
        result += $"{genericProperties.Select(n => { try { return $"{n.Name}: {(n.GetValue(obj) as IEnumerable<object>)?.Where(v => v != null).JoinToString("\n\t\t")}"; } catch { return $"{n.Name}:"; } }).JoinToString()}";

        return result;
    }

    public static void PullBuiltInTypePropertyValues(this object target, object source)
    {
        var nonClassProperties = target.GetType().GetProperties()
            .Where(n =>
                n.PropertyType.IsClass == false ||
                n.PropertyType == typeof(string));

        foreach (var propertyInfo in nonClassProperties)
        {
            var value = propertyInfo.GetValue(source);

            if (value != null)
                propertyInfo.SetValue(target, value);
        }
    }
}