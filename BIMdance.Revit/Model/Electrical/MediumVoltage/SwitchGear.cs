namespace BIMdance.Revit.Model.Electrical.MediumVoltage;

public class SwitchGear : EquipmentSet<SwitchGearUnit>
{
    public SwitchGear() : base(0, null) { }
    
    public SwitchGear(int revitId, string name) :
        base(revitId, name)
    {
        Products = SwitchBoardUnitProducts = new SwitchBoardUnitProducts();
    }

    public SwitchGear(
        int revitId, string name,
        ElectricalSystemTypeProxy systemType = ElectricalSystemTypeProxy.UndefinedSystemType) :
        base(revitId, name)
    {
        var firstSection = new SwitchGearUnit(revitId, name, systemType);
        AddUnit(firstSection);
        Products = SwitchBoardUnitProducts = new SwitchBoardUnitProducts();
    }

    public SwitchGear(
        int revitId, string name,
        params SwitchGearUnit[] sections) :
        base(revitId, name, sections)
    {
        Products = SwitchBoardUnitProducts = new SwitchBoardUnitProducts();
    }

    public SwitchGear(
        int revitId, string name,
        DistributionSystemProxy distributionSystem) :
        base(revitId, name)
    {
        var firstSection = distributionSystem != null
            ? new SwitchGearUnit(revitId, name, distributionSystem)
            : new SwitchGearUnit(revitId, name, ElectricalSystemTypeProxy.PowerCircuit);
        
        AddUnit(firstSection);
        Products = SwitchBoardUnitProducts = new SwitchBoardUnitProducts();
    }
    
    public SwitchBoardUnitProducts SwitchBoardUnitProducts { get; }
    
    public static SwitchGear CreateSingle(
        int revitId, string name, ElectricalSystemTypeProxy systemType)
    {
        var switchboardUnit = new SwitchGearUnit(revitId, name, systemType);
        return new SwitchGear(revitId, name, switchboardUnit);
    }
    
    public static SwitchGear CreateSingle(
        int revitId, string name, DistributionSystemProxy distributionSystem)
    {
        var switchboardUnit = new SwitchGearUnit(revitId, name, distributionSystem);
        return new SwitchGear(revitId, name, switchboardUnit);
    }
}