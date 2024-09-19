namespace BIMdance.Revit.Model.Electrical.Factories;

public class OperatingModeFactory : ElectricalFactoryBase<OperatingMode>
{
    public OperatingModeFactory(ElectricalContext electricalContext) : base(electricalContext, electricalContext.OperatingModes) { }
    
    public OperatingMode Create(string name) =>
        Create(() => new OperatingMode(NewId(), name));
}