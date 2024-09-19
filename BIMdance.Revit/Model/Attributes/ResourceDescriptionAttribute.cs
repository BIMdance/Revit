namespace BIMdance.Revit.Model.Attributes;

public class ResourceDescriptionAttribute : DescriptionAttribute
{
    public ResourceDescriptionAttribute(Type resourceManagerProvider, string key) :
        base(GetValue(resourceManagerProvider, key))
    { }

    private static string GetValue(Type resourceManagerProvider, string key) =>
        new ResourceManager(resourceManagerProvider).GetString(key);
}