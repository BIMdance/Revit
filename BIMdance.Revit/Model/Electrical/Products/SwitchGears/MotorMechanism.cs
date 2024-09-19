namespace BIMdance.Revit.Model.Electrical.Products.SwitchGears;

public class MotorMechanism : Manufactured
{
    public MotorMechanism(Guid guid) : base(guid) { }
    public ElectricalQuantity Voltage { get; set; }
    public ElectricalQuantity Power { get; set; }
}