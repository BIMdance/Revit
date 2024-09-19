namespace BIMdance.Revit.Model.Electrical.Base;

public abstract class InternalConnector<TOwner> : ElementProxy
    where TOwner : ElectricalBase
{
    protected InternalConnector() { }
    protected InternalConnector(TOwner owner)
    {
        Owner = owner;
        ElectricalConnector = new ConnectorProxy(
            Owner, 1, ElectricalSystemTypeProxy.UndefinedSystemType,
            defaultState: false);
    }

    public TOwner Owner { get; set; }
    public InternalConnector<TOwner> ReferenceConnector { get; set; }
    public ConnectorProxy ElectricalConnector { get; set; }
    public bool IsConnected => ReferenceConnector != null;

    public bool GetState(OperatingMode operatingMode = null)
    {
        return IsConnected && ElectricalConnector.GetState(operatingMode);
    }

    public void SetState(bool newState, params OperatingMode[] operatingModes)
    {
        if (!IsConnected)
            throw new InvalidOperationException($"Connector must be connected.");
            
        ElectricalConnector.SetState(newState, operatingModes);
        ReferenceConnector.ElectricalConnector.SetState(newState, operatingModes);
    }

    public void SetAllStates(bool newState)
    {
        if (!IsConnected)
            throw new InvalidOperationException($"Connector must be connected.");
            
        ElectricalConnector.SetStateAllOperatingModes(newState);
        ReferenceConnector.ElectricalConnector.SetStateAllOperatingModes(newState);
    }
        
    public void Disconnect()
    {
        if (ReferenceConnector == null)
            return;
            
        ReferenceConnector.ReferenceConnector = default;
        ReferenceConnector = default;
    }

    public static T CloneFrom<T>(T prototype) where T : new() =>
        prototype != null ? new T() : default;
}

public abstract class InternalConnector<TOwner, TReferenceConnector> : InternalConnector<TOwner>
    where TOwner : ElectricalBase
{
    protected InternalConnector() { }
    protected InternalConnector(TOwner owner) : base(owner) { }
    public abstract void ConnectTo(TReferenceConnector otherConnector);
}

public class LeftConnector<TOwner> : InternalConnector<TOwner, RightConnector<TOwner>>
    where TOwner : ElectricalBase
{
    public LeftConnector() { }
    public LeftConnector(TOwner owner) : base(owner) { }
    public override void ConnectTo(RightConnector<TOwner> otherConnector)
    {
        Disconnect();
        
        ReferenceConnector = otherConnector;

        if (otherConnector != null)
            otherConnector.ReferenceConnector = this;
    }
}

public class RightConnector<TOwner> : InternalConnector<TOwner, LeftConnector<TOwner>>
    where TOwner : ElectricalBase
{
    public RightConnector() { }
    public RightConnector(TOwner owner) : base(owner) { }
    public override void ConnectTo(LeftConnector<TOwner> otherConnector)
    {
        Disconnect();
        
        ReferenceConnector = otherConnector;
            
        if (otherConnector != null)
            otherConnector.ReferenceConnector = this;
    }
}