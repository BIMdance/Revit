using BIMdance.Revit.Model.Electrical.Transformers;

namespace BIMdance.Revit.Logic.Calculators;

internal class ShortCurrentCalculator : CalculationUnit
{
    #region Fields

    private static double _arcResistance;
    private readonly ResistanceReactance _resistanceReactance;

    #endregion
        
    internal ShortCurrentCalculator(CalculationUnit calculationUnit) : base(calculationUnit)
    {
        _resistanceReactance = EstimatedPowerParameters.ResistanceReactance;
    }
        
    internal void Calculate()
    {
        if (!Electrical.IsPower)
            return;
            
        if (_resistanceReactance.TotalR1.Equals(0) &&
            _resistanceReactance.TotalX1.Equals(0))
            return;

        var electricalCircuit = Electrical is ElectricalSystemProxy circuit
            ? circuit
            : Electrical.GetFirstSourceOf<ElectricalSystemProxy>();
            
        var cableCount = electricalCircuit?.CircuitProducts.EstimateCablesCount ?? 0;

        var estimateResistanceReactance = new ResistanceReactance();

        if (cableCount > 1)
        {
            var sourceResistanceReactance = electricalCircuit?.BaseSource?.GetEstimatedPowerParameters(OperatingMode)?.ResistanceReactance;
            
            estimateResistanceReactance.TotalR1 = (sourceResistanceReactance?.TotalR1 ?? 0d) + _resistanceReactance.InternalR1 * cableCount;
            estimateResistanceReactance.TotalR0 = (sourceResistanceReactance?.TotalR0 ?? 0d) + _resistanceReactance.InternalR0 * cableCount;
            estimateResistanceReactance.TotalX1 = (sourceResistanceReactance?.TotalX1 ?? 0d) + _resistanceReactance.InternalX1 * cableCount;
            estimateResistanceReactance.TotalX0 = (sourceResistanceReactance?.TotalX0 ?? 0d) + _resistanceReactance.InternalX0 * cableCount;
        }                                                                                       
        else
        {
            estimateResistanceReactance.TotalR1 = _resistanceReactance.TotalR1;
            estimateResistanceReactance.TotalR0 = _resistanceReactance.TotalR0;
            estimateResistanceReactance.TotalX1 = _resistanceReactance.TotalX1;
            estimateResistanceReactance.TotalX0 = _resistanceReactance.TotalX0;
        }

        var voltage = GetVoltage(Electrical);

        EstimatedPowerParameters.ShortCurrent1 = 
            3 * voltage /
            Math.Sqrt(Math.Pow(2 * (estimateResistanceReactance.TotalR1 + ArcResistance) + estimateResistanceReactance.TotalR0, 2) +
                      Math.Pow(2 * estimateResistanceReactance.TotalX1 + estimateResistanceReactance.TotalX0, 2));

        if (Electrical.PowerParameters.PhasesNumber != PhasesNumber.Three)
            return;

        EstimatedPowerParameters.ShortCurrent3 = 
            voltage /
            Math.Sqrt(Math.Pow(estimateResistanceReactance.TotalR1 + ArcResistance, 2) +
                      Math.Pow(estimateResistanceReactance.TotalX1, 2));

        EstimatedPowerParameters.SurgeShortCurrent3 = Math.Sqrt(2) * EstimatedPowerParameters.ShortCurrent3 * GetShockFactor();

        // TODO 2021.01.12
        // if (!LockService.IsLocked(Lock.Calculate))
        // {
        //     EstimatedPowerParameters.ReferenceElectricalNode?.AllChildElectricalCircuits.ToList()
        //         .ForEach(ElectricalWarningService.CheckCircuitBreaker);
        // }
    }

    private static double GetVoltage(ElectricalBase electrical) => electrical switch
    {
        Transformer { SecondaryDistributionSystem: { } } transformer => ElectricalCalculator.LineToGroundVoltage(transformer.SecondaryDistributionSystem.LineToLineVoltage.ActualValue),
        _ => electrical.PowerParameters.LineToGroundVoltage,
    };

    private const double OmegaC = 2 * Math.PI * 50;

    private double GetShockFactor()
    {
        try
        {
            if (_resistanceReactance.TotalR1.Equals(0) ||
                _resistanceReactance.TotalX1.Equals(0))
                return 1;

            var ta = _resistanceReactance.TotalX1 / _resistanceReactance.TotalR1 / OmegaC;
            var fi = Math.Atan(_resistanceReactance.TotalX1 / _resistanceReactance.TotalR1);
            var tShock = 0.01 * (Math.PI / 2 + fi) / Math.PI;

            return 1 + Math.Sin(fi) * Math.Pow(Math.E, -tShock / ta);
        }
        catch (Exception exception)
        {
            Logger.Error(exception);
            return 1;
        }
    }

    private static double ArcResistance => _arcResistance.Equals(0)
        ? (_arcResistance = GetArcResistance())
        : _arcResistance;

    private static double GetArcResistance()
    {
        return 0;
            
        // using var uow = UnitOfWork.NewInstance();
        // var networkRepository = new ElectricalSourceRepository(uow);
        // var firstPanel = networkRepository.GetNetwork(ElectricalSystemTypeProxy.PowerCircuit).GetConsumersOf<SwitchBoardUnit>().FirstOrDefault();
        //
        // if (firstPanel?.EstimatedPowerParameters == null)
        //     return 0.015;
        //
        // var loadTrueEstimate = firstPanel.EstimatedPowerParameters.EstimateTrueLoad;
        //
        // if (loadTrueEstimate >= 2_500_000)
        //     return 0.003;
        //
        // if (loadTrueEstimate >= 1_600_000)
        //     return 0.004;
        //
        // if (loadTrueEstimate >= 1_000_000)
        //     return 0.005;
        //
        // if (loadTrueEstimate >= 630_000)
        //     return 0.007;
        //
        // if (loadTrueEstimate >= 400_000)
        //     return 0.010;
        //
        // return 0.015;
    }
}