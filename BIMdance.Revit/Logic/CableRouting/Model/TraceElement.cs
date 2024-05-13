namespace BIMdance.Revit.Logic.CableRouting.Model;

public abstract class TraceElement : ElementProxy
{
    private readonly List<ConnectorProxy> _connectors = new();
    
    protected TraceElement() { }
    protected TraceElement(int revitId) : base(revitId) { }
    public TraceNetwork TraceNetwork { get; set; }
    public IEnumerable<ConnectorProxy> Connectors => _connectors;
    public int ConnectorsCount => _connectors.Count;
    public List<ElectricalSystemProxy> ElectricalSystems { get; } = new();
    public string CableTrace { get; set; }

    public void AddConnector(ConnectorProxy traceConnector)
    {
        if (_connectors.Any(n => n.Id == traceConnector.Id))
            return;
        
        _connectors.Add(traceConnector);
        traceConnector.Element = this;
    }
    
    public bool ContainsConnector(int id) => _connectors.Select(n => n.Id).Contains(id);

    public ConnectorProxy GetNextConnector(ConnectorProxy traceConnector)
    {
        return _connectors.FirstOrDefault(n => n.Id > traceConnector.Id) ??
               _connectors.FirstOrDefault(n => n.Id < traceConnector.Id);
    }
    
    public ConnectorProxy GetTraceConnectorToBaseEquipment(ElectricalElementProxy baseEquipment) =>
        _connectors.FirstOrDefault(n => n.IsTraceConnectorToBaseEquipment(baseEquipment));
    
    public virtual ConnectorProxy CreateStartConnector(ElectricalElementProxy electricalElement)
    {
        var idStartConnector = this.GetNewStartConnectorId();
        var projectPoint = this.GetNearestPoint(electricalElement);
        var startConnector = new ConnectorProxy(this, idStartConnector, projectPoint);

        return startConnector;
    }
    
    public XYZProxy GetNearestPoint(ElectricalElementProxy electricalElement)
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

    public ConnectorProxy GetNearestConnector(XYZProxy point)
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