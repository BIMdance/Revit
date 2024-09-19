namespace BIMdance.Revit.Model.Electrical.Base;

public abstract class ElectricalBase : ElementProxy
{
    protected ElectricalBase() { }
    protected ElectricalBase(int revitId, string name) : base(revitId, name) { }
    public ElectricalBase BaseSource => BaseConnector.Source;
    public IEnumerable<ElectricalBase> Consumers => ConsumerConnectors.Select(n => n.Owner);
    public ConnectorProxy BaseConnector { get; private set; }
    public List<ConnectorProxy> ConsumerConnectors { get; } = new();
    public ElectricalSystemTypeProxy SystemType => BaseConnector.SystemType;
    public PowerParameters PowerParameters => BaseConnector.PowerParameters;
    public EstimatedPowerParameters EstimatedPowerParameters => GetEstimatedPowerParameters();
    public virtual ElectricalProducts Products { get; } = new();
    public string SchemeNotes { get; set; }
    public double CableReserve { get; set; }
    public bool IsPlacedInModel => RevitId > 0;

    public void SetBaseConnector(ConnectorProxy connector)
    {
        BaseConnector = connector;
        BaseConnector.Owner = this;
    }
    
    public virtual ElectricalBase GetConnectedSource(OperatingMode operatingMode = null)
    {
        return BaseConnector.GetState(operatingMode) ? BaseConnector.Source : null;
    }
        
    public bool GetState(OperatingMode operatingMode = null)
    {
        return BaseConnector?.GetState(operatingMode) ?? false;
    }

    public EstimatedPowerParameters GetEstimatedPowerParameters(OperatingMode operatingMode = null)
    {
        return BaseConnector?.GetEstimatedPowerParameters(operatingMode);
    }

    public bool IsPower => BaseConnector.IsPower;
        
    public T GetFirstSourceOf<T>() where T : ElectricalBase
    {
        var source = this.BaseSource;

        while (source != null)
        {
            if (source is T t)
                return t;

            source = source.BaseSource;
        }

        return default;
    }
        
    public T GetConsumerAt<T>(int index)  where T : ElectricalBase
    {
        var consumers = Consumers.ToList();
        return 0 <= index && index < consumers.Count ? consumers[index] as T : null;
    }
        
    public List<ElectricalBase> GetAllSources()
    {
        var sources = new List<ElectricalBase>();
        var source = this.BaseSource;

        while (source != null)
        {
            sources.Add(source);
            source = source.BaseSource;
        }

        return sources;
    }

    public List<T> GetSourceChainOf<T>(ElectricalBase to = null) where T : ElectricalBase
    {
        var sources = new List<T>();
        var source = this.BaseSource;

        while (source != null)
        {
            if (source is T t)
                sources.Add(t);

            if (source.Equals(to))
                break;

            source = source.BaseSource;
        }

        return sources;
    }

    public IEnumerable<ElectricalBase> GetAllConsumers()
    {
        return Consumers.Concat(Consumers.SelectMany(n => n.GetAllConsumers()));
    }

    public IEnumerable<T> GetAllConsumersOf<T>() where T : ElectricalBase
    {
        return GetConsumersOf<T>().Concat(Consumers.SelectMany(n => n.GetAllConsumersOf<T>()));
    }

    public IEnumerable<T> GetConsumersOf<T>() where T : ElectricalBase
    {
        return this.Consumers.OfType<T>();
    }

    public List<T> GetFirstConsumersOf<T>() where T : ElectricalBase
    {
        return GetFirstConsumersOf<T>(this);
    }
        
    private static List<T> GetFirstConsumersOf<T>(ElectricalBase electrical) where T : ElectricalBase
    {
        var consumers = electrical.Consumers.OfType<T>().ToList();

        if (consumers.Any())
            return consumers;

        var firstConsumers = new List<T>();

        foreach (var consumer in electrical.Consumers)
            firstConsumers.AddRange(consumer.Consumers.OfType<T>());

        if (firstConsumers.Any())
            return firstConsumers;
                
        foreach (var consumer in electrical.Consumers)
            firstConsumers.AddRange(GetFirstConsumersOf<T>(consumer));

        return firstConsumers;
    }

    public virtual void Add(ElectricalBase electrical)
    {
        electrical.BaseConnector.ConnectTo(this);
    }

    public virtual void AddRange(IEnumerable<ElectricalBase> electricals)
    {
        foreach (var element in electricals)
        {
            Add(element);
        }
    }

    public virtual void Insert(int index, ElectricalBase electrical)
    {
        electrical.BaseConnector.ConnectTo(this, index);
    }

    public virtual void Remove(ElectricalBase electrical)
    {
        electrical.BaseConnector.Disconnect();
    }

    public virtual void Move(ElectricalBase electrical1, ElectricalBase electrical2)
    {
        var connector1 = electrical1.BaseConnector;
        var connector2 = electrical2.BaseConnector;
            
        var index1 = this.ConsumerConnectors.IndexOf(connector1);
        var index2 = this.ConsumerConnectors.IndexOf(connector2);

        this.ConsumerConnectors.RemoveAt(index1);
        this.ConsumerConnectors.Insert(index1, connector2);

        this.ConsumerConnectors.RemoveAt(index2);
        this.ConsumerConnectors.Insert(index2, connector1);
    }

    public T ParentAs<T>() where T : ElectricalBase => this.BaseSource as T;

    // public IElectricalWarnings ElectricalWarnings { get; }
    // public PowerElectrical Power { get; }
}

// public interface ElectricalBase : IElementProxy, INode
// {
//     ConnectorProxy SourceConnector { get; }
//     List<ConnectorProxy> ConsumerConnectors { get; }
//     ElectricalSystemTypeProxy ElectricalSystemType { get; }
//     T As<T>() where T : class, IElectrical;
//     
//     T ParentAs<T>() where T : class, IElectrical;
//     bool Is<T>() where T : class, IElectrical;
//     bool Is<T>(out T downcast) where T : class, IElectrical;
//     bool IsNot<T>() where T : class, IElectrical;
//     IElectricalWarnings ElectricalWarnings { get; }
//     PowerElectrical Power { get; }
//     
//     // TODO 2021.04.23
//     // ElectricalDeviceContainer Devices { get; }
//     // ElectricalProducts Products { get; }
//     
//     bool IsPower { get; }
//     string SchemeNotes { get; set; }
// }
//