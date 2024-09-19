using BIMdance.Revit.Model.Electrical;
using BIMdance.Revit.Model.Electrical.Base;

namespace BIMdance.Revit.Model.RevitProxy;

public class ConnectorProxy
{
    public ConnectorProxy() { }
    
    public ConnectorProxy(
        ElectricalBase owner, int id,
        ElectricalSystemTypeProxy systemType,
        Point3D origin = null,
        bool defaultState = true,
        bool isPrimary = true)
    {
        Owner = owner;
        Id = id;
        SystemType = systemType;
        Origin = origin ?? new Point3D();
        IsPrimary = isPrimary;
        ConnectorOperatingModes = new ConnectorOperatingModes(this, defaultState);
    }

    public ConnectorProxy(
        ElectricalBase owner, int id,
        DistributionSystemProxy distributionSystem,
        Point3D origin = null, bool defaultState = true, bool isPrimary = true) :
        this(owner, id, distributionSystem.PhasesNumber, distributionSystem.GetLineToGroundVoltage(),
            origin, defaultState, isPrimary) { }

    public ConnectorProxy(
        ElectricalBase owner, int id,
        PhasesNumber phasesNumber, double lineToGroundVoltage,
        Point3D origin = null,
        bool defaultState = true,
        bool isPrimary = true)
    {
        Owner = owner;
        Id = id;
        SystemType = ElectricalSystemTypeProxy.PowerCircuit;
        CreatePowerParameters(phasesNumber, lineToGroundVoltage);
        Origin = origin ?? new Point3D();
        IsPrimary = isPrimary;
        ConnectorOperatingModes = new ConnectorOperatingModes(this, defaultState);
    }

    public ConnectorOperatingModes ConnectorOperatingModes { get; set; }
    public ElectricalBase Source { get; set; }
    public ElectricalBase Owner { get; set; }
    public ElectricalSystemTypeProxy SystemType { get; set; }
    public int Id { get; }
    public bool IsPower => PowerParameters is not null;
    public bool IsConnected => Source is not null;
    public bool IsPrimary { get; set; }
    public Point3D Origin { get; set; }
    public PowerParameters PowerParameters { get; set; }

    public PowerParameters CreatePowerParameters(DistributionSystemProxy distributionSystem)
    {
        return CreatePowerParameters(distributionSystem.PhasesNumber, distributionSystem.GetLineToGroundVoltage());
    }
        
    public PowerParameters CreatePowerParameters(PhasesNumber phasesNumber, double lineToGroundVoltage)
    {
        return PowerParameters ??= new PowerParameters(phasesNumber, lineToGroundVoltage);
    }
        
    public EstimatedPowerParameters GetEstimatedPowerParameters(OperatingMode operatingMode = null)
    {
        return ConnectorOperatingModes.GetEstimatedPowerParameters(operatingMode);
    }
        
    public bool GetState(OperatingMode operatingMode = null)
    {
        return ConnectorOperatingModes.GetState(operatingMode);
    }

    public void SetState(bool newState, params OperatingMode[] operatingModes)
    {
        ConnectorOperatingModes.SetState(newState, operatingModes);
    }
        
    public void SetStateAllOperatingModes(bool newState)
    {
        ConnectorOperatingModes.SetAllStates(newState);
    }

    public bool IsCompatible(ConnectorProxy otherConnector)
    {
        return IsPower == otherConnector.IsPower || SystemType == otherConnector.SystemType;
        // throw new InvalidOperationException($"{Owner}: {nameof(ElectricalBase.BaseConnector)}.{nameof(IsPower)} = {IsPower} but\n{otherConnector.Owner}: {nameof(ElectricalBase.BaseConnector)}.{nameof(IsPower)} = {otherConnector.IsPower}");
        // throw new InvalidOperationException($"{Owner}: {nameof(ElectricalBase.BaseConnector)}.{nameof(SystemType)} = {SystemType} but\n{otherConnector.Owner}: {nameof(ElectricalBase.BaseConnector)}.{nameof(SystemType)} = {otherConnector.SystemType}");
    }
}

public class ConnectorOperatingMode
{
    public ConnectorOperatingMode()
    {
        EstimatedPowerParameters = new EstimatedPowerParameters();
    }
    public ConnectorOperatingMode(OperatingMode operatingMode, EstimatedPowerParameters estimatedPowerParameters = null)
    {
        OperatingMode = operatingMode;
        EstimatedPowerParameters = estimatedPowerParameters;
    }
    public OperatingMode OperatingMode { get; }
    public bool State { get; set; } = true;
    public EstimatedPowerParameters EstimatedPowerParameters { get; }
}