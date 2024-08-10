namespace BIMdance.Revit.Model.RevitProxy;

public class ElectricalElementProxy : TraceElement
{
    private List<TraceElement> _traceElements = new();
    public ElectricalElementProxy() { }
    public IReadOnlyCollection<TraceElement> TraceElements => _traceElements;
    public long LevelId { get; set; }
    public LevelProxy Level { get; set; }
    public long RoomId { get; set; }
    public RoomProxy Room { get; set; }
    public string ServiceType { get; set; }
    public TraceElement TraceBinding { get; private set; }
    public Point3D LocationPoint { get; set; }
    public double CableReserve { get; set; }

    public void AddTraceElement(TraceElement traceElement)
    {
        _traceElements.Add(traceElement);
        TraceNetwork = traceElement?.TraceNetwork;
    }

    public void ResetTraceElements() => _traceElements = new List<TraceElement>();

    public void SetTraceBinding(TraceElement traceElement)
    {
        TraceBinding = traceElement;
        TraceNetwork = traceElement?.TraceNetwork;
    }

    public void PullTraceBinding(ElectricalElementProxy prototype)
    {
        if (prototype == null) return;
        
        TraceBinding = prototype.TraceBinding;
        TraceNetwork = prototype.TraceNetwork;
        _traceElements = prototype._traceElements.ToList();
    }
}