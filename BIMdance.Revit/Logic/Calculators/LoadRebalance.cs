using BIMdance.Revit.Logic.Locks;
using StringComparer = BIMdance.Revit.Utils.Common.StringComparer;

namespace BIMdance.Revit.Logic.Calculators;

internal class LoadRebalance
{
    private const int AttemptsNumber = 5;
    private static int _counter = 0;

    private readonly ElectricalCalculator _electricalCalculator;
    private readonly ElectricalContext _electricalContext;

    public LoadRebalance(ElectricalCalculator electricalCalculator, ElectricalContext electricalContext)
    {
        _electricalCalculator = electricalCalculator;
        _electricalContext = electricalContext;
    }

    internal void RebalanceAll(RebalanceLoadMode rebalanceLoadMode, OperatingMode operatingMode, double tolerancePercent = 0)
    {
        var sources = _electricalContext.ElectricalSources;
        var electricalEquipments = StructureUtils.GetOrderedConnectedEquipments(sources, operatingMode).Reverse().ToArray();
        var isPhasesChanged = false;

        // ProgressBarController.Title = ProgressBarDescriptions.LoadBalancing + (_counter > 0 ? $" {_counter + 1} / { AttemptsNumber}" : string.Empty);
            
        // using (ProgressBarController.StartProcess(2, ProgressBarDescriptions.LoadBalancing))
        {
            // using (ProgressBarController.StartProcess(powerPanels.Count))
            using (LockService.ToLock(Lock.Calculate))
            {
                isPhasesChanged = rebalanceLoadMode switch
                {
                    RebalanceLoadMode.ByOrder => electricalEquipments.Aggregate(false, (current, n) => current | Rebalance(n as SwitchBoardUnit, rebalanceLoadMode, operatingMode, tolerancePercent)),
                    RebalanceLoadMode.ByLoad => electricalEquipments.Aggregate(false, (current, n) => current | Rebalance(n as SwitchBoardUnit, rebalanceLoadMode, operatingMode, tolerancePercent)),
                    _ => throw new ArgumentOutOfRangeException(nameof(rebalanceLoadMode), rebalanceLoadMode, null)
                };
            }

            // ProgressBarController.Next(ProgressBarDescriptions.AllElectricalLoadsCalculation);

            _electricalCalculator.Loads.CalculateAll(operatingMode);
                
            if (isPhasesChanged == false ||
                _counter >= AttemptsNumber ||
                electricalEquipments.All(n => n.PowerParameters.PhasesNumber != PhasesNumber.Two))
            {
                _counter = 0;
                // ProgressBarController.Close();
                    
                return;
            }

        }

        _counter++;
        
        RebalanceAll(rebalanceLoadMode, operatingMode, tolerancePercent);

        _counter = 0;
        // ProgressBarController.Close();
    }

    internal bool Rebalance(SwitchBoardUnit electricalPanel, RebalanceLoadMode rebalanceLoadMode, OperatingMode operatingMode, double tolerancePercent = 0)
    {
        // ProgressBarController.Next($"{ProgressBarDescriptions.LoadBalancing} {electricalPanel.Name}");
            
        var isPhasesChanged = false;

        using (LockService.ToLock(Lock.Phase))
        {
            switch (rebalanceLoadMode)
            {
                case RebalanceLoadMode.ByOrder:
                    isPhasesChanged = RebalanceLoadByOrder(electricalPanel);
                    break;

                case RebalanceLoadMode.ByLoad:
                    isPhasesChanged = RebalanceLoadByLoad(electricalPanel, tolerancePercent);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(rebalanceLoadMode), rebalanceLoadMode, null);
            }
        }

        if (isPhasesChanged && LockService.IsLocked(Lock.Calculate) == false)
            _electricalCalculator.Loads.CalculateParents(electricalPanel, operatingMode);

        return isPhasesChanged;
    }

    private static bool RebalanceLoadByOrder(SwitchBoardUnit electricalPanel) => electricalPanel.PowerParameters?.PhasesNumber switch
    {
        PhasesNumber.Two => BalanceTwoPhasesPanelByOrder(electricalPanel),
        PhasesNumber.Three => BalanceThreePhasesPanelByOrder(electricalPanel),
        _ => false
    };

    private static bool BalanceTwoPhasesPanelByOrder(SwitchBoardUnit electricalPanel)
    {
        if (electricalPanel.PowerParameters?.PhasesNumber == PhasesNumber.One)
            return false;

        var onePhaseCircuits = electricalPanel
            .GetConsumersOf<ElectricalSystemProxy>()
            .Where(x => x.PowerParameters?.IsOnePhase ?? false)
            .OrderBy(x => x.CircuitNumber).ToList();
        var firstOnePhaseCircuit = onePhaseCircuits.FirstOrDefault();
        var firstOnePhaseCircuitPhase = firstOnePhaseCircuit?.PowerParameters.Phase ?? Phase.Undefined;
        var isPhasesChanged = false;
        var currentPhase = firstOnePhaseCircuitPhase;

        foreach (var circuit in onePhaseCircuits)
        {
            if (circuit.PowerParameters.Phase != currentPhase)
            {
                PhaseUtils.SetPhaseAndCalculate(circuit, currentPhase);
                isPhasesChanged = true;
            }

            currentPhase = RebalanceHelper.GetNextPhaseInTwoPhasesPanel(electricalPanel, currentPhase);
        }

        if (isPhasesChanged)
            RebalanceHelper.MovePhases(onePhaseCircuits, firstOnePhaseCircuit, firstOnePhaseCircuitPhase);

        return isPhasesChanged;
    }

    private static bool BalanceThreePhasesPanelByOrder(SwitchBoardUnit electricalPanel)
    {
        if (electricalPanel.PowerParameters?.PhasesNumber == PhasesNumber.One)
            return false;

        var nonThreePhaseCircuits = electricalPanel
            .GetConsumersOf<ElectricalSystemProxy>()
            .Where(x => x.IsPower && x.PowerParameters.PhasesNumber != PhasesNumber.Three)
            .OrderBy(x => x.CircuitNumber, new StringComparer()).ToList();
        var firstPhaseCircuit = nonThreePhaseCircuits.FirstOrDefault();
        var firstOnePhaseCircuitPhase = firstPhaseCircuit?.PowerParameters.Phase ?? Phase.Undefined;
        var isPhasesChanged = false;
        var phaseIndex = 0;

        foreach (var circuit in nonThreePhaseCircuits)
        {
            var newPhase = (Phase)(++phaseIndex) + (circuit.PowerParameters.IsTwoPhases ? 3 : 0);

            if (circuit.PowerParameters.Phase != newPhase)
            {
                PhaseUtils.SetPhaseAndCalculate(circuit, newPhase);
                isPhasesChanged = true;
            }

            phaseIndex %= 3;
        }

        if (isPhasesChanged)
            RebalanceHelper.MovePhases(nonThreePhaseCircuits, firstPhaseCircuit, firstOnePhaseCircuitPhase);

        return isPhasesChanged;
    }

    private static bool RebalanceLoadByLoad(SwitchBoardUnit electricalPanel, double tolerancePercent) => electricalPanel.PowerParameters?.PhasesNumber switch
    {
        PhasesNumber.Two => BalanceTwoPhasesPanelByLoad(electricalPanel, tolerancePercent),
        PhasesNumber.Three => BalanceThreePhasesPanelByLoad(electricalPanel, tolerancePercent),
        _ => false
    };

    private static bool BalanceTwoPhasesPanelByLoad(SwitchBoardUnit electricalPanel, double tolerancePercent)
    {
        if (RebalanceHelper.IsBalanced(electricalPanel, tolerancePercent))
            return false;

        var powerCircuits = electricalPanel.GetConsumersOf<ElectricalSystemProxy>().ToArray();
        var onePhaseCircuits = powerCircuits
            .Where(n => n.PowerParameters?.IsOnePhase ?? false)
            .OrderBy(n => n.CircuitNumber, new StringComparer()).ToArray();
        var firstOnePhaseCircuit = onePhaseCircuits.FirstOrDefault();
        var firstOnePhaseCircuitPhase = firstOnePhaseCircuit?.PowerParameters.Phase ?? Phase.Undefined;
        var twoPhasesCircuits = powerCircuits.Where(n => n.PowerParameters.IsThreePhases);
        var totalCurrents = new TotalCurrents(electricalPanel.PowerParameters.Phase);
        totalCurrents.AddCurrents(twoPhasesCircuits, electricalPanel.PowerParameters.Phase);

        var isPhasesChanged = onePhaseCircuits
            .OrderByDescending(n => n.EstimatedPowerParameters.Current)
            .Aggregate(false, (current, n) => current | totalCurrents.SetPhase(n));

        if (isPhasesChanged)
            RebalanceHelper.MovePhases(onePhaseCircuits, firstOnePhaseCircuit, firstOnePhaseCircuitPhase);

        return isPhasesChanged;
    }

    private static bool BalanceThreePhasesPanelByLoad(SwitchBoardUnit electricalPanel, double tolerancePercent)
    {
        if (RebalanceHelper.IsBalanced(electricalPanel, tolerancePercent))
            return false;

        var powerCircuits = electricalPanel.GetConsumersOf<ElectricalSystemProxy>().ToArray();
        var nonThreePhaseCircuits = powerCircuits
            .Where(x => x.IsPower && x.PowerParameters.PhasesNumber != PhasesNumber.Three)
            .OrderBy(x => x.CircuitNumber, new StringComparer()).ToArray();
        var firstPhaseCircuit = nonThreePhaseCircuits.FirstOrDefault();
        var firstOnePhaseCircuitPhase = firstPhaseCircuit?.PowerParameters.Phase ?? Phase.Undefined;
        var threePhasesCircuits = powerCircuits.Where(n => n.PowerParameters.IsThreePhases);
        var totalCurrents = new TotalCurrents(electricalPanel.PowerParameters.Phase);
        totalCurrents.AddCurrents(threePhasesCircuits, Phase.L123);

        var isPhasesChanged = nonThreePhaseCircuits
            .OrderByDescending(x => x.EstimatedPowerParameters.Current)
            .Aggregate(false, (current, n) => current | totalCurrents.SetPhase(n));

        if (isPhasesChanged)
            RebalanceHelper.MovePhases(nonThreePhaseCircuits, firstPhaseCircuit, firstOnePhaseCircuitPhase);

        return isPhasesChanged;
    }
}

internal class TotalCurrents
{
    internal TotalCurrents(Phase panelPhase)
    {
        PhaseCurrents = panelPhase switch
        {
            Phase.L12 => new Dictionary<Phase, double> { [Phase.L1] = 0, [Phase.L2] = 0 },
            Phase.L23 => new Dictionary<Phase, double> { [Phase.L2] = 0, [Phase.L3] = 0 },
            Phase.L13 => new Dictionary<Phase, double> { [Phase.L1] = 0, [Phase.L3] = 0 },
            Phase.L123 => new Dictionary<Phase, double> { [Phase.L1] = 0, [Phase.L2] = 0, [Phase.L3] = 0 },
            Phase.Undefined => throw new ArgumentException($"Wrong {nameof(panelPhase)} = {panelPhase}"),
            _ => null
        };
    }

    internal Dictionary<Phase, double> PhaseCurrents { get; set; }

    internal bool SetPhase(ElectricalSystemProxy electricalCircuit)
    {
        var powerElectrical = electricalCircuit.PowerParameters;
        var oldPhase = powerElectrical.Phase;
        var minPhaseInTotal = GetMinPhase(powerElectrical.PhasesNumber);

        AddCurrents(electricalCircuit, minPhaseInTotal);

        if (oldPhase == minPhaseInTotal)
            return false;

        PhaseUtils.SetPhaseAndCalculate(electricalCircuit, minPhaseInTotal);
            
        return true;
    }

    private Phase GetMinPhase(PhasesNumber phasesNumber) => phasesNumber switch
    {
        PhasesNumber.One => GetMinCurrentPhase(),
        PhasesNumber.Two => GetMinTwoPhases(),
        _ => throw new ArgumentOutOfRangeException(nameof(phasesNumber), phasesNumber, "Only non three phases loads are allowed.")
    };

    private Phase GetMinTwoPhases()
    {
        var firstValue = PhaseCurrents.Values.FirstOrDefault();

        if (PhaseCurrents.Values.All(n => n.Equals(firstValue)))
            return Phase.L12;

        var phase1 = GetMinCurrentPhase();
        var phase2 = GetMinCurrentPhase(phase1);

        return phase1 switch
        {
            Phase.L1 when phase2 == Phase.L2 => Phase.L12,
            Phase.L2 when phase2 == Phase.L1 => Phase.L12,
            Phase.L1 when phase2 == Phase.L3 => Phase.L13,
            Phase.L3 when phase2 == Phase.L1 => Phase.L13,
            Phase.L2 when phase2 == Phase.L3 => Phase.L23,
            Phase.L3 when phase2 == Phase.L2 => Phase.L23,
            _ => throw new ArgumentOutOfRangeException($"Invalid phases combinations: {phase1} and {phase2}")
        };
    }

    private Phase GetMinCurrentPhase(Phase excludePhase = Phase.Undefined)
    {
        var minCurrentPhase = Phase.L1;
        var minCurrent = double.MaxValue;

        foreach (var phaseCurrent in PhaseCurrents)
        {
            var phase = phaseCurrent.Key;
            var current = phaseCurrent.Value;

            if (current < minCurrent && phase != excludePhase)
            {
                minCurrent = current;
                minCurrentPhase = phase;
            }
        }

        return minCurrentPhase;
    }

    public void AddCurrents(IEnumerable<ElectricalSystemProxy> electricalCircuits, Phase targetPhase) =>
        electricalCircuits.ForEach(n => AddCurrents(n, targetPhase));

    internal void AddCurrents(ElectricalSystemProxy electricalCircuit, Phase targetPhase)
    {
        var powerParameters = electricalCircuit.PowerParameters;
        var estimatedPowerParameters = electricalCircuit.EstimatedPowerParameters;

        switch (powerParameters.PhasesNumber)
        {
            case PhasesNumber.One:
                PhaseCurrents[targetPhase] += estimatedPowerParameters.Current.Round(4);
                break;

            case PhasesNumber.Two:
                AddTwoPhasesCurrents(electricalCircuit, targetPhase);
                break;

            case PhasesNumber.Three when targetPhase == Phase.L123:
                PhaseCurrents[Phase.L1] += estimatedPowerParameters.ThreePhases.EstimateCurrent1.Round(4);
                PhaseCurrents[Phase.L2] += estimatedPowerParameters.ThreePhases.EstimateCurrent2.Round(4);
                PhaseCurrents[Phase.L3] += estimatedPowerParameters.ThreePhases.EstimateCurrent3.Round(4);
                break;
        }

    }

    private void AddTwoPhasesCurrents(ElectricalSystemProxy electricalCircuit, Phase targetPhase)
    {
        var (current1, current2) = RebalanceHelper.GetCurrents(electricalCircuit);

        switch (targetPhase)
        {
            case Phase.L12:
                PhaseCurrents[Phase.L1] += current1.Round(4);
                PhaseCurrents[Phase.L2] += current2.Round(4);
                break;

            case Phase.L23:
                PhaseCurrents[Phase.L2] += current1.Round(4);
                PhaseCurrents[Phase.L3] += current2.Round(4);
                break;

            case Phase.L13:
                PhaseCurrents[Phase.L1] += current1.Round(4);
                PhaseCurrents[Phase.L3] += current2.Round(4);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(PowerParameters.Phase), targetPhase, "Only two phases circuits are allowed.");
        }
    }

    public override string ToString() =>
        $"{PhaseCurrents.Select(n => $"{n.Key} -> {n.Value}").JoinToString(" | ")}";
}

internal class RebalanceHelper
{
    internal static bool IsBalanced(SwitchBoardUnit electricalPanel, double tolerancePercent)
    {
        var currents = electricalPanel.PowerParameters.PhasesNumber == PhasesNumber.Two
            ? GetTwoPhasesCurrents(electricalPanel)
            : GetThreePhasesCurrents(electricalPanel);
        var minEstimateCurrent = currents.Min();
        var maxEstimateCurrent = currents.Max();

        return Math.Abs(1 - minEstimateCurrent / maxEstimateCurrent) < tolerancePercent;
    }

    internal static (double, double) GetCurrents(ElectricalSystemProxy electricalCircuit)
    {
        var powerParameters = electricalCircuit.PowerParameters;

        if (powerParameters.IsTwoPhases == false)
            throw new ArgumentException($"{nameof(electricalCircuit)} is not a two phases circuit.");

        var threePhases = electricalCircuit.EstimatedPowerParameters.ThreePhases;

        return powerParameters.Phase switch
        {
            Phase.L12 => (threePhases.EstimateCurrent1, threePhases.EstimateCurrent2),
            Phase.L23 => (threePhases.EstimateCurrent2, threePhases.EstimateCurrent3),
            Phase.L13 => (threePhases.EstimateCurrent1, threePhases.EstimateCurrent3),
            _ => throw new ArgumentOutOfRangeException(nameof(PowerParameters.Phase), powerParameters.Phase, "Only two phases circuits are allowed.")
        };
    }

    internal static Phase GetNextPhaseInTwoPhasesPanel(SwitchBoardUnit electricalPanel, Phase currentPhase)
    {
        var phase = electricalPanel.PowerParameters?.Phase;

        return phase switch
        {
            Phase.L12 when currentPhase == Phase.L1 => Phase.L2,
            Phase.L23 when currentPhase == Phase.L3 => Phase.L2,
            Phase.L12 when currentPhase == Phase.L2 => Phase.L1,
            Phase.L13 when currentPhase == Phase.L3 => Phase.L1,
            Phase.L13 when currentPhase == Phase.L1 => Phase.L3,
            Phase.L23 when currentPhase == Phase.L2 => Phase.L3,
            _ => throw new ArgumentOutOfRangeException(nameof(phase), phase, $"Not valid {nameof(SwitchBoard)}.{nameof(Phase)} = {phase} or {nameof(currentPhase)} = {currentPhase} is not valid {nameof(Phase)} for {nameof(SwitchBoard)}.{nameof(Phase)} = {phase}")
        };
    }

    internal static List<double> GetTwoPhasesCurrents(SwitchBoardUnit electricalPanel)
    {
        var threePhases = electricalPanel.EstimatedPowerParameters.ThreePhases;

        return electricalPanel.PowerParameters.Phase switch
        {
            Phase.L12 => new List<double> { threePhases.EstimateCurrent1, threePhases.EstimateCurrent2 },
            Phase.L23 => new List<double> { threePhases.EstimateCurrent2, threePhases.EstimateCurrent3 },
            Phase.L13 => new List<double> { threePhases.EstimateCurrent1, threePhases.EstimateCurrent3 },
            _ => throw new ArgumentOutOfRangeException(nameof(PowerParameters.Phase), electricalPanel.PowerParameters.Phase, new ArgumentOutOfRangeException().Message)
        };
    }

    internal static List<double> GetThreePhasesCurrents(SwitchBoardUnit electricalPanel)
    {
        var threePhases = electricalPanel.EstimatedPowerParameters.ThreePhases;
            
        return new List<double>
        {
            threePhases.EstimateCurrent1,
            threePhases.EstimateCurrent2,
            threePhases.EstimateCurrent3
        };
    }

    internal static void MovePhases(IReadOnlyCollection<ElectricalSystemProxy> circuits, ElectricalSystemProxy firstCircuit, Phase oldFirstCircuitPhase)
    {
        var panelPhasesNumber = firstCircuit.GetFirstSourceOf<SwitchBoardUnit>()?.PowerParameters?.PhasesNumber;
            
        if (panelPhasesNumber == PhasesNumber.Two && circuits.Count < 2 ||
            panelPhasesNumber == PhasesNumber.Three && circuits.Count < 3)
            return;

        var newFirstCircuitPhase = firstCircuit.PowerParameters.Phase;
        var phaseDifferent = (oldFirstCircuitPhase - newFirstCircuitPhase) % 3;

        if (phaseDifferent == 0)
            return;
            
        if (phaseDifferent < 0)
            phaseDifferent += 3;
            
        foreach (var circuit in circuits)
            SetNewPhase(circuit, phaseDifferent);
    }

    private static void SetNewPhase(ElectricalSystemProxy circuit, int phaseDifferent)
    {
        var panelPhase = circuit.GetFirstSourceOf<SwitchBoardUnit>()?.PowerParameters?.Phase;
        
        switch (circuit.PowerParameters.Phase)
        {
            case Phase.L1 when phaseDifferent == 1:
            case Phase.L1 when phaseDifferent == 2 && panelPhase == Phase.L12:
            case Phase.L3 when phaseDifferent == 2 || panelPhase == Phase.L23:
                PhaseUtils.SetPhaseAndCalculate(circuit, Phase.L2);
                break;

            case Phase.L2 when phaseDifferent == 1:
            case Phase.L2 when phaseDifferent == 2 && panelPhase == Phase.L23:
            case Phase.L1 when phaseDifferent == 2 || panelPhase == Phase.L13:
                PhaseUtils.SetPhaseAndCalculate(circuit, Phase.L3);
                break;

            case Phase.L3 when phaseDifferent == 1:
            case Phase.L3 when phaseDifferent == 2 && panelPhase == Phase.L13:
            case Phase.L2 when phaseDifferent == 2 || panelPhase == Phase.L12:
                PhaseUtils.SetPhaseAndCalculate(circuit, Phase.L1);
                break;

            case Phase.L12 when phaseDifferent == 1:
            case Phase.L13 when phaseDifferent == 2:
                PhaseUtils.SetPhaseAndCalculate(circuit, Phase.L23);
                break;

            case Phase.L23 when phaseDifferent == 1:
            case Phase.L12 when phaseDifferent == 2:
                PhaseUtils.SetPhaseAndCalculate(circuit, Phase.L13);
                break;
                
            case Phase.L13 when phaseDifferent == 1:
            case Phase.L23 when phaseDifferent == 2:
                PhaseUtils.SetPhaseAndCalculate(circuit, Phase.L12);
                break;
        }
    }
}