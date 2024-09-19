namespace BIMdance.Revit.Model.Electrical.Products.SwitchGears;

public class SwitchGearComponents : IPrototype<SwitchGearComponents>
{
    public FaultPassageIndicator FaultPassageIndicator { get; set; }
    public List<AuxiliaryContact> AuxiliaryContacts { get; set; } = new();
    public List<Fuse> Fuses { get; set; } = new();
    public ProtectionRelay ProtectionRelay { get; set; }
    public SwitchGearSwitch Switch { get; set; }
    public SwitchController SwitchController { get; set; }
    public MotorMechanism MotorMechanism { get; set; }
    public ShuntTripRelease ShuntTripRelease { get; set; }
    public UndervoltageRelease UndervoltageRelease { get; set; }

    public void SetFuse(Fuse fuse)
    {
        Fuses.Clear();
        
        for (var i = 0; i < 3; i++)
            Fuses.Add(fuse);
    }
    
    public SwitchGearComponents Clone() => new()
    {
        AuxiliaryContacts = new List<AuxiliaryContact>(this.AuxiliaryContacts),
        FaultPassageIndicator = this.FaultPassageIndicator,
        Fuses = new List<Fuse>(this.Fuses),
        ProtectionRelay = this.ProtectionRelay?.Clone(),
        MotorMechanism = this.MotorMechanism,
        ShuntTripRelease = this.ShuntTripRelease,
        Switch = this.Switch,
        SwitchController = this.SwitchController?.Clone(),
        UndervoltageRelease = this.UndervoltageRelease,
    };
}