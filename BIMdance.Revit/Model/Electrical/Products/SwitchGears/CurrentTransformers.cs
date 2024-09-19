namespace BIMdance.Revit.Model.Electrical.Products.SwitchGears;

public class CurrentTransformer : Manufactured
{
    public CurrentTransformer(Guid guid) : base(guid) { }
    public MeasuringAccuracyClass MeasuringAccuracyClass { get; set; }
    public ProtectionAccuracyClass ProtectionAccuracyClass { get; set; }
    public double PrimaryCurrent { get; set; }
}