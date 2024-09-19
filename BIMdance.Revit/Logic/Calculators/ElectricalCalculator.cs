namespace BIMdance.Revit.Logic.Calculators;

public class ElectricalCalculator
{
    private const double Sqrt3 = 1.7320508;

    public ElectricalCalculator(ElectricalContext electricalContext)
    {
        ElectricalContext = electricalContext;
        Currents = new CurrentCalculators();
        Loads = new LoadCalculators(this, electricalContext);
        Parameters = new ParameterCalculators(this, electricalContext);
    }
    
    public ElectricalContext ElectricalContext { get; }
    public CurrentCalculators Currents { get; }
    public LoadCalculators Loads { get; }
    public ParameterCalculators Parameters { get; }

    public void CalculateAll(OperatingMode operatingMode)
    {
        Loads.CalculateAll(operatingMode);
        Parameters.CalculateAllResistances(operatingMode);
    }

    public void CalculateAllOperatingModes()
    {
        Loads.CalculateAllOperatingModes();
        Parameters.CalculateAllResistancesAllOperatingModes();
    }
    
    public static double Efficiency(double mechanicalPower, double electricalPower) =>
        (mechanicalPower / electricalPower).Round(3, 3);

    public static double MechanicalPower(double electricalPower, double electricalEfficiency) =>
        (electricalPower * electricalEfficiency).Round(4, 4);

    public static double ActivePower(double mechanicalPower, double electricalEfficiency)
    {
        if (mechanicalPower.IsEqualToZero() || electricalEfficiency.IsEqualToZero())
            return default;

        return (mechanicalPower / electricalEfficiency).Round(4, 4);
    }

    public static double ApparentPower(double activePower, double powerFactor)
    {
        if (activePower.IsEqualToZero() || powerFactor.IsEqualToZero())
            return default;

        return (activePower / powerFactor).Round(4, 4);
    }

    public static double PowerFactor(double apparentPower, double activePower)
    {
        if (activePower.IsEqualToZero() || apparentPower.IsEqualToZero())
            return default;

        return (activePower / apparentPower).Round(3, 3);
    }

    public static double Current(double apparentPower, double voltage, int phasesNumber)
    {
        if (voltage.IsEqualToZero())
            return default;
        
        return apparentPower / voltage / (phasesNumber < 2 ? 1: 3);
    }

    public static double LineToGroundVoltage(double lineToLineVoltage) => (lineToLineVoltage / Sqrt3).Round(significantDigits: 2);
    public static double LineToLineVoltage(double lineToGroundVoltage) => (lineToGroundVoltage * Sqrt3).Round(significantDigits: 2);
}