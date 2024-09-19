namespace BIMdance.Revit.Model.Electrical;

public class Network : ElectricalSource
{
    public Network() { }
        
    internal Network(
        int revitId, string name,
        ElectricalSystemTypeProxy electricalSystemType) :
        base(revitId, name, electricalSystemType) { }

    internal Network(
        int revitId, string name,
        DistributionSystemProxy distributionSystem) :
        base(revitId, name, distributionSystem) { }
}