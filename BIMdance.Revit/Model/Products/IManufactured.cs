namespace BIMdance.Revit.Model.Products;

public interface IManufactured
{
    Product Product { get; }
    bool IsProductOf(Guid productGuid);
    bool IsSameProduct(IManufactured manufactured);
}