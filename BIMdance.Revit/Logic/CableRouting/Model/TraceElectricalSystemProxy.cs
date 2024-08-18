namespace BIMdance.Revit.Logic.CableRouting.Model;

public class TraceElectricalSystemProxy : ElementProxy
{
    public TraceElectricalSystemProxy() { }
    public TraceElectricalElementProxy BaseEquipment { get; set; }
    public List<TraceElectricalElementProxy> Elements { get; set; }
    
    public string CableDesignation { get; set; }
    public string CircuitDesignation { get; set; }
    public string CircuitNumber { get; set; }
    public ConnectionTopology Topology { get; set; }
    public int CablesCount { get; set; }
    public double CableDiameter { get; set; }
    public double CableLength { get; set; }
    public double CableLengthMax { get; set; }
    public double CableLengthInCableTray { get; set; }
    public double CableLengthOutsideCableTray { get; set; }

    public override string ToString() =>
        $"[{Guid} / {RevitId}] <{GetType().Name}> {Name}" +
        $"\n\t{BaseEquipment}" +
        $"\n\t{string.Join("\n\t", Elements)}";
}