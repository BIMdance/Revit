namespace BIMdance.Revit.Model.Electrical;

/// <summary>
/// Combinations of <see cref="SwitchBoardUnit"/> - low voltage panels.<br/>
/// Any <see cref="SwitchBoardUnit"/> is a part of <see cref="SwitchBoard"/>.<br/>
/// Any <see cref="SwitchBoard"/> must have one or more <see cref="SwitchBoardUnit"/>.<br/>
/// Less then 1000 V.
/// </summary>
public class SwitchBoard : EquipmentSet<SwitchBoardUnit>
{
    public SwitchBoard() : base(NotCreatedInRevitId, null) =>
        Products = SwitchBoardUnitProducts = new SwitchBoardUnitProducts();

    public SwitchBoard(int revitId, string name) : base(revitId, name) =>
        Products = SwitchBoardUnitProducts = new SwitchBoardUnitProducts();

    private SwitchBoard(
        int revitId, string name,
        params SwitchBoardUnit[] sections) :
        base(revitId, name, sections) =>
        Products = SwitchBoardUnitProducts = new SwitchBoardUnitProducts();

    public SwitchBoardUnitProducts SwitchBoardUnitProducts { get; }
}

public class PanelInput
{
    public PanelInput(ElectricalBase electrical)
    {
        Electrical = electrical;
    }

    public ElectricalBase Electrical { get; set; }
}

public class CabinetSettings : IPropertyPrototype<CabinetSettings>
{
    // public CabinetType CabinetType { get; set; }
    public double BusCurrent { get; set; }
    // public EnclosureType EnclosureType { get; set; }
    public Material Material { get; set; }
    // public MountingType MountingType { get; set; }
    public bool AutomaticTransferSwitch { get; set; }

    public void PullProperties(CabinetSettings prototype)
    {
        // this.CabinetType = prototype.CabinetType;
        BusCurrent = prototype.BusCurrent;
        // this.EnclosureType = prototype.EnclosureType;
        Material = prototype.Material;
        // this.MountingType = prototype.MountingType;
        AutomaticTransferSwitch = prototype.AutomaticTransferSwitch;
    }
}