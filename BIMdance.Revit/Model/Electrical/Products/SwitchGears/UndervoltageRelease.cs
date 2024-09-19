namespace BIMdance.Revit.Model.Electrical.Products.SwitchGears;

public class UndervoltageRelease : Manufactured
{
    public UndervoltageRelease(Guid guid) : base(guid) { }
    public ElectricalQuantity Voltage { get; set; }
    public ElectricalQuantity Power { get; set; }
    public ElectricalQuantity LatchedPower { get; set; }
    public Range<double> DelayTime { get; set; }
    public Range<double> ThresholdOpening { get; set; }
    public Range<double> ThresholdClosing { get; set; }
}