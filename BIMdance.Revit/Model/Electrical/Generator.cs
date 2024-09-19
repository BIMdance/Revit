namespace BIMdance.Revit.Model.Electrical;

public class Generator :
    ElectricalSource,
    IPropertyPrototype<Generator>
{
    public Generator() { }
    
    internal Generator(
        int revitId, string name,
        ElectricalSystemTypeProxy electricalSystemType) :
        base(revitId, name, electricalSystemType) { }
        
    internal Generator(
        int revitId, string name,
        DistributionSystemProxy distributionSystem) :
        base(revitId, name, distributionSystem) { }

    public void PullProperties(Generator prototype)
    {
        base.PullProperties(prototype);
    }
}