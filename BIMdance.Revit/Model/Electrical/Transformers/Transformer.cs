namespace BIMdance.Revit.Model.Electrical.Transformers;

public sealed class Transformer :
    ElectricalEquipmentProxy,
    IPrototype<Transformer>,
    IPropertyPrototype<Transformer>
{
    private double _primaryShortCircuitPower;
    private double _primaryBreakingCapacity;
        
    public Transformer() { }

    internal Transformer(
        int revitId, string name = null,
        DistributionSystemProxy primaryDistributionSystem = null,
        DistributionSystemProxy secondaryDistributionSystem = null) :
        base(revitId, name)
    {
        Category = BuiltInCategoryProxy.OST_ElectricalEquipment;
        DistributionSystem = primaryDistributionSystem;
        SecondaryDistributionSystem = secondaryDistributionSystem;
            
        var lineToGroundVoltage = DistributionSystem?.LineToLineVoltage?.ActualValue ?? 0 / MathConstants.Sqrt3;
            
        SetBaseConnector(new ConnectorProxy(this, 1, PhasesNumber.Three, lineToGroundVoltage));
    }

    public void PullProperties(Transformer prototype)
    {
        if (prototype == null)
            return;
            
        this.Name = prototype.Name;
        this.TransformerProduct = prototype.TransformerProduct.Clone();
        this.DistributionSystem = prototype.DistributionSystem;
        this.SecondaryDistributionSystem = prototype.SecondaryDistributionSystem;
        this.NetworkInductiveReactance = prototype.NetworkInductiveReactance;
        this.PrimaryBreakingCapacity = prototype.PrimaryBreakingCapacity;
        this.PrimaryShortCircuitPower = prototype.PrimaryShortCircuitPower;
        this.TransformerReactanceMode = prototype.TransformerReactanceMode;
    }

    public Transformer Clone()
    {
        var clone = (Transformer)this.MemberwiseClone();
        clone.TransformerProduct = this.TransformerProduct.Clone();
        return clone;
    }
    
    public DistributionSystemProxy SecondaryDistributionSystem { get; set; }
    public TransformerProduct TransformerProduct { get; set; } = new();
    public TransformerReactanceMode TransformerReactanceMode { get; set; }
    public double NetworkInductiveReactance { get; set; }

    public double PrimaryShortCircuitPower
    {
        get => _primaryShortCircuitPower;
        set
        {
            _primaryShortCircuitPower = value;

            if (PrimaryShortCircuitPower.IsEqualToZero())
                return;

            PrimaryBreakingCapacity = default;

            CalculateNetworkInductiveReactance();
        }
    }

    public double PrimaryBreakingCapacity
    {
        get => _primaryBreakingCapacity;
        set
        {
            _primaryBreakingCapacity = value;

            if (PrimaryBreakingCapacity.IsEqualToZero())
                return;

            PrimaryShortCircuitPower = default;

            CalculateNetworkInductiveReactance();
        }
    }

    private void CalculateNetworkInductiveReactance()
    {
        var primaryVoltage = DistributionSystem?.LineToLineVoltage?.ActualValue ?? 0;
        var secondaryVoltage = SecondaryDistributionSystem?.LineToLineVoltage?.ActualValue ?? 0;
            
        if (primaryVoltage.IsEqualToZero() || secondaryVoltage.IsEqualToZero())
            return;

        if (PrimaryShortCircuitPower > 0)
        {
            NetworkInductiveReactance = PrimaryShortCircuitPower > 0
                ? Math.Pow(secondaryVoltage, 2) / PrimaryShortCircuitPower
                : 0;
        }

        else if (PrimaryBreakingCapacity > 0)
        {
            NetworkInductiveReactance = PrimaryBreakingCapacity > 0
                ? Math.Pow(secondaryVoltage, 2)
                    / MathConstants.Sqrt3
                    / PrimaryBreakingCapacity
                    / primaryVoltage
                : 0;
        }
    }
}

public enum TransformerReactanceMode
{
    ManualInput,
    ByPrimaryBreakingCapacity,
    ByPrimaryShortCircuitPower
}