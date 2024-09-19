namespace BIMdance.Revit.Model.Electrical.Base;

public abstract class ElectricalEquipmentProxy : ElectricalBase
{
    protected ElectricalEquipmentProxy() { }
    protected ElectricalEquipmentProxy(string name) :
        base(NotCreatedInRevitId, name) { }
    protected ElectricalEquipmentProxy(int revitId, string name) :
        base(revitId, name) { }

    public Function[] Functions { get; set; } = Array.Empty<Function>();
    public BuiltInCategoryProxy Category { get; set; }
    public DistributionSystemProxy DistributionSystem { get; set; }
    public IBus Bus { get; set; }

    public void SetDistributionSystem(DistributionSystemProxy distributionSystem)
    {
        DistributionSystem = distributionSystem;

        if (DistributionSystem == null)
        {
            UpdateBaseConnectorVoltage();
            return;
        }
        
        if (BaseConnector == null)
            SetBaseConnector(new ConnectorProxy(this, 1, DistributionSystem));
        
        if (BaseConnector is { IsPower: false })
            BaseConnector.CreatePowerParameters(distributionSystem);
        else
            UpdateBaseConnectorVoltage();
    }

    public void UpdateBaseConnectorVoltage()
    {
        if (BaseConnector?.PowerParameters == null)
            return;
        
        BaseConnector.PowerParameters.PhasesNumber = DistributionSystem?.PhasesNumber ?? PhasesNumber.Undefined;
        BaseConnector.PowerParameters.LineToGroundVoltage = DistributionSystem?.GetLineToGroundVoltage() ?? 0d;
    }
}

public abstract class ElectricalEquipment<TElectricalEquipment> : ElectricalEquipmentProxy, IPrototype<TElectricalEquipment>, IPropertyPrototype<TElectricalEquipment>
    where TElectricalEquipment : ElectricalBase
{
    protected ElectricalEquipment() { }
    protected ElectricalEquipment(int revitId, string name) : base(revitId, name) { }

    public abstract TElectricalEquipment Clone();
    public abstract void PullProperties(TElectricalEquipment prototype);
    public LeftConnector<TElectricalEquipment> LeftConnector { get; set; }
    public RightConnector<TElectricalEquipment> RightConnector { get; set; }
}