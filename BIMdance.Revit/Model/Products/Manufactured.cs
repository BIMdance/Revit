namespace BIMdance.Revit.Model.Products;

public abstract class Manufactured : IManufactured
{
    protected Manufactured(Guid guid, string reference = null) => Product = new Product(guid, reference);
    protected Manufactured(string reference = null) => Product = new Product(Guid.NewGuid(), reference);

    public Product Product { get; protected set; }
    public bool IsProductOf(Guid productGuid) => Product.Guid == productGuid;
    public bool IsSameProduct(IManufactured manufactured) => Product.Guid == manufactured?.Product.Guid;

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (ReferenceEquals(null, obj)) return false;
        return obj.GetType() == this.GetType() && Equals((Manufactured) obj);
    }
    public override string ToString() => Product.Name;
    protected bool Equals(Manufactured other) => Product.Guid.Equals(other.Product.Guid);
    public override int GetHashCode() => Product.Guid.GetHashCode();
}