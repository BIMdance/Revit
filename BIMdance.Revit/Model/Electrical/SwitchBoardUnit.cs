namespace BIMdance.Revit.Model.Electrical;

/// <summary>
/// Low Voltage Panel. Part of <see cref="SwitchBoard"/>.<br/>
/// Any <see cref="SwitchBoardUnit"/> is a part of <see cref="SwitchBoard"/>.<br/>
/// Any <see cref="SwitchBoard"/> must have one or more <see cref="SwitchBoardUnit"/>.<br/>
/// Less then 1000 V.
/// </summary>
public sealed class SwitchBoardUnit : EquipmentUnit
{
    public SwitchBoard SwitchBoard => EquipmentSet as SwitchBoard;
    
    public SwitchBoardUnit() { }

    internal SwitchBoardUnit(
        int revitId, string name,
        ElectricalSystemTypeProxy systemType) :
        base(revitId, name, systemType) { }

    internal SwitchBoardUnit(
        int revitId, string name,
        DistributionSystemProxy distributionSystem,
        ConnectorProxy connector = null) :
        base(revitId, name, distributionSystem, connector) { }

    internal SwitchBoardUnit(
        int revitId, string name,
        PhasesNumber phasesNumber, double lineToGroundVoltage) :
        base(revitId, name, phasesNumber, lineToGroundVoltage) { }

    public SwitchBoardUnitProducts SwitchBoardUnitProducts { get; } = new();
    public override ElectricalProducts Products => SwitchBoardUnitProducts;
}