namespace BIMdance.Revit.Model.Products;

public class Product : BaseProduct, IPrototype<Product>, IPropertyPrototype<Product>
{
    public Guid Guid { get; }
    public int Count { get; set; } = 1;
    public string Description { get; set; }
    public string LongDescription { get; set; }
    public Bitmap Image { get; set; }
    public string Manufacturer { get; set; }
    public string Name { get; set; }
    public string ProductRange { get; set; }
    public string ProductType { get; set; }
    public string ShortName { get; set; }
    public ProductAdvantages[] ProductAdvantages { get; set; }
    public Dimensions Dimensions { get; set; }
    public double Weight { get; set; }
    public string ProductUri { get; set; }

    public Product() { }
    public Product(Guid guid, string reference = null) : base(reference) => Guid = guid;
    public Product(string reference) : base(reference) { }
    public Product(BaseProduct baseProduct) : base(baseProduct.Reference)
    {
        Characteristics = baseProduct.Characteristics;
        
        if (baseProduct is Product product)
            PullProperties(product);
    }

    private Product(Product prototype) : this(prototype.Reference) => PullProperties(prototype);
    public void PullProperties(Product prototype)
    {
        this.Characteristics = prototype.Characteristics;
        this.Count = prototype.Count;
        this.Description = prototype.Description;
        this.LongDescription = prototype.LongDescription;
        this.Dimensions = prototype.Dimensions;
        this.Image = prototype.Image;
        this.Manufacturer = prototype.Manufacturer;
        this.Name = prototype.Name;
        this.ProductType = prototype.ProductType;
        this.ProductRange = prototype.ProductRange;
        this.ShortName = prototype.ShortName;
        this.Weight = prototype.Weight;
    }
    public Product Clone() => new(prototype: this);
}