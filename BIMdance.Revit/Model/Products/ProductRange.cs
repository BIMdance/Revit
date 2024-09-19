namespace BIMdance.Revit.Model.Products;

public class ProductRange : IPrototype<ProductRange>
{
    public ProductRange()
    {
        CharacteristicValues = new Dictionary<string, List<string>>();
        SelectedValues = new Dictionary<string, string>();
        Products = new List<BaseProduct>();
    }

    public ProductRange(List<BaseProduct> products) : this()
    {
        Products = products;
    }

    private ProductRange(ProductRange prototype)
    {
        CharacteristicValues = prototype.CharacteristicValues;
        SelectedValues = new Dictionary<string, string>(prototype.SelectedValues);
        Products = prototype.Products;
    }

    public ProductRange Clone()
    {
        return new ProductRange(this);
    }

    public void PullSelectedValues(ProductRange prototype)
    {
        SelectedValues = new Dictionary<string, string>(prototype.SelectedValues);
    }

    public List<BaseProduct> Products { get; set; }
    public Dictionary<string, List<string>> CharacteristicValues { get; set; }
    public Dictionary<string, string> SelectedValues { get; set; }

    private double? GetCharacteristicDoubleValue(BaseProduct product, List<string> characteristics)
    {
        foreach(var characteristic in characteristics)
        {
            var value = product.GetCharacteristic(characteristic)?.Value?.FirstDoubleOrNull();

            if(value != null)
            {
                return value;
            }
        }

        return null;
    }

    public IEnumerable<BaseProduct> Filter(Dictionary<string, string> filterCharacteristics, List<string> characteristics, double minValue, double maxValue)
    {
        return Products.Where(p =>
        {
            var value = GetCharacteristicDoubleValue(p, characteristics);

            if (value != null && (value < minValue || maxValue < value))
            {
                return false;
            }

            if (filterCharacteristics == null)
            {
                return true;
            }

            foreach (var filterCharacteristic in filterCharacteristics)
            {
                if (p.Characteristics.TryGetValue(filterCharacteristic.Key, out var resultCharacteristic) &&
                    resultCharacteristic.Value != filterCharacteristic.Value)
                {
                    return false;
                }
            }

            return true;
        });
    }

    public IEnumerable<BaseProduct> Filter(Dictionary<string, string> filterCharacteristics, string characteristic, double minValue, double maxValue)
    {
        return Products.Where(p =>
        {
            var value = p.GetCharacteristic(characteristic)?.Value?.FirstDoubleOrNull();

            if (value != null && (value < minValue || maxValue < value))
            {
                return false;
            }

            if (filterCharacteristics == null)
            {
                return true;
            }

            foreach (var filterCharacteristic in filterCharacteristics)
            {
                if (p.Characteristics.TryGetValue(filterCharacteristic.Key, out var resultCharacteristic) &&
                    resultCharacteristic.Value != filterCharacteristic.Value)
                {
                    return false;
                }
            }

            return true;
        });
    }

    public IEnumerable<BaseProduct> Filter(Dictionary<string, string> filterCharacteristics)
    {
        return Products.Where(n =>
        {
            foreach (var filterCharacteristic in filterCharacteristics)
            {
                if (n.Characteristics.TryGetValue(filterCharacteristic.Key, out var characteristic) &&
                    characteristic.Value != filterCharacteristic.Value)
                {
                    return false;
                }
            }

            return true;
        });
    }

    public IEnumerable<BaseProduct> Filter(params (string Id, string Value)[] parameters)
    {
        return Products.Where(n =>
        {
            foreach (var (id, filterValue) in parameters)
            {
                if (n.Characteristics.TryGetValue(id, out var characteristic) &&
                    characteristic.Value != filterValue)
                {
                    return false;
                }
            }

            return true;
        });
    }
}