using BIMdance.Revit.Model.Context;

namespace BIMdance.Revit.Model.Electrical;

public class ConnectorOperatingModes : Dictionary<OperatingMode, ConnectorOperatingMode>
{
    private static ElectricalContext ElectricalContext => Locator.Get<ElectricalContext>();
    
    private readonly ConnectorProxy _connector;
    private readonly bool _defaultState;
        
    public ConnectorOperatingModes() { }
    public ConnectorOperatingModes(ConnectorProxy connector, bool defaultState)
    {
        _connector = connector;
        _defaultState = defaultState;
    }

    public EstimatedPowerParameters GetEstimatedPowerParameters(OperatingMode operatingMode)
    {
        operatingMode ??= ElectricalContext.CurrentOperatingMode;

        return TryGetValue(operatingMode, out var value)
            ? value.EstimatedPowerParameters
            : CreateConnectorOperatingMode(operatingMode).EstimatedPowerParameters;
    }

    public bool GetState(OperatingMode operatingMode = null)
    {
        operatingMode ??= Locator.Get<ElectricalContext>().CurrentOperatingMode;

        return TryGetValue(operatingMode, out var value)
            ? value.State
            : CreateConnectorOperatingMode(operatingMode).State;
    }
        
    public void SetState(bool newState, params OperatingMode[] operatingModes)
    {
        foreach (var operatingMode in operatingModes)
        {
            SetState(newState, operatingMode);
        }
    }

    private void SetState(bool newState, OperatingMode operatingMode)
    {
        if (TryGetValue(operatingMode, out var connectorOperatingMode))
        {
            connectorOperatingMode.State = newState;
        }
        else
        {
            CreateConnectorOperatingMode(operatingMode).State = newState;
        }
    }
        
    public void SetAllStates(bool newState)
    {
        foreach (var value in Values)
            value.State = newState;
    }

    private ConnectorOperatingMode CreateConnectorOperatingMode(OperatingMode operatingMode)
    {
        var newEstimatedPowerParameters = _connector.IsPower
            ? new EstimatedPowerParameters(_connector.PowerParameters.PhasesNumber, _connector.PowerParameters.LineToGroundVoltage)
            : null;
            
        var newConnectorOperatingMode = new ConnectorOperatingMode(operatingMode, newEstimatedPowerParameters)
        {
            State = _defaultState,
        };

        Add(operatingMode, newConnectorOperatingMode);
            
        return newConnectorOperatingMode;
    }
}