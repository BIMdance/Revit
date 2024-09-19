namespace BIMdance.Revit.Model.Electrical.MediumVoltage;

public class SwitchGearOptions : IPrototype<SwitchGearOptions>, IPropertyPrototype<SwitchGearOptions>
{
    public List<SwitchGearOption> Common { get; private set; } = new();
    public List<SwitchGearOption> Controls { get; private set; } = new();
    public List<SwitchGearOption> Signalization { get; private set; } = new();
    public List<SwitchGearOption> Telemetry { get; private set; } = new();
    public SwitchGearOptions Clone()
    {
        var clone = new SwitchGearOptions();
        clone.PullProperties(this);
        return clone;
    }
    public void PullProperties(SwitchGearOptions prototype)
    {
        this.Common = prototype.Common.ToList();
        this.Controls = prototype.Controls.ToList();
        this.Signalization = prototype.Signalization.ToList();
        this.Telemetry = prototype.Telemetry.ToList();
    }
}