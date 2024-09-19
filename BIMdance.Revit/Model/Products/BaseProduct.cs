namespace BIMdance.Revit.Model.Products;

public class BaseProduct
{
    public string Reference { get; set; }
    public SortedDictionary<string, ProductCharacteristic> Characteristics { get; protected set; }

    public BaseProduct()
    {
        Characteristics = new SortedDictionary<string, ProductCharacteristic>();
    }

    public BaseProduct(string reference) : this()
    {
        Reference = reference;
    }

    public BaseProduct(string reference, IEnumerable<ProductCharacteristic> characteristics) : this()
    {
        Reference = reference;
        AddCharacteristics(characteristics);
    }

    public void AddCharacteristics(IEnumerable<ProductCharacteristic> characteristics)
    {
        foreach (var characteristic in characteristics)
        {
            AddCharacteristic(characteristic);
        }
    }

    public void AddCharacteristic(string key, string value)
    {
        AddCharacteristic(new ProductCharacteristic(key, value));
    }

    public void AddCharacteristic(ProductCharacteristic characteristic)
    {
        if (Characteristics.ContainsKey(characteristic.Key))
            Characteristics[characteristic.Key] = characteristic;
        else
            Characteristics.Add(characteristic.Key, characteristic);
    }

    public ProductCharacteristic GetCharacteristic(string key)
    {
        return Characteristics.TryGetValue(key, out var characteristic)
            ? characteristic
            : default;
    }

    public string GetValue(string key)
    {
        return GetCharacteristic(key)?.Value;
    }

    public T GetValueAs<T>(string key)
        where T : class
    {
        return GetCharacteristic(key)?.GetValueAs<T>();
    }

    public object GetValueAs(Type characteristicType, string key)
    {
        return GetCharacteristic(key)?.GetValueAs(characteristicType);
    }

    public double? GetValueAsDouble(string key) => GetValue(key)?.FirstDouble();
    public int? GetValueAsInt(string key) => GetValue(key)?.FirstInt();

    protected bool Equals(BaseProduct other)
    {
        return Reference == other.Reference;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType() && Equals((BaseProduct)obj);
    }

    public override int GetHashCode()
    {
        return Reference?.GetHashCode() ?? 0;
    }

    public override string ToString()
    {
        return $"<{GetType().Name}> {Reference}";
    }
}