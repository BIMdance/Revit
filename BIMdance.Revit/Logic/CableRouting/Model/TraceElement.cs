namespace BIMdance.Revit.Logic.CableRouting.Model;

public abstract class TraceElement : ElementProxy
{
    private readonly List<TraceConnectorProxy> _connectors = new();
    
    protected TraceElement() { }
    protected TraceElement(int revitId) : base(revitId) { }
    public TraceNetwork TraceNetwork { get; set; }
    public IEnumerable<TraceConnectorProxy> Connectors => _connectors;
    public int ConnectorsCount => _connectors.Count;
    public List<TraceElectricalSystemProxy> ElectricalSystems { get; } = new();
    public string CableTrace { get; set; }

    public void AddConnector(TraceConnectorProxy traceConnector)
    {
        if (_connectors.Any(n => n.Id == traceConnector.Id))
            return;
        
        _connectors.Add(traceConnector);
        traceConnector.Element = this;
    }
    
    public bool ContainsConnector(int id) => _connectors.Select(n => n.Id).Contains(id);

    public TraceConnectorProxy GetNextConnector(TraceConnectorProxy traceConnector)
    {
        return _connectors.FirstOrDefault(n => n.Id > traceConnector.Id) ??
               _connectors.FirstOrDefault(n => n.Id < traceConnector.Id);
    }
    
    public TraceConnectorProxy GetTraceConnectorToBaseEquipment(TraceElectricalElementProxy baseEquipment) =>
        _connectors.FirstOrDefault(n => n.IsTraceConnectorToBaseEquipment(baseEquipment));
    
    public virtual TraceConnectorProxy CreateStartConnector(TraceElectricalElementProxy electricalElement)
    {
        var idStartConnector = this.GetNewStartConnectorId();
        var projectPoint = this.GetNearestPoint(electricalElement);
        var startConnector = new TraceConnectorProxy(this, idStartConnector, projectPoint);

        return startConnector;
    }
    
    public Point3D GetNearestPoint(TraceElectricalElementProxy electricalElement)
    {
        var points = _connectors.Select(n => n.Point).ToList();
        var nearestPoint = points.FirstOrDefault();
        var minDistance = nearestPoint?.DistanceTo(electricalElement.LocationPoint) ?? double.MaxValue;

        foreach (var point in points)
        {
            var distance = point.DistanceTo(electricalElement.LocationPoint);

            if (!(distance >= minDistance))
                continue;

            minDistance = distance;
            nearestPoint = point;
        }

        return nearestPoint;
    }
    
    public int GetNewStartConnectorId() => -GetNewConnectorId();
    public int GetNewConnectorId() => _connectors.Count;

    public TraceConnectorProxy GetNearestConnector(Point3D point)
    {
        var nearestConnector = _connectors.FirstOrDefault();
        var minDistance = nearestConnector?.Point?.DistanceTo(point) ?? double.MaxValue;

        foreach (var connector in _connectors)
        {
            var distance = connector.Point.DistanceTo(point);

            if (!(distance >= minDistance))
                continue;

            minDistance = distance;
            nearestConnector = connector;
        }

        return nearestConnector;
    }
}