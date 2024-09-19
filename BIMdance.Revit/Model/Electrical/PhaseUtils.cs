namespace BIMdance.Revit.Model.Electrical;

public static class PhaseUtils
{
    // public List<ElectricalSystemProxy> ChangedCircuits { get; } = new();
        
    public static Phase[] GetPhases(ElectricalBase source, ElectricalBase consumer)
    {
        if (source?.PowerParameters is null)
            return Array.Empty<Phase>();
        
        return (source.PowerParameters.PhasesNumber, consumer.PowerParameters.PhasesNumber,
                source.PowerParameters.Phase) switch
            {
                (PhasesNumber.One, _, _) => new[] { source.PowerParameters.Phase },
                (PhasesNumber.Two, PhasesNumber.One, Phase.L12) => new[] { Phase.L1, Phase.L2 },
                (PhasesNumber.Two, PhasesNumber.One, Phase.L13) => new[] { Phase.L1, Phase.L3 },
                (PhasesNumber.Two, PhasesNumber.One, Phase.L23) => new[] { Phase.L2, Phase.L3 },
                (PhasesNumber.Two, PhasesNumber.Two, _) => new[] { source.PowerParameters.Phase },
                (PhasesNumber.Three, PhasesNumber.One, _) => new[] { Phase.L1, Phase.L2, Phase.L3 },
                (PhasesNumber.Three, PhasesNumber.Two, _) => new[] { Phase.L12, Phase.L13, Phase.L23 },
                (PhasesNumber.Three, PhasesNumber.Three, _) => new[] { Phase.L123 },
                _ => Array.Empty<Phase>()
            };
    }

    public static void SetPhases(SwitchBoardUnit switchBoardUnit)
    {
        if (switchBoardUnit.DistributionSystem?.PhasesNumber != PhasesNumber.Three)
            return;

        var currentPhase = 1;

        foreach (var electricalCircuit in switchBoardUnit.GetConsumersOf<ElectricalSystemProxy>())
        {
            var circuitPowerParameters = electricalCircuit.PowerParameters;

            if (circuitPowerParameters == null)
                return;

            var phase = circuitPowerParameters.PhasesNumber switch
            {
                PhasesNumber.Undefined => Phase.Undefined,
                PhasesNumber.One when circuitPowerParameters.Phase is < Phase.L1 or > Phase.L3 => Phase.Undefined,
                PhasesNumber.Two when circuitPowerParameters.Phase is < Phase.L12 or > Phase.L13 => Phase.Undefined,
                PhasesNumber.Three when circuitPowerParameters.Phase != Phase.L123 => Phase.Undefined,
                _ => circuitPowerParameters.Phase
            };

            if (phase != Phase.Undefined)
                continue;
                
            switch (circuitPowerParameters.PhasesNumber)
            {
                case PhasesNumber.One:
                    SetPhase(electricalCircuit, (Phase)currentPhase);
                    currentPhase = currentPhase % 3 + 1;
                    continue;

                case PhasesNumber.Two:
                    SetPhase(electricalCircuit, (Phase)(currentPhase + 3));
                    currentPhase = currentPhase % 3 + 2;
                    continue;

                case PhasesNumber.Three:
                    continue;

                default:
                    throw new ArgumentOutOfRangeException(nameof(circuitPowerParameters.PhasesNumber), circuitPowerParameters.PhasesNumber, new ArgumentOutOfRangeException().Message);
            }
        }
    }
        
    public static void SetPhase(
        ElectricalBase consumer,
        ElectricalBase source,
        Phase phase = Phase.Undefined) =>
        SetPhase(consumer.BaseConnector, source, phase);

    public static void SetPhase(
        ConnectorProxy consumerConnector,
        ElectricalBase source,
        Phase phase = Phase.Undefined)
    {
        var sourcePhasesNumber = source.PowerParameters.PhasesNumber;
        var sourcePhase = source.PowerParameters.Phase;
        var consumerPhasesNumber = consumerConnector.PowerParameters.PhasesNumber;
        var consumerPhase = consumerConnector.PowerParameters.Phase;
            
        if (sourcePhasesNumber == PhasesNumber.One ||
            sourcePhasesNumber == consumerPhasesNumber)
        {
            SetPhase(consumerConnector, sourcePhase);
        }
        else if (consumerPhase == Phase.Undefined)
        {
            phase = phase != Phase.Undefined ? phase : GetNextPhase(source, consumerPhasesNumber);
            SetPhase(consumerConnector, phase);
        }
    }
        
    public static Phase GetNextPhase(ElectricalBase source, PhasesNumber consumerPhasesNumber)
    {
        var sourcePhase = source.PowerParameters?.Phase;

        if (sourcePhase is null or Phase.Undefined)
            sourcePhase = GetSourcePhase(source);
        
        var previousConsumerPhase = source.Consumers.LastOrDefault(x => x?.PowerParameters?.Phase is not (null or Phase.Undefined or Phase.L123))?.PowerParameters?.Phase; 
        
        return (sourcePhase, consumerPhasesNumber, previousConsumerPhase) switch
        {
            (Phase.L12, PhasesNumber.One, Phase.L1) => Phase.L2,
            (Phase.L12, PhasesNumber.One, _) => Phase.L1,
            (Phase.L12, _, _) => source.PowerParameters?.Phase ?? Phase.L12,

            (Phase.L23, PhasesNumber.One, Phase.L2) => Phase.L3,
            (Phase.L23, PhasesNumber.One, _) => Phase.L2,
            (Phase.L23, _, _) => source.PowerParameters?.Phase ?? Phase.L23,

            (Phase.L13, PhasesNumber.One, Phase.L3) => Phase.L1,
            (Phase.L13, PhasesNumber.One, _) => Phase.L3,
            (Phase.L13, _, _) => source.PowerParameters?.Phase ?? Phase.L13,

            (Phase.L123, PhasesNumber.One, Phase.L1 or Phase.L13) => Phase.L2,
            (Phase.L123, PhasesNumber.One, Phase.L2 or Phase.L12) => Phase.L3,
            (Phase.L123, PhasesNumber.One, _) => Phase.L1,

            (Phase.L123, PhasesNumber.Two, Phase.L1 or Phase.L13) => Phase.L23,
            (Phase.L123, PhasesNumber.Two, Phase.L2 or Phase.L12) => Phase.L13,
            (Phase.L123, PhasesNumber.Two, _) => Phase.L12,

            _ => source.PowerParameters?.Phase ?? Phase.L123
        };
    }

    public static Phase GetSourcePhase(ElectricalBase electrical) => GetSourcePhase(electrical.BaseConnector);

    public static Phase GetSourcePhase(ConnectorProxy connector)
    {
        var source = connector.Source;
        
        while (source is not null)
        {
            if (source.PowerParameters?.Phase is not (null or Phase.Undefined))
                return source.PowerParameters.Phase;
            
            source = source.BaseSource;
        }

        return Phase.Undefined;
    }
    
    
    public static Phase GetDefaultPhase(ElectricalBase electrical) => electrical.PowerParameters.PhasesNumber switch
    {
        PhasesNumber.One => Phase.L1,
        PhasesNumber.Two => Phase.L12,
        _ => Phase.L123
    };

    public static void SetPhaseAndCalculate(ElectricalBase electrical, Phase value)
    {
        SetPhase(electrical, value);
    }

    public static void SetPhase(ConnectorProxy connector, Phase value)
    {
        if (connector.PowerParameters.Phase == value)
            return;
            
        connector.PowerParameters.Phase = value;

        if (connector.Owner is ElectricalSystemProxy circuit &&
            connector.PowerParameters.IsThreePhases == false)
            UpdateChildCircuitPhase(circuit, value);
    }
        
    public static void SetPhase(ElectricalBase electrical, Phase value)
    {
        if (electrical.PowerParameters.Phase == value)
            return;
            
        electrical.PowerParameters.Phase = value;

        if (electrical is not ElectricalSystemProxy circuit ||
            electrical.PowerParameters.IsThreePhases)
            return;
            
        // ChangedCircuits.Add(circuit);
        UpdateChildCircuitPhase(circuit, value);
    }

    public static void UpdateChildCircuitPhase(ElectricalSystemProxy electricalSystem, Phase phase)
    {
        switch (electricalSystem.PowerParameters?.PhasesNumber)
        {
            case PhasesNumber.One:
                SetSamePhaseChildren(electricalSystem, phase);
                break;

            case PhasesNumber.Two:
                SetOneAndTwoPhasesChildren(electricalSystem, phase);
                break;

            case PhasesNumber.Three:
                return;
        }
    }

    private static void SetSamePhaseChildren(ElectricalBase electricalCircuit, Phase phase)
    {
        foreach (var childCircuit in electricalCircuit.GetConsumersOf<ElectricalSystemProxy>())
        {
            SetPhase(childCircuit, phase);
        }
    }

    private static void SetOneAndTwoPhasesChildren(ElectricalSystemProxy electricalSystem, Phase phase)
    {
        SetOnePhaseChildrenInTwoPhasesCircuit(electricalSystem, phase);
        SetTwoPhasesChildrenInTwoPhasesCircuit(electricalSystem, phase);
    }

    private static void SetOnePhaseChildrenInTwoPhasesCircuit(ElectricalSystemProxy electricalSystem, Phase phase)
    {
        var onePhaseCircuits = electricalSystem.GetConsumersOf<ElectricalSystemProxy>().Where(n => n.PowerParameters.IsOnePhase)
            .ToList();

        foreach (var onePhaseCircuit in onePhaseCircuits)
            SetPhaseInTwoPhasesCircuit(onePhaseCircuit, phase);
    }

    private static void SetTwoPhasesChildrenInTwoPhasesCircuit(ElectricalSystemProxy electricalSystem, Phase phase)
    {
        var twoPhasesCircuits = electricalSystem.GetConsumersOf<ElectricalSystemProxy>().Where(n => n.PowerParameters.IsTwoPhases);

        foreach (var twoPhasesCircuit in twoPhasesCircuits)
        {
            SetPhase(twoPhasesCircuit, phase);
        }
    }

    private static void SetPhaseInTwoPhasesCircuit(ElectricalSystemProxy onePhaseSystem, Phase phase)
    {
        switch (phase, onePhaseSystem.PowerParameters.Phase)
        {
            case (Phase.L12, Phase.L3):
                SetPhase(onePhaseSystem, Phase.L1);
                return;

            case (Phase.L23, Phase.L1):
                SetPhase(onePhaseSystem, Phase.L2);
                return;

            case (Phase.L13, Phase.L2):
                SetPhase(onePhaseSystem, Phase.L3);
                return;
        }
    }
}