namespace BIMdance.Revit.Model.Electrical.Base;

public abstract class EquipmentUnit : ElectricalEquipmentProxy
{
    public EquipmentSet EquipmentSet { get; set; }
    public List<ConnectorProxy> ReserveConnectors { get; } = new();
    public LeftConnector<EquipmentUnit> LeftConnector { get; set; }
    public RightConnector<EquipmentUnit> RightConnector { get; set; }

    protected EquipmentUnit() :
        this(default(int), default) { }

    protected EquipmentUnit(string name, ElectricalSystemTypeProxy systemType) :
        this(NotCreatedInRevitId, name, systemType) { }

    protected EquipmentUnit(int revitId, string name, ElectricalSystemTypeProxy systemType) :
        this(revitId, name) => SetBaseConnector(new ConnectorProxy(this, 1, systemType));

    protected EquipmentUnit(string name, DistributionSystemProxy distributionSystem) :
        this(NotCreatedInRevitId, name, distributionSystem) { }

    protected EquipmentUnit(int revitId, string name, DistributionSystemProxy distributionSystem, ConnectorProxy connector = null) : this(revitId, name)
    {
        DistributionSystem = distributionSystem;
        SetBaseConnector(connector ?? new ConnectorProxy(this, 1, DistributionSystem));
        PowerParameters.Phase = PowerParameters.PhasesNumber switch
        {
            PhasesNumber.One => Phase.L1,
            PhasesNumber.Two => Phase.L12,
            PhasesNumber.Three => Phase.L123,
            _ => Phase.Undefined
        };
    }

    protected EquipmentUnit(int revitId, string name, PhasesNumber phasesNumber, double lineToGroundVoltage) : this(revitId, name)
    {
        SetBaseConnector(new ConnectorProxy(this, 1, phasesNumber, lineToGroundVoltage));
        PowerParameters.Phase = PowerParameters.PhasesNumber switch
        {
            PhasesNumber.One => Phase.L1,
            PhasesNumber.Two => Phase.L12,
            PhasesNumber.Three => Phase.L123,
            _ => Phase.Undefined
        };
    }

    protected EquipmentUnit(string name) : base(name)
    {
        LeftConnector = new LeftConnector<EquipmentUnit>(this);
        RightConnector = new RightConnector<EquipmentUnit>(this);
    }

    protected EquipmentUnit(int revitId, string name) : base(revitId, name)
    {
        LeftConnector = new LeftConnector<EquipmentUnit>(this);
        RightConnector = new RightConnector<EquipmentUnit>(this);
    }

    public IEnumerable<ConnectorProxy> GetInputConnectors() => new[] { BaseConnector }.Concat(ReserveConnectors);
    public IEnumerable<EquipmentUnit> GetSideConnected(bool includeOwner = false, OperatingMode operatingMode = null)
    {
        return includeOwner
            ? new[] {this}.Concat(SideConnected(operatingMode))
            : SideConnected(operatingMode);
    }

    public IEnumerable<EquipmentUnit> GetLeftConnected(bool includeOwner = false, OperatingMode operatingMode = null)
    {
        return includeOwner
            ? new[] {this}.Concat(LeftConnected(operatingMode))
            : LeftConnected(operatingMode);
    }

    public IEnumerable<EquipmentUnit> GetRightConnected(bool includeOwner = false, OperatingMode operatingMode = null)
    {
        return includeOwner
            ? new[] {this}.Concat(RightConnected(operatingMode))
            : RightConnected(operatingMode);
    }

    private IEnumerable<EquipmentUnit> SideConnected(OperatingMode operatingMode)
    {
        return LeftConnected(operatingMode).Concat(RightConnected(operatingMode));
    }

    private IEnumerable<EquipmentUnit> LeftConnected(OperatingMode operatingMode)
    {
        var connected = GetNextLeftConnected(this, operatingMode);

        return connected != null
            ? new[] { connected }.Concat(connected.LeftConnected(operatingMode))
            : new List<EquipmentUnit>();
    }

    private IEnumerable<EquipmentUnit> RightConnected(OperatingMode operatingMode)
    {
        var connected = GetNextRightConnected(this, operatingMode);
            
        return connected != null
            ? new[] { connected }.Concat(connected.RightConnected(operatingMode))
            : new List<EquipmentUnit>();
    }

    public override ElectricalBase GetConnectedSource(OperatingMode operatingMode = null)
    {
        if (BaseConnector.GetState(operatingMode))
            return BaseConnector.Source;

        foreach (var reserveConnector in ReserveConnectors
                     .Where(x => x?.GetState(operatingMode) ?? false))
            return reserveConnector.Source;

        var leftConnected = GetNextLeftConnected(this, operatingMode);

        while (leftConnected != null)
        {
            if (TryGetSource(leftConnected, operatingMode, out var source))
                return source;

            leftConnected = GetNextLeftConnected(leftConnected, operatingMode);
        }

        var rightConnected = GetNextRightConnected(this, operatingMode);

        while (rightConnected != null)
        {
            if (TryGetSource(rightConnected, operatingMode, out var source))
                return source;

            rightConnected = GetNextRightConnected(rightConnected, operatingMode);
        }

        return null;
    }

    public EquipmentUnit GetNextLeftConnected(OperatingMode operatingMode) => GetNextLeftConnected(this, operatingMode);
    private static EquipmentUnit GetNextLeftConnected(EquipmentUnit equipmentUnit, OperatingMode operatingMode) =>
        equipmentUnit.LeftConnector.IsConnected &&
        equipmentUnit.LeftConnector.GetState(operatingMode)
            ? equipmentUnit.LeftConnector.ReferenceConnector?.Owner
            : null;

    public EquipmentUnit GetNextRightConnected(OperatingMode operatingMode) => GetNextRightConnected(this, operatingMode);
    private static EquipmentUnit GetNextRightConnected(EquipmentUnit equipmentUnit, OperatingMode operatingMode) =>
        equipmentUnit.RightConnector.IsConnected &&
        equipmentUnit.RightConnector.GetState(operatingMode)
            ? equipmentUnit.RightConnector.ReferenceConnector?.Owner
            : null;

    private static bool TryGetSource(EquipmentUnit equipmentUnit, OperatingMode operatingMode,
        out ElectricalBase source)
    {
        source = null;
            
        if (equipmentUnit.BaseConnector.IsConnected &&
            equipmentUnit.BaseConnector.GetState(operatingMode))
        {
            source = equipmentUnit.BaseConnector.Source;
            return true;
        }

        foreach (var reserveConnector in equipmentUnit.ReserveConnectors)
        {
            if (reserveConnector is not { IsConnected: true } ||
                !reserveConnector.GetState(operatingMode))
                continue;
            
            source = reserveConnector.Source;
            return true;
        }

        return false;
    }

    public void SetInputConnectorState(ConnectorProxy inputConnector, bool state, OperatingMode operatingMode)
    {
        var inputConnectors = GetInputConnectors().ToArray();
        
        if (!inputConnectors.Contains(inputConnector))
            return;
        
        inputConnector.SetState(state, operatingMode);

        if (!state)
            return;
        
        inputConnectors.Where(x => x.Id != inputConnector.Id).ForEach(x => x.SetState(false, operatingMode));

        if (GetLeftConnected(includeOwner: false, operatingMode).Any(unit => unit.GetInputConnectors().Any(c => c.GetState(operatingMode))))
            LeftConnector.SetState(false, operatingMode);
            
        if (GetRightConnected(includeOwner: false, operatingMode).Any(unit => unit.GetInputConnectors().Any(c => c.GetState(operatingMode))))
            RightConnector.SetState(false, operatingMode);
    }
    
    public void SetLeftConnectorState(bool state, ConnectorProxy activeInputConnector, OperatingMode operatingMode)
    {
        LeftConnector.SetState(state, operatingMode);

        if (activeInputConnector is null)
            return;

        var allConnectingUnits = GetAllConnectingUnits(operatingMode);
        var changedUnits = new [] { this, GetNextLeftConnected(this, operatingMode) }.ToList();

        foreach (var unit in allConnectingUnits)
        {
            var oldBaseState = unit.BaseConnector.GetState(operatingMode);
            var newBaseState = unit.BaseConnector == activeInputConnector;

            if (newBaseState != oldBaseState)
            {
                if (!changedUnits.Contains(unit))
                    changedUnits.Add(unit);

                unit.BaseConnector.SetState(newBaseState, operatingMode);
            }

            foreach (var reserveConnector in unit.ReserveConnectors)
            {
                var oldReserveState = reserveConnector.GetState(operatingMode);
                var newReserveState = reserveConnector == activeInputConnector;

                if (newReserveState == oldReserveState)
                    continue;

                if (!changedUnits.Contains(unit))
                    changedUnits.Add(unit);

                reserveConnector.SetState(newReserveState, operatingMode);
            }
        }
    }

    public bool IsSupplied(OperatingMode operatingMode) => GetAllConnectingUnits(operatingMode)
            .Any(unit => unit.GetInputConnectors().Any(c => c.GetState(operatingMode)));

    public List<EquipmentUnit> GetAllConnectingUnits(OperatingMode operatingMode)
    {
        var leftUnits = GetLeftConnected(includeOwner: false, operatingMode).Reverse();
        var rightUnits = GetRightConnected(includeOwner: true, operatingMode);
            
        return leftUnits.Union(rightUnits).ToList();
    }
    
    public ConnectorProxy CreateReserveSourceConnector()
    {
        var connector = new ConnectorProxy(
            this, 1 + (ReserveConnectors.Any() ? ReserveConnectors.Max(x => x.Id) : BaseConnector.Id),
            PowerParameters.PhasesNumber, PowerParameters.LineToGroundVoltage,
            defaultState: false, isPrimary: false);

        ReserveConnectors.Add(connector);

        return connector;
    }
}