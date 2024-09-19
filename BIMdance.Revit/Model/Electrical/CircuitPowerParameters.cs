namespace BIMdance.Revit.Model.Electrical;

public class CircuitPowerParameters
{
    public const double DefaultReducingFactor = 0.65;
        
    public CircuitPowerParameters(ElectricalSystemProxy electricalSystem)
    {
        ElectricalSystem = electricalSystem;
    }

    public ElectricalSystemProxy ElectricalSystem { get; }
    public double CablePermissibleCurrent { get; set; }
    public double EstimateCablePermissibleCurrent { get; set; }
    public bool LockCalculateCircuit { get; set; }
    public double ReducingFactor { get; private set; } = DefaultReducingFactor;

    public void SetReducingFactor(double reducingFactor)
    {
        ReducingFactor = reducingFactor;
        UpdatePermissibleCurrent();
    }

    public void UpdatePermissibleCurrent()
    {
        var cable = ElectricalSystem.CircuitProducts.Cable;

        if (cable == null)
        {
            CablePermissibleCurrent = default;
            EstimateCablePermissibleCurrent = default;
            return;
        }

        CablePermissibleCurrent = ElectricalSystem.Cabling.CablingEnvironment switch
        {
            CablingEnvironment.Air => cable.PermissibleCurrent,
            CablingEnvironment.Ground => cable.GroundPermissibleCurrent,
            _ => throw new ArgumentOutOfRangeException(nameof(ElectricalSystem.Cabling.CablingEnvironment), ElectricalSystem.Cabling.CablingEnvironment, null),
        };
            
        EstimateCablePermissibleCurrent =
            ElectricalSystem.CircuitProducts.EstimateCablesCount
            * ReducingFactor
            * CablePermissibleCurrent;
    }
}