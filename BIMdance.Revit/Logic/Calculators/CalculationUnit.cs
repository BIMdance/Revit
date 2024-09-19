// ReSharper disable LocalizableElement

namespace BIMdance.Revit.Logic.Calculators;

internal class CalculationUnit
{
    internal CalculationUnit(CalculationUnit calculationUnit)
    {
        Electrical = calculationUnit.Electrical;
        OperatingMode = calculationUnit.OperatingMode;
        PowerParameters = calculationUnit.PowerParameters;
        PhasesNumber = PowerParameters.PhasesNumber;
        Voltage = PowerParameters.LineToGroundVoltage;
        EstimatedPowerParameters = calculationUnit.EstimatedPowerParameters;
        ThreePhases = EstimatedPowerParameters.ThreePhases;
        DemandFactor = EstimatedPowerParameters.DemandFactor;
        AdditionalDemandFactor = EstimatedPowerParameters.AdditionalDemandFactor;
        TotalDemandFactor = EstimatedPowerParameters.TotalDemandFactor;
    }
    
    internal CalculationUnit(ElectricalBase electricalBase, OperatingMode operatingMode)
    {
        Electrical = electricalBase;
        OperatingMode = operatingMode;
        PowerParameters = Electrical.PowerParameters;
        PhasesNumber = PowerParameters.PhasesNumber;
        Voltage = PowerParameters.LineToGroundVoltage;
        EstimatedPowerParameters = Electrical.GetEstimatedPowerParameters(OperatingMode);
        ThreePhases = EstimatedPowerParameters.ThreePhases;
        DemandFactor = EstimatedPowerParameters.DemandFactor;
        AdditionalDemandFactor = EstimatedPowerParameters.AdditionalDemandFactor;
        TotalDemandFactor = EstimatedPowerParameters.TotalDemandFactor;
    }

    #region Properties

    internal ElectricalBase Electrical { get; }
    internal OperatingMode OperatingMode { get; }
    internal PowerParameters PowerParameters { get; }
    internal EstimatedPowerParameters EstimatedPowerParameters { get; }
    internal ThreePhasesParameters ThreePhases { get; }
    internal Dictionary<long, bool> CalculateSwitches { get; } = new();

    internal PhasesNumber PhasesNumber { get; }
    internal List<ElectricalElementProxy> AllElements { get; set; }
    internal LoadClassificationProxy CalculatedLoadClassification { get; set; }
    // private Dictionary<LoadClassificationProxy, double> _loadClassificationTrueEstimate1 = new();
    // private Dictionary<LoadClassificationProxy, double> _loadClassificationTrueEstimate2 = new();
    // private Dictionary<LoadClassificationProxy, double> _loadClassificationTrueEstimate3 = new();

    internal double ApparentLoad { get; set; }           // S  - Полная установленная мощность
    internal double ApparentLoad1 { get; set; }          // S1 - Полная установленная мощность для фазы 1
    internal double ApparentLoad2 { get; set; }          // S2 - Полная установленная мощность для фазы 2
    internal double ApparentLoad3 { get; set; }          // S3 - Полная установленная мощность для фазы 3
    internal double TrueLoad { get; set; }               // P  - Активная установленная мощность 
    internal double TrueLoad1 { get; set; }              // P1 - Активная установленная мощность для фазы 1
    internal double TrueLoad2 { get; set; }              // P2 - Активная установленная мощность для фазы 2
    internal double TrueLoad3 { get; set; }              // P3 - Активная установленная мощность для фазы 3
    internal double EstimateApparentLoad { get; set; }   // S  - Полная расчётная мощность
    internal double EstimateTrueLoad { get; set; }       // P  - Активная расчётная мощность
    internal double EstimateTrueLoad1 { get; set; }      // P1 - Активная расчётная мощность для фазы 1
    internal double EstimateTrueLoad2 { get; set; }      // P2 - Активная расчётная мощность для фазы 2
    internal double EstimateTrueLoad3 { get; set; }      // P3 - Активная расчётная мощность для фазы 3
    internal double EstimateCurrent { get; set; }        // I  - Расчётный ток
    internal double EstimateCurrent1 { get; set; }       // I1 - Расчётный ток для фазы 1
    internal double EstimateCurrent2 { get; set; }       // I2 - Расчётный ток для фазы 2
    internal double EstimateCurrent3 { get; set; }       // I3 - Расчётный ток для фазы 3
    internal double Asymmetry { get; set; }              // Несимметричность нагрузки
    internal double DemandFactor { get; set; }           // Ки - Коэффициент спроса или использования
    internal double AdditionalDemandFactor { get; set; } // Не используем в РТМ
    internal double TotalDemandFactor { get; set; }      // = DemandFactor 
    internal double PowerFactor { get; set; }            // cos(fi) - Коэффициент мощности 
    internal double Voltage { get; }                     // Uф - Фазное напряжение
    internal bool IsLoadCalculating { get; set; }

    #endregion

    internal (double apparentLoad, double trueLoad, double estimateTrueLoad)
        GetLoads(Phase phase) => phase switch
        {
            Phase.Undefined => (0, 0, 0),
            Phase.L123 => (0, 0, 0),
            Phase.L1 => (ThreePhases.ApparentLoad1, ThreePhases.TrueLoad1, ThreePhases.EstimateTrueLoad1),
            Phase.L2 => (ThreePhases.ApparentLoad2, ThreePhases.TrueLoad2, ThreePhases.EstimateTrueLoad2),
            Phase.L3 => (ThreePhases.ApparentLoad3, ThreePhases.TrueLoad3, ThreePhases.EstimateTrueLoad3),
            _ => (0, 0, 0)
        };

    internal void UpdateAllElements()
    {
        AllElements ??= StructureUtils.GetConnectedElements(Electrical, OperatingMode).ToList();
    }

    internal bool ToCalculate(ElementProxy element)
    {
        bool toCalculate;

        if (element is LoadClassificationProxy loadClassification)
        {
            toCalculate = !OperatingMode.DisabledElements.Contains(loadClassification.RevitId);
        }
        else if (CalculateSwitches.TryGetValue(element.RevitId, out var value))
        {
            toCalculate = value;
        }
        else
        {
            toCalculate = !OperatingMode.ElementSwitches.TryGetValue(element, out var v2) || v2;
            CalculateSwitches.Add(element.RevitId, toCalculate);
        }

        return toCalculate;
    }

    internal void UpdateParameters()
    {
        PowerParameters.LoadClassification = CalculatedLoadClassification;
        EstimatedPowerParameters.ApparentLoad = ApparentLoad;
        EstimatedPowerParameters.TrueLoad = TrueLoad;
        EstimatedPowerParameters.EstimateApparentLoad = EstimateApparentLoad;
        EstimatedPowerParameters.EstimateTrueLoad = EstimateTrueLoad;
        EstimatedPowerParameters.Current = EstimateCurrent;
        EstimatedPowerParameters.DemandFactor = DemandFactor;
        EstimatedPowerParameters.AdditionalDemandFactor = AdditionalDemandFactor;
        EstimatedPowerParameters.TotalDemandFactor = TotalDemandFactor;
        EstimatedPowerParameters.PowerFactor = PowerFactor;

        if (PhasesNumber == PhasesNumber.One)
            return;

        ThreePhases.ApparentLoad1 = ApparentLoad1;
        ThreePhases.ApparentLoad2 = ApparentLoad2;
        ThreePhases.ApparentLoad3 = ApparentLoad3;
        ThreePhases.TrueLoad1 = TrueLoad1;
        ThreePhases.TrueLoad2 = TrueLoad2;
        ThreePhases.TrueLoad3 = TrueLoad3;
        ThreePhases.EstimateTrueLoad1 = EstimateTrueLoad1;
        ThreePhases.EstimateTrueLoad2 = EstimateTrueLoad2;
        ThreePhases.EstimateTrueLoad3 = EstimateTrueLoad3;
        ThreePhases.EstimateCurrent1 = EstimateCurrent1;
        ThreePhases.EstimateCurrent2 = EstimateCurrent2;
        ThreePhases.EstimateCurrent3 = EstimateCurrent3;
        ThreePhases.Asymmetry = Asymmetry;
    }

    internal void Reset()
    {
        ApparentLoad = default;
        ApparentLoad1 = default;
        ApparentLoad2 = default;
        ApparentLoad3 = default;
        TrueLoad = default;
        TrueLoad1 = default;
        TrueLoad2 = default;
        TrueLoad3 = default;
        EstimateApparentLoad = default;
        EstimateTrueLoad = default;
        EstimateTrueLoad1 = default;
        EstimateTrueLoad2 = default;
        EstimateTrueLoad3 = default;
        EstimateCurrent = default;
        EstimateCurrent1 = default;
        EstimateCurrent2 = default;
        EstimateCurrent3 = default;
        Asymmetry = default;
        DemandFactor = 1;
        AdditionalDemandFactor = 1;
        TotalDemandFactor = 1;
        PowerFactor = 1;

        UpdateParameters();
    }
}