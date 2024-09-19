namespace BIMdance.Revit.Model.Electrical.Base;

public abstract class EquipmentSet : ElementProxy
{
    protected EquipmentSet(int revitId, string name) :
        base(revitId, name) { }

    public abstract int UnitCount { get; }
    public abstract IEnumerable<EquipmentUnit> Units { get; }
}

public abstract class EquipmentSet<TEquipmentUnit> : EquipmentSet
    where TEquipmentUnit : EquipmentUnit
{
    protected EquipmentSet(
        int revitId, string name,
        params TEquipmentUnit[] sections) :
        this(revitId, name) 
    {
        foreach (var section in sections)
            AddUnit(section);
    }

    protected EquipmentSet(int revitId, string name) :
        base(revitId, name)
    {
        // SelectAndConfigSetting = SelectAndConfigSetting<ElectricalPanel>.Create();
    }

    public override int UnitCount => SpecificUnits.Count;
    public override IEnumerable<EquipmentUnit> Units => SpecificUnits;
    public List<TEquipmentUnit> SpecificUnits { get; } = new();
    public BuiltInCategoryProxy Category { get; set; } = BuiltInCategoryProxy.OST_ElectricalEquipment;
    public ElectricalProducts Products { get; set; } = new();
    public DistributionSystemProxy DistributionSystem => FirstUnit.DistributionSystem;
    public TEquipmentUnit FirstUnit => SpecificUnits.First();

    public TEquipmentUnit AddUnit(TEquipmentUnit unit)
    {
        var previousUnit = SpecificUnits.LastOrDefault();
        SpecificUnits.Add(unit);
        unit.EquipmentSet = this;
        previousUnit?.RightConnector?.ConnectTo(unit.LeftConnector);
        return unit;
    }
    
    public TEquipmentUnit InsertUnit(int index, TEquipmentUnit unit)
    {
        if (index < 0) index = 0;
        if (index > SpecificUnits.Count) index = SpecificUnits.Count;
        
        var previousUnit = index > 0 ? SpecificUnits[index - 1] : null;
        var nextUnit = index < SpecificUnits.Count ? SpecificUnits[index] : null;
            
        SpecificUnits.Insert(index, unit);
        unit.EquipmentSet = this;
            
        previousUnit?.RightConnector?.ConnectTo(unit.LeftConnector);
        nextUnit?.LeftConnector?.ConnectTo(unit.RightConnector);
        return unit;
    }

    public bool IsSingle => SpecificUnits.Count == 1;
}