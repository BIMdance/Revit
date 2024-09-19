namespace BIMdance.Revit.Model.Electrical.Products.SwitchGears;

public class MeasuringVoltageTransformer : Manufactured
{
    public MeasuringVoltageTransformer(Guid guid) : base(guid) { }
    public MeasuringAccuracyClass MeasuringAccuracyClass { get; set; }
}