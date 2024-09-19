namespace BIMdance.Revit.Model.Electrical;

public static class ConnectorExtension
{
    public static ElectricalBase ConnectTo(this SwitchBoard switchBoard, ElectricalBase newBase)
    {
        return switchBoard.FirstUnit.BaseConnector.ConnectTo(newBase);
    }
    
    public static ElectricalBase ConnectTo(this SwitchBoard switchBoard, SwitchBoard newBase)
    {
        return switchBoard.FirstUnit.BaseConnector.ConnectTo(newBase);
    }
    
    public static ElectricalBase ConnectTo(this SwitchBoard switchBoard, int unitIndex, ElectricalBase newBase)
    {
        if (switchBoard.SpecificUnits.Count <= unitIndex)
            throw new IndexOutOfRangeException($"switchBoard.Units.Count: {switchBoard.SpecificUnits.Count} but unitIndex: {unitIndex}");
        
        return switchBoard.SpecificUnits[unitIndex].BaseConnector.ConnectTo(newBase);
    }
        
    public static ElectricalBase ConnectTo(this ElectricalBase electrical, ElectricalBase newBase)
    {
        return electrical.BaseConnector.ConnectTo(newBase);
    }
        
    public static ElectricalBase ConnectTo(this ElectricalBase electrical, SwitchBoard newBase)
    {
        return electrical.BaseConnector.ConnectTo(newBase);
    }
        
    public static ElectricalBase ConnectTo(this ConnectorProxy connector, SwitchBoard newBase)
    {
        return connector.ConnectTo(newBase.FirstUnit);
    }
        
    public static ElectricalBase ConnectTo(this ConnectorProxy connector, ElectricalBase newBase, Phase phase = Phase.Undefined)
    {
        var baseConnector = newBase.BaseConnector;

        if (!connector.IsCompatible(baseConnector))
            return newBase;
        
        if (connector.IsConnected)
            connector.Disconnect();

        connector.Source = newBase;
        newBase.ConsumerConnectors.Add(connector);

        if (connector.IsPower)
            PhaseUtils.SetPhase(connector, newBase, phase);

        return newBase;
    }

    public static ElectricalBase ConnectTo(this ConnectorProxy connector, ElectricalBase newBase, int index)
    {
        var baseConnector = newBase.BaseConnector;
            
        if (!connector.IsCompatible(baseConnector))
            return newBase;
            
        if (connector.IsConnected)
        {
            var oldBase = connector.Source;
            oldBase?.ConsumerConnectors.Remove(connector);
        }
        else
        {
            connector.Source = newBase;
            newBase.ConsumerConnectors.Insert(index, connector);

            if (connector.IsPower && connector.Owner is ElectricalSystemProxy)
            {
                PhaseUtils.SetPhase(connector, newBase);
            }
        }

        return newBase;
    }
        
    public static void Disconnect(this ElectricalBase electrical)
    {
        Disconnect(electrical.BaseConnector);
    }
        
    public static void Disconnect(this ConnectorProxy connector)
    {
        var oldBase = connector.Source;
        oldBase?.ConsumerConnectors.Remove(connector);

        connector.Source = null;
    }
}