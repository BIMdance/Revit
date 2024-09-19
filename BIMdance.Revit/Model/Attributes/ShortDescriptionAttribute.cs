namespace BIMdance.Revit.Model.Attributes;

public class ShortDescriptionAttribute : DescriptionAttribute
{
    public ShortDescriptionAttribute(string description) : base(description) { }

    public ShortDescriptionAttribute(Type resourceManagerProvider, string key) :
        base(GetValue(resourceManagerProvider, key))
    { }

    private static string GetValue(Type resourceManagerProvider, string key) =>
        new ResourceManager(resourceManagerProvider).GetString(key);
}