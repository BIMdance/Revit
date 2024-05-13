namespace BIMdance.Revit.Logic.CableRouting.Model;

public abstract class CableTrayConduitBaseProxy : TraceElement
{
    private const double Tolerance = 1e-3;
    private CableTrayConduitOrientation _orientation;
    
    public CableTrayConduitBaseProxy() { }
    public XYZProxy Point1 { get; set; }
    public XYZProxy Point2 { get; set; }
    public CableTrayConduitOrientation Orientation => _orientation != CableTrayConduitOrientation.Undefined ? _orientation : SetOrientation();
    public string ServiceType { get; set; }
    public List<RoomProxy> Rooms { get; set; } = new();
    public List<LevelProxy> Levels { get; set; } = new();

    public override string ToString() => $"[{RevitId}] {Point1} <-> {Point2}";
    
    public override ConnectorProxy CreateStartConnector(ElectricalElementProxy electricalElement)
    {
        var idStartConnector = this.GetNewStartConnectorId();
        var projectPoint = electricalElement.LocationPoint.ProjectToSegment(Point1, Point2);
        var startConnector = new ConnectorProxy(this, idStartConnector, projectPoint);
            
        return startConnector;
    }
    
    private CableTrayConduitOrientation SetOrientation()
    {
        if (IsHorizontal()) return _orientation = CableTrayConduitOrientation.Horizontal;
        if (IsVertical())   return _orientation = CableTrayConduitOrientation.Vertical;
                            return _orientation = CableTrayConduitOrientation.Inclined;
    }

    private bool IsHorizontal() => Math.Abs(Point1.Z - Point2.Z) < Tolerance;
    private bool IsVertical() => Math.Abs(Point1.X - Point2.X) < Tolerance && Math.Abs(Point1.Y - Point2.Y) < Tolerance;
}