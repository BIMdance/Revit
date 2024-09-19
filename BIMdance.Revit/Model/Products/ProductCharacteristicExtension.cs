namespace BIMdance.Revit.Model.Products;

public static class ProductCharacteristicExtension
{
    public static T GetValueAs<T>(this ProductCharacteristic productCharacteristic)
        where T : class
    {
        return CharacteristicConverter.GetValueAs<T>(productCharacteristic.Value);
    }

    public static object GetValueAs(this ProductCharacteristic productCharacteristic, Type characteristicType)
    {
        return CharacteristicConverter.GetValue(productCharacteristic.Value, characteristicType);
    }
}