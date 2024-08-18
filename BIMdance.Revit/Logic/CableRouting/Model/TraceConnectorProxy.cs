namespace BIMdance.Revit.Logic.CableRouting.Model;

public class TraceConnectorProxy
{
    private readonly Dictionary<TraceElectricalElementProxy, TracePath> _tracePathsToBaseEquipments = new();
    private readonly HashSet<TraceElectricalSystemProxy> _electricalCircuits = new();
    
    public TraceConnectorProxy() { }
    public TraceConnectorProxy(TraceElement element, int id, Point3D point) : this(id, point)
    {
        Element = element;
        Element.AddConnector(this);
    }
    public TraceConnectorProxy(int id, Point3D point) : this()
    {
        Id = id;
        Point = point;
    }

    public TraceElement Element { get; set; }
    public int Id { get; set; }
    public Point3D Point { get; set; }
    public TraceConnectorProxy RefConnector { get; set; }

    public void AddRefConnector(TraceConnectorProxy refConnector)
    {
        this.RefConnector = refConnector;
        refConnector.RefConnector = this;
    }
    
    public void AddTracePathToBaseEquipment(TraceElectricalSystemProxy electricalSystem, TracePath pathToBaseEquipment)
    {
        AddElectricalCircuit(electricalSystem);
        AddTracePathToBaseEquipment(electricalSystem.BaseEquipment, pathToBaseEquipment);
    }
    public void AddTracePathToBaseEquipment(TraceElectricalElementProxy baseEquipment, TracePath pathToBaseEquipment)
    {
        if (IsTraceConnectorToBaseEquipment(baseEquipment))
            return;

        _tracePathsToBaseEquipments.Add(baseEquipment, pathToBaseEquipment);
    }
    public void AddElectricalCircuit(TraceElectricalSystemProxy electricalSystem) => _electricalCircuits.Add(electricalSystem);
    public TracePath GetTracePathToBaseEquipment(TraceElectricalElementProxy baseEquipment) => IsTraceConnectorToBaseEquipment(baseEquipment) ? _tracePathsToBaseEquipments[baseEquipment] : null;

    public bool IsTraceConnectorDirectToBaseEquipment(TraceElectricalElementProxy baseEquipment) => GetTracePathToBaseEquipment(baseEquipment)?.DirectPathToBaseEquipment ?? false;
    public bool IsTraceConnectorToBaseEquipment(TraceElectricalElementProxy baseEquipment) => _tracePathsToBaseEquipments.ContainsKey(baseEquipment);
    public bool IsTraceConnectorOfElectricalCircuit(TraceElectricalSystemProxy electricalSystem) => _electricalCircuits.Contains(electricalSystem);

    public double DistanceTo(TraceConnectorProxy traceConnector)
    {
        return this.Point.DistanceTo(traceConnector.Point);
    }
}