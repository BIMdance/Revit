namespace BIMdance.Revit.Model.Electrical.Products.SwitchGears;

public class ShuntTripRelease : Manufactured
{
    public ShuntTripRelease(Guid guid) : base(guid) { }
    public ElectricalQuantity Voltage { get; set; }
    public ElectricalQuantity Power { get; set; }
    public double ResponseTime { get; set; }
}