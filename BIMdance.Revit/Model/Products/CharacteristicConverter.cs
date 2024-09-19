namespace BIMdance.Revit.Model.Products;

internal static class CharacteristicConverter
{
    internal static T GetValueAs<T>(string inputValue)
        where T : class
    {
        return GetValue(inputValue, typeof(T)) as T;
    }

    internal static object GetValue(string inputValue, Type characteristicType)
    {
        if (string.IsNullOrWhiteSpace(inputValue))
            return default;

        if (characteristicType == typeof(bool))
        {
            return inputValue.ToUpper() switch
            {
                "1" => true,
                "TRUE" => true,
                "YES" => true,
                 
                "" => false,
                "0" => false,
                "FALSE" => false,
                "NO" => false,
                _ => false
            };
        }

        if (characteristicType == typeof(string))
        {
            return inputValue;
        }

        if (characteristicType == typeof(double))
        {
            GetDoubleValueAndUnits(inputValue, out var value, out var units);
                
            return units switch
            {
                "KA" => value * 1000,
                "MA" => value / 1000,
                _ => value
            };
        }

        else
        {
            throw new ArgumentOutOfRangeException(nameof(characteristicType), characteristicType, null);
        }
    }

    private static void GetDoubleValueAndUnits(string valueAsString, out double value, out string units)
    {
        value = default;
        var split = valueAsString.Split('_', ' ');
        var index = 0;

        foreach (var s in split)
        {
            if (double.TryParse(s.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out value))
            {
                split = split.Skip(index).ToArray();
                break;
            }

            index++;
        }

        units = split.Length > index + 1 ? split[index + 1].ToUpper() : null;
    }
}