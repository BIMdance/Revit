namespace BIMdance.Revit.Model.Electrical.Products.SwitchGears;

public interface ICompatibleComponents<T> where T : IManufactured
{
    UniqueCollection<T> CompatibleComponents { get; }
    bool IsCompatible(T component);
    T GetDefault(Type type = null);
}