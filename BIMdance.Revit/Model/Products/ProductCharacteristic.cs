namespace BIMdance.Revit.Model.Products;

public record ProductCharacteristic(string Key, string Value)
{
    public string Key { get; set; } = Key;
    public string Value { get; set; } = Value;
}