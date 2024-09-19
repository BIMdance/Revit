namespace BIMdance.Revit.Model.Electrical.Products.SwitchGears;

public class Fuse : Manufactured
{
    public Fuse(
        string range, double current, double voltage,
        double length = default, double diameter = default, double weight = default)
    {
        Product = new Product(Guid.NewGuid())
        {
            Name = $"{range} {current} {voltage}",
        }; 
        Current = current; 
        Voltage = voltage;
        Length = length;
        Diameter = diameter;
        Weight = weight;
    }

    public Fuse(
        string range, string reference, double current, double voltage,
        double length = default, double diameter = default, double weight = default) :
        this(range, current, voltage, length, diameter, weight)
    {
        Product.Reference = reference;
    }

    public double Current { get; }
    public double Voltage { get; }
    public double Length { get; }
    public double Diameter { get; }
    public double Weight
    {
        get => Product.Weight;
        set => Product.Weight = value;
    }
}