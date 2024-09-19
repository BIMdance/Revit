namespace BIMdance.Revit.Logic.Calculators;

public class ParameterCalculators
{
    private readonly ResistanceReactanceCalculator _resistanceReactanceCalculator;

    internal ParameterCalculators(ElectricalCalculator electricalCalculator, ElectricalContext electricalContext) =>
        _resistanceReactanceCalculator = new ResistanceReactanceCalculator(electricalCalculator, electricalContext);

    public void CalculateAllResistancesAllOperatingModes() =>
        _resistanceReactanceCalculator.CalculateAllResistancesAllOperatingModes();
            
    public void CalculateAllResistances(OperatingMode operatingMode) =>
        _resistanceReactanceCalculator.CalculateAll(operatingMode);

    public void CalculateResistanceAllOperatingModes(ElectricalBase electrical) =>
        _resistanceReactanceCalculator.CalculateAllOperatingModes(electrical);

    public void CalculateResistance(ElectricalBase electrical, OperatingMode operatingMode) =>
        _resistanceReactanceCalculator.Calculate(electrical, operatingMode);

    internal void CalculateShortCurrent(CalculationUnit calculationUnit) =>
        new ShortCurrentCalculator(calculationUnit).Calculate();

    internal void CalculateVoltageDrop(CalculationUnit calculationUnit) =>
        new VoltageDropCalculator(calculationUnit).Calculate();

    public void CheckAllVoltageDrop() =>
        VoltageDropCalculator.NewCheckAllVoltageDrop();

    public void SetDesignTemperature(OperatingMode operatingMode, double designTemperature)
    {
        _resistanceReactanceCalculator.DesignTemperature = designTemperature;
        CalculateAllResistances(operatingMode);
    }
}