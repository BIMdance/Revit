using StringComparer = BIMdance.Revit.Utils.Common.StringComparer;

namespace BIMdance.Revit.Model.RevitProxy;

public sealed class ElectricalSystemProxy : ElectricalBase,
    IPrototype<ElectricalSystemProxy>,
    IPropertyPrototype<ElectricalSystemProxy>
{
    private double _cableLength;
    private double _cableLengthInCableTray;
    private double _cableLengthMax;
        
    public ElectricalSystemProxy() : this(NotCreatedInRevitId) { }
    internal ElectricalSystemProxy(int revitId, string circuitNumber = null) : this(revitId, circuitNumber, ElectricalSystemTypeProxy.UndefinedSystemType) { }
    internal ElectricalSystemProxy(int revitId, string circuitNumber, ElectricalSystemTypeProxy systemType, CircuitTypeProxy circuitType = CircuitTypeProxy.Circuit) : base(revitId, circuitNumber)
    {
        CircuitNumber = circuitNumber;
        CircuitProducts = new CircuitProducts(this);
        CircuitType = circuitType;
        SetBaseConnector(new ConnectorProxy(this, 1, systemType));
        
        if (systemType is ElectricalSystemTypeProxy.PowerCircuit)
            CircuitPowerParameters = new CircuitPowerParameters(this);
    }
    internal ElectricalSystemProxy(int revitId, string circuitNumber, DistributionSystemProxy distributionSystem, CircuitTypeProxy circuitType = CircuitTypeProxy.Circuit) : base(revitId, circuitNumber)
    {
        CircuitNumber = circuitNumber;
        CircuitProducts = new CircuitProducts(this);
        CircuitType = circuitType;
        SetBaseConnector(new ConnectorProxy(this, 1, distributionSystem));
        CircuitPowerParameters = new CircuitPowerParameters(this);
    }

    private ElectricalSystemProxy(ElectricalSystemProxy prototype)
    {
        Cabling = prototype.Cabling.Clone();
        CircuitProducts = new CircuitProducts(this);
        Rooms = prototype.Rooms;

        PullProperties(prototype);
    }

    public ElectricalSystemProxy Clone() => new(prototype: this);

    public void PullProperties(ElectricalSystemProxy prototype)
    {
        SetBaseConnector(prototype.BaseConnector);
        Cabling.PullProperties(prototype.Cabling);
        CircuitType = prototype.CircuitType;
        CircuitProducts.PullProperties(prototype.CircuitProducts);
        Topology = prototype.Topology;
        CircuitPowerParameters = new CircuitPowerParameters(this);
        CircuitPowerParameters.SetReducingFactor(prototype.CircuitPowerParameters.ReducingFactor);
        // this.Power.PullProperties(prototype.Power);
    }

    public CircuitPowerParameters CircuitPowerParameters { get; set; }
    public Cabling Cabling { get; } = new();
    public CircuitProducts CircuitProducts { get; }
    public override ElectricalProducts Products => CircuitProducts;
    public CircuitTypeProxy CircuitType { get; set; } = CircuitTypeProxy.Circuit;
    public ConnectionTopology Topology { get; set; } = ConnectionTopology.Tree;
    public IList<RoomProxy> Rooms { get; } = new List<RoomProxy>();
    public string CircuitNumber { get; set; }
    public string CircuitDesignation { get; set; }
    public string CableDesignation { get; set; }
    public string LoadName { get; set; }
    public bool IsGroupCircuit { get; set; }

    public double CableLength
    {
        get => _cableLength;
        set => _cableLength = Math.Round(value);
    }

    public double CableLengthMax
    {
        get => _cableLengthMax;
        set => _cableLengthMax = Math.Round(value);
    }

    public double CableLengthInCableTray
    {
        get => _cableLengthInCableTray;
        set => _cableLengthInCableTray = Math.Round(value);
    }

    public double CableLengthOutsideCableTray { get; set; }
    public string CableMounting { get; set; }
    public bool LockCableLength { get; set; }

    public string RoomsAsString => string.Join(", ", Rooms.Select(n => n.Number).OrderBy(n => n, new StringComparer()));

    // TODO : # | Petrov | Исправить зависание при соединении в кольцо
        
    public override string ToString() =>
        $"[{RevitId}] <{GetType().Name}> {Name} : {ConsumerConnectors.Count} elements";

    public IEnumerable<ElectricalElementProxy> GetConsumers() => GetConsumersOf<ElectricalElementProxy>();

    private void AddRoom(ElectricalElementProxy electricalElement)
    {
        var room = electricalElement.Room;

        if (room == null)
            return;

        if (false == Rooms.Contains(room, new RevitProxyComparer<RoomProxy>()))
            Rooms.Add(room);
    }

    private void RemoveRoom(ElectricalElementProxy electricalElement)
    {
        var room = electricalElement.Room;

        if (room == null)
            return;

        if (Rooms.Count(n => n.RevitId == room.RevitId) == 1)
            Rooms.Remove(room);
    }
}