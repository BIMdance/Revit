

// ReSharper disable LocalizableElement

using BIMdance.Revit.Logic.Locks;

namespace BIMdance.Revit.Logic.Calculators;

internal class LoadCalculatorDefault : CalculationUnit
{
    internal LoadCalculatorDefault(ElectricalBase electricalBase, OperatingMode operatingMode) :
        base(electricalBase, operatingMode) { }

    internal LoadCalculatorDefault(CalculationUnit calculationUnit) :
        base(calculationUnit) { }

    internal void CalculateLoads(bool estimateOnly = false, bool calculateParentLoads = false)
    {
        if (LockService.IsLocked(Lock.Calculate))
            return;

        UpdateAllElements();

        if (!Electrical.IsPower)
        {
            return;
        }

        if (AllElements.IsEmpty())
        {
            Reset();

            if (calculateParentLoads)
                CalculateParentLoads(estimateOnly);

            return;
        }

        IsLoadCalculating = true;

        if (!estimateOnly)
            CalculateApparentAndTrueLoad();

        CalculateEstimateTrueLoad();
        CalculateDemandFactor();
        CalculatePowerFactor();
        CalculateEstimateCurrent();
        UpdateParameters();

        if (calculateParentLoads)
            CalculateParentLoads(estimateOnly);

        IsLoadCalculating = false;
    }

    #region CalculateLoads

    private void CalculateApparentAndTrueLoad()
    {
        switch (PhasesNumber)
        {
            case PhasesNumber.One:
                CalculateLoadApparentAndTrue1();
                break;

            case PhasesNumber.Two:
            case PhasesNumber.Three:
                CalculateLoadApparentAndTrue3();
                break;
        }
    }

    private void CalculateLoadApparentAndTrue1()
    {
        foreach (var element in AllElements.Where(element => ToCalculate(element.PowerParameters.LoadClassification)))
        {
            var elementPowerParameters = element.PowerParameters;
            var elementApparentLoad = elementPowerParameters.OwnApparentLoad;

            ApparentLoad += elementApparentLoad;
            TrueLoad += elementApparentLoad * elementPowerParameters.OwnPowerFactor;
        }
    }

    private void CalculateLoadApparentAndTrue3()
    {
        foreach (var element in AllElements.Where(element => ToCalculate(element.PowerParameters.LoadClassification)))
        {
            switch (element.PowerParameters.PhasesNumber)
            {
                case PhasesNumber.One:
                    AddLoadApparentAndTrueOnePhase(element);
                    continue;

                case PhasesNumber.Two:
                    AddLoadApparentAndTrueTwoPhases(element);
                    continue;

                case PhasesNumber.Three:
                    AddLoadApparentAndTrueThreePhases(element);
                    continue;
            }
        }

        ApparentLoad = ApparentLoad1 + ApparentLoad2 + ApparentLoad3;
        TrueLoad = TrueLoad1 + TrueLoad2 + TrueLoad3;
    }

    private void AddLoadApparentAndTrueOnePhase(ElectricalElementProxy element)
    {
        var elementPowerParameters = element.PowerParameters;
        var apparentLoad = elementPowerParameters.OwnApparentLoad;
        var trueLoad = apparentLoad * elementPowerParameters.OwnPowerFactor;
        var sourcePhase = GetSourcePhase(element);

        switch (sourcePhase)
        {
            case Phase.L1:
                ApparentLoad1 += apparentLoad;
                TrueLoad1 += trueLoad;
                break;

            case Phase.L2:
                ApparentLoad2 += apparentLoad;
                TrueLoad2 += trueLoad;
                break;

            case Phase.L3:
                ApparentLoad3 += apparentLoad;
                TrueLoad3 += trueLoad;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(sourcePhase), sourcePhase, new ArgumentOutOfRangeException().Message);
        }
    }

    private void AddLoadApparentAndTrueTwoPhases(ElectricalElementProxy element)
    {
        var elementPowerParameters = element.PowerParameters;
        var powerFactor = elementPowerParameters.OwnPowerFactor;
        var apparentLoad1 = elementPowerParameters.OwnApparentLoad1;
        var apparentLoad2 = elementPowerParameters.OwnApparentLoad2;
        var trueLoad1 = apparentLoad1 * powerFactor;
        var trueLoad2 = apparentLoad2 * powerFactor;
        var sourcePhase = GetSourcePhase(element);

        switch (sourcePhase)
        {
            case Phase.L12:
                ApparentLoad1 += apparentLoad1;
                ApparentLoad2 += apparentLoad2;
                TrueLoad1 += trueLoad1;
                TrueLoad2 += trueLoad2;
                return;

            case Phase.L13:
                ApparentLoad1 += apparentLoad1;
                ApparentLoad3 += apparentLoad2;
                TrueLoad1 += trueLoad1;
                TrueLoad3 += trueLoad2;
                return;

            case Phase.L23:
                ApparentLoad2 += apparentLoad1;
                ApparentLoad3 += apparentLoad2;
                TrueLoad2 += trueLoad1;
                TrueLoad3 += trueLoad2;
                return;

            default:
                throw new ArgumentOutOfRangeException(nameof(elementPowerParameters.Phase), elementPowerParameters.Phase, new ArgumentOutOfRangeException().Message);
        }
    }

    private static Phase GetSourcePhase(ElectricalElementProxy element)
    {
        var sourceCircuit = element.GetFirstSourceOf<ElectricalSystemProxy>();
        var sourcePhase = sourceCircuit.PowerParameters.Phase;

        CorrectPhaseValue(ref sourcePhase, PhasesNumber.One);

        if (sourceCircuit.PowerParameters.Phase != sourcePhase)
        {
            PhaseUtils.SetPhase(sourceCircuit, sourcePhase);
        }
            
        return sourcePhase;
    }

    private static void CorrectPhaseValue(ref Phase phase, PhasesNumber phasesNumber)
    {
        switch (phasesNumber)
        {
            case PhasesNumber.One:
                phase = phase switch
                {
                    < Phase.L1 => Phase.L1,
                    > Phase.L3 => (Phase)(1 + (int)phase % 3),
                    _ => phase
                };
                return;

            case PhasesNumber.Two:
                switch (phase)
                {
                    case < Phase.L1:
                        phase = Phase.L12;
                        break;
                    case < Phase.L12:
                        phase += 3;
                        break;
                }
                return;
        }
    }

    private void AddLoadApparentAndTrueThreePhases(ElectricalElementProxy element)
    {
        var elementPowerParameters = element.PowerParameters;
        var powerFactor = elementPowerParameters.OwnPowerFactor;
        var apparentLoad1 = elementPowerParameters.OwnApparentLoad1;
        var apparentLoad2 = elementPowerParameters.OwnApparentLoad2;
        var apparentLoad3 = elementPowerParameters.OwnApparentLoad3;

        ApparentLoad1 += apparentLoad1;
        ApparentLoad2 += apparentLoad2;
        ApparentLoad3 += apparentLoad3;

        TrueLoad1 += apparentLoad1 * powerFactor;
        TrueLoad2 += apparentLoad2 * powerFactor;
        TrueLoad3 += apparentLoad3 * powerFactor;
    }

    #endregion

    #region CalculateEstimateLoads

    private void CalculateEstimateTrueLoad()
    {
        var loadClassifications = AllElements.Select(n => n.PowerParameters.LoadClassification)
            .Distinct(new RevitProxyComparer<LoadClassificationProxy>());

        LoadClassificationProxy maxLoadClassification = null;
        var maxLoad = 0d;
        var demandFactorTrueLoad = 0d;

        foreach (var loadClassification in loadClassifications)
        {
            if (ToCalculate(loadClassification) == false)
                continue;

            var loadClassificationLoad = CalculateLoadClassification(loadClassification);

            demandFactorTrueLoad += loadClassificationLoad;

            if (loadClassificationLoad < maxLoad)
                continue;

            maxLoad = loadClassificationLoad;
            maxLoadClassification = loadClassification;
        }

        DemandFactor = /*powerElectrical.EstimateTrueLoad*/ demandFactorTrueLoad / TrueLoad;
        CalculatedLoadClassification = maxLoadClassification;
    }

    private double CalculateLoadClassification(LoadClassificationProxy loadClassification)
    {
        var elements = AllElements.Where(n =>
                Equals(n.PowerParameters.LoadClassification, loadClassification) &&
                n.PowerParameters.OwnApparentLoad > 0)
            .ToList();

        return PhasesNumber switch
        {
            PhasesNumber.One => CalculateEstimateTrueLoad1(loadClassification, elements),
            PhasesNumber.Two => CalculateEstimateTrueLoad3(loadClassification, elements),
            PhasesNumber.Three => CalculateEstimateTrueLoad3(loadClassification, elements),
            _ => throw new ArgumentOutOfRangeException(nameof(PhasesNumber), PhasesNumber, null)
        };
    }

    private double CalculateEstimateTrueLoad1(
        LoadClassificationProxy loadClassification,
        List<ElectricalElementProxy> loadClassificationElements)
    {
        var directConnectedTrueLoad = 0d;
        var directConnectedAdditionalTrueLoad = 0d;
        var nonDirectConnectedTrueLoad = 0d;
        var nonDirectConnectedAdditionalTrueLoad = 0d;
        var nonDirectConnectedElements = new List<ElectricalElementProxy>();

        foreach (var element in loadClassificationElements)
        {
            var elementPowerParameters = element.PowerParameters;
            var additionalDemandFactor = element.GetSourceChainOf<ElectricalSystemProxy>(to: Electrical)
                .Aggregate(1.0, (current, n) => current * n.GetEstimatedPowerParameters(OperatingMode).AdditionalDemandFactor);

            var elementTrueLoad = elementPowerParameters.OwnApparentLoad * elementPowerParameters.OwnPowerFactor;

            if (ElementIsDirectConnected(element, Electrical))
            {
                directConnectedTrueLoad += elementTrueLoad;
                directConnectedAdditionalTrueLoad += elementTrueLoad * additionalDemandFactor;
            }
            else
            {
                nonDirectConnectedElements.Add(element);
                nonDirectConnectedTrueLoad += elementTrueLoad;
                nonDirectConnectedAdditionalTrueLoad += elementTrueLoad * additionalDemandFactor;
            }
        }

        var demandFactor = loadClassification?.DemandFactor?.GetValue(nonDirectConnectedElements) ?? 1;
        var estimateTrueLoad = directConnectedTrueLoad + nonDirectConnectedTrueLoad * demandFactor;
        var estimateAdditionalTrueLoad = directConnectedAdditionalTrueLoad + nonDirectConnectedAdditionalTrueLoad * demandFactor;

        EstimateTrueLoad += estimateAdditionalTrueLoad;

        return estimateTrueLoad;
    }

    private double CalculateEstimateTrueLoad3(
        LoadClassificationProxy loadClassification,
        List<ElectricalElementProxy> loadClassificationElements)
    {
        var directConnectedTrueLoad = new ThreePhasesLoad();
        var directConnectedAdditionalTrueLoad = new ThreePhasesLoad();
        var nonDirectConnectedTrueLoad = new ThreePhasesLoad();
        var nonDirectConnectedAdditionalTrueLoad = new ThreePhasesLoad();
        var nonDirectConnectedElements = new List<ElectricalElementProxy>();

        foreach (var element in loadClassificationElements)
        {
            if (ElementIsDirectConnected(element, Electrical))
            {
                CalculateTrueLoad(ref directConnectedTrueLoad, ref directConnectedAdditionalTrueLoad, element);//, parentCircuit);
            }
            else
            {
                CalculateTrueLoad(ref nonDirectConnectedTrueLoad, ref nonDirectConnectedAdditionalTrueLoad, element);//, parentCircuit);
                nonDirectConnectedElements.Add(element);
            }
        }

        var demandFactor = loadClassification?.DemandFactor?.GetValue(nonDirectConnectedElements) ?? 1;
        var estimateTrueLoad = directConnectedTrueLoad + nonDirectConnectedTrueLoad * demandFactor;
        var estimateAdditionalTrueLoad = directConnectedAdditionalTrueLoad + nonDirectConnectedAdditionalTrueLoad * demandFactor;// * circuitAdditionalDemandFactor;

        // if (loadClassification != null)
        // {
        //     AddLoadToLoadClassification(_loadClassificationTrueEstimate1, loadClassification, estimateAdditionalTrueLoad.Load1);
        //     AddLoadToLoadClassification(_loadClassificationTrueEstimate2, loadClassification, estimateAdditionalTrueLoad.Load2);
        //     AddLoadToLoadClassification(_loadClassificationTrueEstimate3, loadClassification, estimateAdditionalTrueLoad.Load3);
        // }

        EstimateTrueLoad1 += estimateAdditionalTrueLoad.Load1;
        EstimateTrueLoad2 += estimateAdditionalTrueLoad.Load2;
        EstimateTrueLoad3 += estimateAdditionalTrueLoad.Load3;

        EstimateTrueLoad = EstimateTrueLoad1 + EstimateTrueLoad2 + EstimateTrueLoad3;

        return estimateTrueLoad.Total;
    }

    // private static void AddLoadToLoadClassification(IDictionary<LoadClassificationProxy, double> loadClassificationDictionary, LoadClassificationProxy loadClassification, double value)
    // {
    //     if (loadClassificationDictionary.ContainsKey(loadClassification))
    //         loadClassificationDictionary[loadClassification] += value;
    //     else
    //         loadClassificationDictionary.Add(loadClassification, value);
    // }

    private static bool ElementIsDirectConnected(ElectricalElementProxy element, ElectricalBase source)
    {
        return source is ElectricalSystemProxy electricalCircuit &&
               Equals(electricalCircuit, element.GetFirstSourceOf<ElectricalSystemProxy>());
    }

    private void CalculateTrueLoad(
        ref ThreePhasesLoad trueLoad,
        ref ThreePhasesLoad additionalTrueLoad,
        ElectricalElementProxy element)
    {
        var additionalDemandFactor = element.GetSourceChainOf<ElectricalSystemProxy>(to: Electrical)
            .Aggregate(1.0, (current, n) => current * n.GetEstimatedPowerParameters(OperatingMode).AdditionalDemandFactor);

        switch (element.PowerParameters.PhasesNumber)
        {
            case PhasesNumber.One:
                AddOnePhaseLoad(ref trueLoad, ref additionalTrueLoad, element, additionalDemandFactor);
                return;

            case PhasesNumber.Two:
            case PhasesNumber.Three:
                AddThreePhasesLoad(ref trueLoad, ref additionalTrueLoad, element, additionalDemandFactor);
                return;
        }
    }

    private static void AddOnePhaseLoad(ref ThreePhasesLoad trueLoad, ref ThreePhasesLoad additionalTrueLoad, ElectricalElementProxy element, double additionalDemandFactor)
    {
        var powerElectrical = element.PowerParameters;
        var elementTrueLoad = powerElectrical.OwnApparentLoad * powerElectrical.OwnPowerFactor;
        var elementAdditionalTrueLoad = elementTrueLoad * additionalDemandFactor;

        switch (element.GetFirstSourceOf<ElectricalSystemProxy>().PowerParameters.Phase)
        {
            case Phase.L1:
                trueLoad.Load1 += elementTrueLoad;
                additionalTrueLoad.Load1 += elementAdditionalTrueLoad;
                return;

            case Phase.L2:
                trueLoad.Load2 += elementTrueLoad;
                additionalTrueLoad.Load2 += elementAdditionalTrueLoad;
                return;

            case Phase.L3:
                trueLoad.Load3 += elementTrueLoad;
                additionalTrueLoad.Load3 += elementAdditionalTrueLoad;
                return;
        }
    }

    private static void AddThreePhasesLoad(ref ThreePhasesLoad trueLoad, ref ThreePhasesLoad additionalTrueLoad, ElectricalElementProxy element, double additionalDemandFactor)
    {
        var elementPowerParameters = element.PowerParameters;
        var elementTrueLoad3 = elementPowerParameters.IsTwoPhases
            ? TwoPhasesElementTrueLoad(element)
            : ThreePhasesElementTrueLoad(element);

        trueLoad += elementTrueLoad3;
        additionalTrueLoad += elementTrueLoad3 * additionalDemandFactor;
    }

    private static ThreePhasesLoad TwoPhasesElementTrueLoad(ElectricalBase electrical)
    {
        var powerParameters = electrical.PowerParameters;
        var powerFactor = powerParameters.OwnPowerFactor;

        return powerParameters.Phase switch
        {
            Phase.L1 => new ThreePhasesLoad { Load1 = powerParameters.OwnApparentLoad * powerFactor, },
            Phase.L2 => new ThreePhasesLoad { Load2 = powerParameters.OwnApparentLoad * powerFactor, },
            Phase.L3 => new ThreePhasesLoad { Load3 = powerParameters.OwnApparentLoad * powerFactor, },
            Phase.L12 => new ThreePhasesLoad
            {
                Load1 = powerParameters.OwnApparentLoad1 * powerFactor,
                Load2 = powerParameters.OwnApparentLoad2 * powerFactor,
            },
            Phase.L13 => new ThreePhasesLoad
            {
                Load1 = powerParameters.OwnApparentLoad1 * powerFactor,
                Load3 = powerParameters.OwnApparentLoad3 * powerFactor,
            },
            Phase.L23 => new ThreePhasesLoad
            {
                Load2 = powerParameters.OwnApparentLoad2 * powerFactor,
                Load3 = powerParameters.OwnApparentLoad3 * powerFactor,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(powerParameters.Phase), powerParameters.Phase,
                $"Invalid value for 2-phases load.\n{new ArgumentOutOfRangeException().Message}")
        };
    }

    private static ThreePhasesLoad ThreePhasesElementTrueLoad(ElectricalBase electrical)
    {
        var powerParameters = electrical.PowerParameters;
        var powerFactor = powerParameters.OwnPowerFactor;

        return new ThreePhasesLoad
        {
            Load1 = powerParameters.OwnApparentLoad1 * powerFactor,
            Load2 = powerParameters.OwnApparentLoad2 * powerFactor,
            Load3 = powerParameters.OwnApparentLoad3 * powerFactor
        };
    }

    private struct ThreePhasesLoad
    {
        public double Load1 { get; set; }
        public double Load2 { get; set; }
        public double Load3 { get; set; }
        public double Total => Load1 + Load2 + Load3;

        public static ThreePhasesLoad operator +(ThreePhasesLoad threePhasesLoad1, ThreePhasesLoad threePhasesLoad2)
        {
            return new ThreePhasesLoad
            {
                Load1 = threePhasesLoad1.Load1 + threePhasesLoad2.Load1,
                Load2 = threePhasesLoad1.Load2 + threePhasesLoad2.Load2,
                Load3 = threePhasesLoad1.Load3 + threePhasesLoad2.Load3,
            };
        }

        public static ThreePhasesLoad operator *(ThreePhasesLoad threePhasesLoad, double multiplier)
        {
            return new ThreePhasesLoad
            {
                Load1 = threePhasesLoad.Load1 * multiplier,
                Load2 = threePhasesLoad.Load2 * multiplier,
                Load3 = threePhasesLoad.Load3 * multiplier,
            };
        }

        public static ThreePhasesLoad operator *(double multiplier, ThreePhasesLoad threePhasesLoad) => threePhasesLoad * multiplier;

        public override string ToString()
        {
            return $"{Total} = {Load1} + {Load2} + {Load3}";
        }
    }

    #endregion

    #region CalculateEstimateCurrents

    private void CalculateEstimateCurrent()
    {
        if (PhasesNumber == PhasesNumber.One)
            CalculateOnePhaseEstimateCurrent();
        else
            CalculateThreePhasesEstimateCurrent();
    }

    private void CalculateOnePhaseEstimateCurrent()
    {
        EstimateCurrent = EstimateApparentLoad / Voltage / (int)PhasesNumber;
    }

    private void CalculateThreePhasesEstimateCurrent()
    {
        CalculateEstimateCurrent(Phase.L1);
        CalculateEstimateCurrent(Phase.L2);
        CalculateEstimateCurrent(Phase.L3);
    }

    private void CalculateEstimateCurrent(Phase phase)
    {
        var estimateCurrent = GetEstimateCurrent(phase);

        SetEstimateCurrent(phase, estimateCurrent);
    }

    private double GetEstimateCurrent(Phase phase)
    {
        var (apparentLoad, trueLoad, estimateTrueLoad) = GetLoads(phase);
        var demandFactor = trueLoad > 0 ? estimateTrueLoad / trueLoad : 1;
        var estimateApparentLoad = demandFactor * apparentLoad;
        var voltage = PowerParameters.LineToGroundVoltage;
        var estimateCurrent = voltage > 0 ? estimateApparentLoad / voltage : 0;

        return estimateCurrent;
    }

    private new (double apparentLoad, double trueLoad, double estimateTrueLoad)
        GetLoads(Phase phase)
    {
        return phase switch
        {
            Phase.L1 => (ApparentLoad1, TrueLoad1, EstimateTrueLoad1),
            Phase.L2 => (ApparentLoad2, TrueLoad2, EstimateTrueLoad2),
            Phase.L3 => (ApparentLoad3, TrueLoad3, EstimateTrueLoad3),
            _ => (0, 0, 0)
        };
    }

    private void SetEstimateCurrent(Phase phase, double estimateCurrent)
    {
        switch (phase)
        {
            case Phase.L1:
                EstimateCurrent1 = estimateCurrent;
                break;

            case Phase.L2:
                EstimateCurrent2 = estimateCurrent;
                break;

            case Phase.L3:
                EstimateCurrent3 = estimateCurrent;
                break;
        }

        EstimateCurrent = new[] { EstimateCurrent1, EstimateCurrent2, EstimateCurrent3 }.Max();

        CalculateAsymmetry();
    }

    private void CalculateAsymmetry()
    {
        var currents = new[]
        {
            EstimateCurrent1,
            EstimateCurrent2,
            EstimateCurrent3,
        };

        if (PowerParameters.IsTwoPhases)
            currents = currents.Where(n => n > 1e-3).ToArray();

        Asymmetry = currents.Any()
            ? (1 - currents.Min() / currents.Max()).Round(3, 3)
            : 0;
    }

    #endregion

    private void CalculateDemandFactor()
    {
        TotalDemandFactor = (TrueLoad > 0 ? EstimateTrueLoad / TrueLoad : 1).Round(3, 3);
    }

    protected void CalculatePowerFactor()
    {
        EstimateApparentLoad = ApparentLoad * TotalDemandFactor;
        PowerFactor = EstimateApparentLoad > 0 ? EstimateTrueLoad / EstimateApparentLoad : 1;
    }

    internal void SetTotalDemandFactor(double value)
    {
        value = value.Round(3, 3);
        var oldTotalDemandFactor = TotalDemandFactor;

        if (Math.Abs(oldTotalDemandFactor - value) < 1e-3)
            return;

        TotalDemandFactor = value;

        SpecifyAdditionalDemandFactor(value, oldTotalDemandFactor);
    }

    private void SpecifyAdditionalDemandFactor(double newTotalDemandFactor, double oldTotalDemandFactor)
    {
        var additionalDemandFactor = AdditionalDemandFactor;

        if (Math.Abs(oldTotalDemandFactor) < 1e-3)
        {
            additionalDemandFactor = 1;
            oldTotalDemandFactor = TotalDemandFactor;
        }

        SetAdditionalDemandFactor(additionalDemandFactor * newTotalDemandFactor / oldTotalDemandFactor);
    }

    private bool _isAdditionalDemandFactorUpdating;

    internal void SetAdditionalDemandFactor(double newAdditionalDemandFactor)
    {
        if (_isAdditionalDemandFactorUpdating)
            return;

        _isAdditionalDemandFactorUpdating = true;

        newAdditionalDemandFactor = newAdditionalDemandFactor.Round(3, 3);
        var oldAdditionalDemandFactor = AdditionalDemandFactor;

        if (Math.Abs(oldAdditionalDemandFactor - newAdditionalDemandFactor) < 1e-3)
            return;

        AdditionalDemandFactor = newAdditionalDemandFactor;
        EstimatedPowerParameters.AdditionalDemandFactor = AdditionalDemandFactor;

        if (LockService.IsLocked(Lock.Calculate))
            return;

        if (!IsLoadCalculating)
            ReCalculateEstimateTrueLoad(newAdditionalDemandFactor, oldAdditionalDemandFactor);

        _isAdditionalDemandFactorUpdating = false;
    }

    private void ReCalculateEstimateTrueLoad(double newAdditionalDemandFactor, double oldAdditionalDemandFactor)
    {
        if (LockService.IsLocked(Lock.Calculate))
            return;

        SetLoads();

        switch (Electrical)
        {
            case ElectricalSystemProxy:
                CalculateLoads(estimateOnly: true);
                CalculateParentLoads(estimateOnly: true);
                break;

            case ElectricalEquipmentProxy equipment:
                SetEstimateLoads();
                ReCalculateEstimateTrueLoad(equipment, newAdditionalDemandFactor, oldAdditionalDemandFactor);
                break;

            case ElectricalElementProxy _:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(Electrical), Electrical, new ArgumentOutOfRangeException().Message);
        }

        UpdateParameters();
    }

    private void SetLoads()
    {
        ApparentLoad = EstimatedPowerParameters.ApparentLoad;
        TrueLoad = EstimatedPowerParameters.TrueLoad;

        if (PhasesNumber is PhasesNumber.One)
            return;
        
        ApparentLoad1 = ThreePhases.ApparentLoad1;
        ApparentLoad2 = ThreePhases.ApparentLoad2;
        ApparentLoad3 = ThreePhases.ApparentLoad3;
        TrueLoad1 = ThreePhases.TrueLoad1;
        TrueLoad2 = ThreePhases.TrueLoad2;
        TrueLoad3 = ThreePhases.TrueLoad3;
    }

    private void SetEstimateLoads()
    {
        EstimateTrueLoad = EstimatedPowerParameters.EstimateTrueLoad;

        if (PhasesNumber is PhasesNumber.One)
            return;
        
        EstimateTrueLoad1 = ThreePhases.EstimateTrueLoad1;
        EstimateTrueLoad2 = ThreePhases.EstimateTrueLoad2;
        EstimateTrueLoad3 = ThreePhases.EstimateTrueLoad3;
    }

    private void CalculateEstimateTrueLoad(ElectricalSystemProxy powerCircuit)
    {
        //SetDemandFactorAdditionalToChild(powerCircuit);
        CalculateParentLoads(estimateOnly: true);
    }

    public void CalculateParentLoads(bool estimateOnly)
    {
        var source = Electrical.BaseSource;

        if (source == null)
            return;

        var calculator = new LoadCalculatorDefault(source, OperatingMode);
        calculator.CalculateLoads(estimateOnly, calculateParentLoads: true);
    }

    private static ElectricalEquipmentProxy GetSourceElectricalEquipment(ElectricalBase electrical) => electrical switch
    {
        ElectricalEquipmentProxy equipment => equipment,
        ElectricalSystemProxy circuit => circuit.GetFirstSourceOf<ElectricalEquipmentProxy>(),
        null => throw new NullReferenceException(nameof(electrical)),
        _ => throw new ArgumentOutOfRangeException(nameof(electrical), electrical,
            new ArgumentOutOfRangeException().Message)
    };

    private void ReCalculateEstimateTrueLoad(ElectricalEquipmentProxy electricalEquipment, double value, double oldValue)
    {
        switch (PhasesNumber)
        {
            case PhasesNumber.One:
                ReCalculateLoadTrueEstimate1(value, oldValue);
                break;

            case PhasesNumber.Two:
            case PhasesNumber.Three:
                ReCalculateLoadTrueEstimate3(value, oldValue);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(PowerParameters.PhasesNumber), electricalEquipment.PowerParameters.PhasesNumber, new ArgumentOutOfRangeException().Message);
        }
    }

    private void ReCalculateLoadTrueEstimate1(double value, double oldValue)
    {
        if (oldValue.Equals(0))
        {
            var loadTrueEstimate = Electrical.GetConsumersOf<ElectricalSystemProxy>()
                .Sum(n => n.GetEstimatedPowerParameters(OperatingMode).EstimateTrueLoad);

            EstimateTrueLoad += loadTrueEstimate * value;
        }
        else
        {
            EstimateTrueLoad += EstimateTrueLoad * (value / oldValue - 1);
        }

        CalculatePowerFactor();
        CalculateEstimateCurrent();
    }

    private void ReCalculateLoadTrueEstimate3(double value, double oldValue)
    {
        try
        {
            if (oldValue.Equals(0))
            {
                var circuits = Electrical.GetConsumersOf<ElectricalSystemProxy>()
                    .Select(n => new
                    {
                        n.PowerParameters.Phase,
                        n.PowerParameters.PhasesNumber,
                        Parameters = n.GetEstimatedPowerParameters(OperatingMode)
                    }).ToList();

                var circuits1 = circuits.Where(n => n.PhasesNumber != PhasesNumber.Three).ToList();
                var circuits3 = circuits.Where(n => n.PhasesNumber == PhasesNumber.Three).ToList();

                var estimateTrueLoad1 = circuits1.Where(n => n.Phase == Phase.L1).Sum(n => n.Parameters.EstimateTrueLoad) 
                                      + circuits3.Sum(n => n.Parameters.ThreePhases.EstimateTrueLoad1);
                var estimateTrueLoad2 = circuits1.Where(n => n.Phase == Phase.L2).Sum(n => n.Parameters.EstimateTrueLoad)
                                      + circuits3.Sum(n => n.Parameters.ThreePhases.EstimateTrueLoad2);
                var estimateTrueLoad3 = circuits1.Where(n => n.Phase == Phase.L3).Sum(n => n.Parameters.EstimateTrueLoad)
                                      + circuits3.Sum(n => n.Parameters.ThreePhases.EstimateTrueLoad3);

                EstimateTrueLoad1 += estimateTrueLoad1 * value;
                EstimateTrueLoad2 += estimateTrueLoad2 * value;
                EstimateTrueLoad3 += estimateTrueLoad3 * value;
            }
            else
            {
                var factor = value / oldValue - 1;
                EstimateTrueLoad1 += EstimateTrueLoad1 * factor;
                EstimateTrueLoad2 += EstimateTrueLoad2 * factor;
                EstimateTrueLoad3 += EstimateTrueLoad3 * factor;
            }

            CalculatePowerFactor();
            CalculateEstimateCurrent(Phase.L1);
            CalculateEstimateCurrent(Phase.L2);
            CalculateEstimateCurrent(Phase.L3);
        }
        catch (Exception exception)
        {
            Logger.Error(exception);
        }
    }
}