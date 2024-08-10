namespace BIMdance.Revit.Model.RevitProxy;

public class ElectricalSystemProxy : ElementProxy
{
    public ElectricalSystemProxy() { }
    public ElectricalElementProxy BaseEquipment { get; set; }
    public List<ElectricalElementProxy> Elements { get; set; }
    
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

    public override string ToString()
    {
        return $"[{Guid} / {RevitId}] <{GetType().Name}> {Name}" +
               $"\n\t{BaseEquipment}" +
               $"\n\t{string.Join("\n\t", Elements)}";
    }
}