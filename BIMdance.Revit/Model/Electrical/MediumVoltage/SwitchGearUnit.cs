

// ReSharper disable InconsistentNaming

namespace BIMdance.Revit.Model.Electrical.MediumVoltage;

public sealed class SwitchGearUnit : EquipmentUnit, IPrototype<SwitchGearUnit>
{
    public SwitchGearUnit() :
        this(-1, null) { }

    private SwitchGearUnit(int revitId, string name) :
        base(revitId, name) { }

    public SwitchGearUnit(
        int revitId, string name,
        ElectricalSystemTypeProxy systemType,
        params SwitchGearFunction[] switchGearFunctions) :
        base(revitId, name, systemType)
    {
        foreach (var switchGearFunction in switchGearFunctions) Add(switchGearFunction);
    }

    public SwitchGearUnit(
        int revitId, string name,
        DistributionSystemProxy distributionSystem,
        params SwitchGearFunction[] switchGearFunctions) :
        base(revitId, name, distributionSystem)
    {
        foreach (var switchGearFunction in switchGearFunctions) Add(switchGearFunction);
    }

    public SwitchGearUnit(
        Guid switchGearSeriesGuid, Product product,
        params SwitchGearFunction[] switchGearFunctions) : base(product?.Name)
    {
        Product = product;
        SwitchGearSeriesGuid = switchGearSeriesGuid;
        foreach (var switchGearFunction in switchGearFunctions) Add(switchGearFunction);
    }

    public static SwitchGearUnit CreateNE(
        Guid switchGearSeriesGuid, Product product,
        params SwitchGearFunction[] switchGearFunctions)
    {
        if (switchGearFunctions.First() != null) switchGearFunctions.First().LeftConnector = null;
        if (switchGearFunctions.Last() != null) switchGearFunctions.Last().RightConnector = null;
        
        return new SwitchGearUnit(switchGearSeriesGuid, product, switchGearFunctions)
        {
            LeftConnector = null,
            RightConnector = null,
        };
    }

    public static SwitchGearUnit CreateDE(
        Guid switchGearSeriesGuid, Product product,
        params SwitchGearFunction[] switchGearFunctions)
    {
        return new SwitchGearUnit(switchGearSeriesGuid, product, switchGearFunctions);
    }

    public static SwitchGearUnit CreateLE(
        Guid switchGearSeriesGuid, Product product,
        params SwitchGearFunction[] switchGearFunctions)
    {
        if (switchGearFunctions.Last() != null) switchGearFunctions.Last().RightConnector = null;
        
        return new SwitchGearUnit(switchGearSeriesGuid, product, switchGearFunctions)
        {
            RightConnector = null,
        };
    }

    public static SwitchGearUnit CreateRE(
        Guid switchGearSeriesGuid, Product product,
        params SwitchGearFunction[] switchGearFunctions)
    {
        if (switchGearFunctions.First() != null) switchGearFunctions.First().LeftConnector = null;

        return new SwitchGearUnit(switchGearSeriesGuid, product, switchGearFunctions)
        {
            LeftConnector = null,
        };
    }
    
    public Guid SwitchGearSeriesGuid { get; }
    public SwitchGear SwitchGear => EquipmentSet as SwitchGear;
    public List<SwitchGearFunction> SwitchGearFunctions { get; } = new();
    public Product Product { get; } = new();

    public void Add(SwitchGearFunction function)
    {
        var previousSwitchGearFunction = SwitchGearFunctions.LastOrDefault();
        SwitchGearFunctions.Add(function);
        
        if (function == null)
            return;
        
        function.SwitchGearUnit = this;
        previousSwitchGearFunction?.RightConnector?.ConnectTo(function.LeftConnector);

        ConnectToLeftSwitchGearUnit(function);
        ConnectToRightSwitchGearUnit(function);
    }

    public void Insert(int index, SwitchGearFunction function)
    {
        var previousUnit = index > 0 ? SwitchGearFunctions[index - 1] : null;
        var nextUnit = index < SwitchGearFunctions.Count - 1 ? SwitchGearFunctions[index] : null;
            
        SwitchGearFunctions.Insert(index, function);
        function.SwitchGearUnit = this;
            
        previousUnit?.RightConnector?.ConnectTo(function.LeftConnector);
        nextUnit?.LeftConnector?.ConnectTo(function.RightConnector);
        
        ConnectToLeftSwitchGearUnit(function);
        ConnectToRightSwitchGearUnit(function);
    }

    public void Replace(int index, SwitchGearFunction function)
    {
        if (index >= SwitchGearFunctions.Count)
        {
            Add(function);
            return;
        }
            
        var oldUnit = SwitchGearFunctions[index];
        var leftReference = oldUnit.LeftConnector.ReferenceConnector as RightConnector<EquipmentUnit>;
        var rightReference = oldUnit.RightConnector.ReferenceConnector as LeftConnector<EquipmentUnit>;
        oldUnit.LeftConnector.Disconnect();
        oldUnit.RightConnector.Disconnect();
            
        SwitchGearFunctions[index] = function;
        function.SwitchGearUnit = this;
        
        leftReference?.ConnectTo(function.LeftConnector);
        rightReference?.ConnectTo(function.RightConnector);
        
        ConnectToLeftSwitchGearUnit(function);
        ConnectToRightSwitchGearUnit(function);
    }

    private void ConnectToRightSwitchGearUnit(SwitchGearFunction function)
    {
        if (SwitchGearFunctions.IsLast(function) &&
            this.RightConnector is { Owner: SwitchGearUnit rightSwitchGearUnit })
        {
            rightSwitchGearUnit.SwitchGearFunctions
                .FirstOrDefault()?.LeftConnector?
                .ConnectTo(function.RightConnector);
        }
    }

    private void ConnectToLeftSwitchGearUnit(SwitchGearFunction function)
    {
        if (SwitchGearFunctions.IsFirst(function) &&
            this.LeftConnector is { Owner: SwitchGearUnit leftSwitchGearUnit })
        {
            leftSwitchGearUnit.SwitchGearFunctions
                .LastOrDefault()?.RightConnector?
                .ConnectTo(function.LeftConnector);
        }
    }

    public SwitchGearUnit Clone()
    {
        var clone = new SwitchGearUnit(
            SwitchGearSeriesGuid, Product,
            SwitchGearFunctions.ToArray());

        clone.LeftConnector = this.LeftConnector != null ? new LeftConnector<EquipmentUnit>(clone) : null;
        clone.RightConnector = this.RightConnector != null ? new RightConnector<EquipmentUnit>(clone) : null;

        return clone;
    }
}