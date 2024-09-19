namespace BIMdance.Revit.Logic.Calculators;

public class CurrentCalculators
{
    // public void CalculateOnePhaseEstimate(ElectricalBase electrical, OperatingMode operatingMode) =>
    //     LoadCalculatorHelper.CalculateOnePhaseEstimateCurrent(new CalculationUnit(electrical, operatingMode));
    //
    // public void CalculateEstimate(ElectricalBase electrical, OperatingMode operatingMode, Phase phase) =>
    //     LoadCalculatorHelper.CalculateEstimateCurrent(new CalculationUnit(electrical, operatingMode), phase);
    //
    // public void CalculateEstimate(ElectricalBase electrical, OperatingMode operatingMode) =>
    //     LoadCalculatorHelper.CalculateEstimateCurrents(new CalculationUnit(electrical, operatingMode));

    internal CurrentCalculators() { }
    
    public double GetAsymmetry(ElectricalBase electrical, OperatingMode operatingMode)
    {
        var threePhases = electrical.GetEstimatedPowerParameters(operatingMode).ThreePhases; 
        var currents = new List<double>
        {
            threePhases.EstimateCurrent1,
            threePhases.EstimateCurrent2,
            threePhases.EstimateCurrent3
        };

        if (electrical.PowerParameters.IsTwoPhases)
            currents = currents.Where(n => n > 1e-3).ToList();

        return currents.Any()
            ? (1 - currents.Min() / currents.Max()).Round(3, 3)
            : 0;
    }

    public double GetMaxEstimateCurrent(ThreePhasesParameters threePhases) =>
        new [] {threePhases.EstimateCurrent1, threePhases.EstimateCurrent2, threePhases.EstimateCurrent3}.Max();

    public double GetEstimateCurrent(ElectricalBase electrical, OperatingMode operatingMode, Phase phase)
    {
        var calculationUnit = new CalculationUnit(electrical, operatingMode);
        var (apparentLoad, trueLoad, estimateTrueLoad) = calculationUnit.GetLoads(phase);
        var demandFactor = trueLoad > 0 ? estimateTrueLoad / trueLoad : 1;
        var estimateApparentLoad = demandFactor * apparentLoad;
        var voltage = calculationUnit.Voltage;
        var estimateCurrent = voltage > 0 ? estimateApparentLoad / voltage : 0;

        return estimateCurrent;
    }
}