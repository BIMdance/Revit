namespace BIMdance.Revit.Model.RevitProxy;

public class ConnectorProxy
{
    private readonly Dictionary<ElectricalElementProxy, TracePath> _tracePathsToBaseEquipments = new();
    private readonly HashSet<ElectricalSystemProxy> _electricalCircuits = new();
    
    public ConnectorProxy() { }
    public ConnectorProxy(TraceElement element, int id, Point3D point) : this(id, point)
    {
        Element = element;
        Element.AddConnector(this);
    }
    public ConnectorProxy(int id, Point3D point) : this()
    {
        Id = id;
        Point = point;
    }

    public TraceElement Element { get; set; }
    public int Id { get; set; }
    public Point3D Point { get; set; }
    public ConnectorProxy RefConnector { get; set; }

    public void AddRefConnector(ConnectorProxy refConnector)
    {
        this.RefConnector = refConnector;
        refConnector.RefConnector = this;
    }
    
    public void AddTracePathToBaseEquipment(ElectricalSystemProxy electricalSystem, TracePath pathToBaseEquipment)
    {
        AddElectricalCircuit(electricalSystem);
        AddTracePathToBaseEquipment(electricalSystem.BaseEquipment, pathToBaseEquipment);
    }
    public void AddTracePathToBaseEquipment(ElectricalElementProxy baseEquipment, TracePath pathToBaseEquipment)
    {
        if (IsTraceConnectorToBaseEquipment(baseEquipment))
            return;

        _tracePathsToBaseEquipments.Add(baseEquipment, pathToBaseEquipment);
    }
    public void AddElectricalCircuit(ElectricalSystemProxy electricalSystem) => _electricalCircuits.Add(electricalSystem);
    public TracePath GetTracePathToBaseEquipment(ElectricalElementProxy baseEquipment) => IsTraceConnectorToBaseEquipment(baseEquipment) ? _tracePathsToBaseEquipments[baseEquipment] : null;

    public bool IsTraceConnectorDirectToBaseEquipment(ElectricalElementProxy baseEquipment) => GetTracePathToBaseEquipment(baseEquipment)?.DirectPathToBaseEquipment ?? false;
    public bool IsTraceConnectorToBaseEquipment(ElectricalElementProxy baseEquipment) => _tracePathsToBaseEquipments.ContainsKey(baseEquipment);
    public bool IsTraceConnectorOfElectricalCircuit(ElectricalSystemProxy electricalSystem) => _electricalCircuits.Contains(electricalSystem);

    public double DistanceTo(ConnectorProxy traceConnector)
    {
        return this.Point.DistanceTo(traceConnector.Point);
    }
}