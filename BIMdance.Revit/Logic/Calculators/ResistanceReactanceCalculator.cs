using BIMdance.Revit.Logic.Locks;

namespace BIMdance.Revit.Logic.Calculators;

internal class ResistanceReactanceCalculator
{
    #region Fields
        
    private readonly ElectricalCalculator _electricalCalculator;
    private readonly ElectricalContext _electricalContext;
    
    private static double _designTemperature;
    private static double _temperatureFactor;

    #endregion

    internal ResistanceReactanceCalculator(ElectricalCalculator electricalCalculator, ElectricalContext electricalContext)
    {
        _electricalCalculator = electricalCalculator;
        _electricalContext = electricalContext;
    }

    internal void CalculateAllResistancesAllOperatingModes()
    {
        foreach (var operatingMode in _electricalContext.OperatingModes)
            CalculateAll(operatingMode);
    }
        
    internal void CalculateAll(OperatingMode operatingMode)
    {
        if (LockService.IsLocked(Lock.Calculate))
            return;

        foreach (var source in _electricalContext.ElectricalSources)
            Calculate(source, operatingMode);
    }

    internal void CalculateAllOperatingModes(ElectricalBase electrical)
    {
        foreach (var operatingMode in _electricalContext.OperatingModes)
            Calculate(new CalculationUnit(electrical, operatingMode));
    }

    internal void Calculate(ElectricalBase electrical, OperatingMode operatingMode)
    {
        IEnumerable<ElectricalEquipmentProxy> electricalItems;
        
        switch (electrical)
        {
            case ElectricalSystemProxy electricalSystem:
                Calculate(new CalculationUnit(electrical, operatingMode));
                electricalItems = StructureUtils.GetOrderedConnectedEquipments(electricalSystem.GetConsumersOf<ElectricalEquipmentProxy>(), operatingMode);
                break;
          
            case ElectricalEquipmentProxy equipment:
                electricalItems = StructureUtils.GetOrderedConnectedEquipments(equipment, operatingMode);
                break;
            
            default:
                electricalItems = Array.Empty<ElectricalEquipmentProxy>();
                break;
        }

        foreach (var electricalItem in electricalItems)
            Calculate(new CalculationUnit(electricalItem, operatingMode));
    }
        
    internal void Calculate(CalculationUnit calculationUnit)
    {
        var resistanceReactance = calculationUnit.EstimatedPowerParameters.ResistanceReactance;
        
        if (!calculationUnit.Electrical.IsPower ||
            LockService.IsLocked(Lock.Calculate))
            return;

        if (calculationUnit.Electrical is ElectricalSystemProxy electricalCircuit)
            AddCableResistance(resistanceReactance, electricalCircuit);

        CalculateResistance(calculationUnit);
    }

    private void AddCableResistance(ResistanceReactance resistanceReactance, ElectricalSystemProxy electricalCircuit)
    {
        var cable = electricalCircuit.CircuitProducts.Cable;

        if (cable == null)
            return;
            
        var cableLength = electricalCircuit.CableLengthMax > 0
            ? electricalCircuit.CableLengthMax
            : electricalCircuit.CableLength;
        var cablesCount = electricalCircuit.CircuitProducts.EstimateCablesCount;

        resistanceReactance.InternalR0 = cable.R0 * cableLength / cablesCount; // / 1_000;
        resistanceReactance.InternalR1 = cable.R1 * cableLength / cablesCount; // / 1_000;
        resistanceReactance.InternalX0 = cable.X0 * cableLength / cablesCount; // / 1_000;
        resistanceReactance.InternalX1 = cable.X1 * cableLength / cablesCount; // / 1_000;
    }

    private void CalculateResistance(CalculationUnit calculationUnit)
    {
        var resistanceReactance = calculationUnit.EstimatedPowerParameters.ResistanceReactance;
        var sourceResistanceReactance = calculationUnit.Electrical.BaseSource?.GetEstimatedPowerParameters(calculationUnit.OperatingMode).ResistanceReactance;

        resistanceReactance.TotalR0 = TemperatureFactor * resistanceReactance.InternalR0 + (sourceResistanceReactance?.TotalR0 ?? 0);
        resistanceReactance.TotalR1 = TemperatureFactor * resistanceReactance.InternalR1 + (sourceResistanceReactance?.TotalR1 ?? 0);
        resistanceReactance.TotalX0 = TemperatureFactor * resistanceReactance.InternalX0 + (sourceResistanceReactance?.TotalX0 ?? 0);
        resistanceReactance.TotalX1 = TemperatureFactor * resistanceReactance.InternalX1 + (sourceResistanceReactance?.TotalX1 ?? 0);
            
        _electricalCalculator.Parameters.CalculateShortCurrent(calculationUnit);
        _electricalCalculator.Parameters.CalculateVoltageDrop(calculationUnit);
        
        CalculateConsumers(calculationUnit);
    }

    private void CalculateConsumers(CalculationUnit calculationUnit)
    {
        // var electricalItems = StructureUtils.GetOrderedConnectedElectricalItems(calculationUnit.Electrical);
        var electricalItems = calculationUnit.Electrical.GetConsumersOf<ElectricalSystemProxy>();

        foreach (var electricalItem in electricalItems)
            Calculate(new CalculationUnit(electricalItem, calculationUnit.OperatingMode));
    }

    private double TemperatureFactor => !_temperatureFactor.Equals(0)
        ? _temperatureFactor
        : _temperatureFactor = 1;

    public double DesignTemperature
    {
        get => !_designTemperature.Equals(0)
            ? _designTemperature
            : _designTemperature = 30; //BimEdContexts.Electrical.ElectricalSetting.DesignTemperature);
        set
        {
            if (_designTemperature.Equals(value)) return;
            _designTemperature = value;
            _temperatureFactor = 1 + 0.004 * (_designTemperature - 20);
        }
    }
}