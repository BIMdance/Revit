namespace BIMdance.Revit.Logic.CableRouting.Model;

public class TraceNetwork
{
    public TraceNetwork(int id)
    {
        Id = id;
    }

    public int Id { get; }
    public List<TraceConnectorProxy> Connectors { get; } = new();
    public Dictionary<long, TraceElement> TraceElements { get; } = new();
    public Dictionary<long, TraceElectricalElementProxy> ElectricalElements { get; } = new();

    public TraceElement AddElement(TraceElement traceElement)
    {
        traceElement.TraceNetwork = this;

        switch (traceElement)
        {
            case TraceElectricalElementProxy electricalElement:
                ElectricalElements.Add(electricalElement.RevitId, electricalElement);
                break;

            default:
                TraceElements.Add(traceElement.RevitId, traceElement);
                break;
        }

        return traceElement;
    }
    
    public TraceElement GetElement(long revitId) =>
        ElectricalElements.TryGetValue(revitId, out var electricalElement) ? electricalElement :
        TraceElements.TryGetValue(revitId, out var traceElement) ? traceElement : null;

    public bool ElementInNetwork(long revitId) =>
        ElectricalElements.ContainsKey(revitId) ||
        TraceElements.ContainsKey(revitId);

    public bool ElementInNetwork(TraceElement traceElement) =>
        traceElement.TraceNetwork?.Equals(this) ?? false;
    
    public bool Equals(TraceNetwork other)
    {
        if (ReferenceEquals(other, null))
            return false;

        return ReferenceEquals(this, other) || Id.Equals(other.Id);
    }

    public override string ToString()
    {
        return $"{nameof(TraceNetwork)} [{Id}] TraceElements: {TraceElements.Count}"; // TraceElements: {TraceElements.EnumerableToString()}";
    }
}