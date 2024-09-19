namespace BIMdance.Revit.Model.Products;

public class SelectorProduct : IPrototype<SelectorProduct>
{
    public string Designation { get; set; }
    public Product Product { get; set; }
    public List<Product> Accessories { get; set; }
    public List<string> References { get; set; }
    public string SerializedConfiguration { get; set; }
    // public SortedDictionary<string, ProductCharacteristic> Characteristics { get; }
    public string GlobalId { get; set; }
    public string SelectorId { get; set; }
    public string MountingModeId { get; set; }
    // public List<string> References { get; set; }
    // public Product Product { get; set; }
    // public List<Product> Accessories { get; set; }
    public int SelectorGroup { get; set; }
    public string AccessoriesNames
    {
        get
        {
            if (Accessories == null || Accessories.IsEmpty())
            {
                return "";
            }

            // var accessoriesWithShortName = Accessories
            //     .Where(a => !string.IsNullOrWhiteSpace(a.ShortName))
            //     .ToList();
            //
            // if (accessoriesWithShortName.Any())
            // {
            //     var shortNames = accessoriesWithShortName
            //         .Select(a => a.ShortName)
            //         .Join(", ");
            //
            //     if (accessoriesWithShortName.Count() == Accessories.Count())
            //     {
            //         return shortNames;
            //     }
            //
            //     return $"{shortNames}, ...";
            // }

            var accessoriesCount = Accessories.Select(a => a.Count).Sum();

            var declension = DeclensionUtils.GetDeclension
            (
                accessoriesCount,
                ModelLocalization.DeviceNominativ,
                ModelLocalization.DeviceGenetiv,
                ModelLocalization.DevicePlural
            );

            return $"{accessoriesCount} {declension}";
        }
    }

    public SelectorProduct()
    {
        // Characteristics = new SortedDictionary<string, ProductCharacteristic>();
        References = new List<string>();
        Accessories = new List<Product>();
        SelectorGroup = -1;
    }
        
    public SelectorProduct(string serializedConfiguration) : this()
    {
        SerializedConfiguration = serializedConfiguration;
    }

    private SelectorProduct(SelectorProduct product) : this(product.SerializedConfiguration)
    {
        GlobalId = product.GlobalId;
        // ShortName = product.ShortName;
        SelectorId = product.SelectorId;
        SelectorGroup = product.SelectorGroup;
        MountingModeId = product.MountingModeId;
        Product = product.Product.Clone();

        // foreach (var parameter in product.Characteristics)
        //     Characteristics.Add(parameter.Key, parameter.Value);

        foreach (var reference in product.References)
            References.Add(reference);

        foreach (var accessory in product.Accessories)
            Accessories.Add(accessory.Clone());
    }

    public SelectorProduct Clone()
    {
        return new SelectorProduct(this);
    }
}