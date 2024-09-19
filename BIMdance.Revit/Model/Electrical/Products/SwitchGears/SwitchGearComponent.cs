namespace BIMdance.Revit.Model.Electrical.Products.SwitchGears;

public class SwitchGearComponent : Manufactured
{
    public SwitchGearComponent(Guid guid, string name = null) : base(guid, name) { }
    public UniqueCollection<SwitchGearSeries> SwitchGearSeries { get; set; } = new();
}