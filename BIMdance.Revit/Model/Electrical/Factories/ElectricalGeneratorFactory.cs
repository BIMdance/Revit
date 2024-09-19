namespace BIMdance.Revit.Model.Electrical.Factories;

public class ElectricalGeneratorFactory : ElectricalFactoryBase<Generator>
{
    public ElectricalGeneratorFactory(ElectricalContext electricalContext) : base(electricalContext, electricalContext.Generators) { }
    
    public Generator Create(ElectricalSystemTypeProxy systemType) =>
        Create(GetName(systemType), systemType);
    public Generator Create(string name, ElectricalSystemTypeProxy systemType) =>
        Create(() => new Generator(NewId(), name, systemType));
    
    public Generator CreateInContext(ElectricalSystemTypeProxy systemType) =>
        CreateInContext(GetName(systemType), systemType);
    public Generator CreateInContext(string name, ElectricalSystemTypeProxy systemType) =>
        CreateInContext(() => new Generator(NewId(), name, systemType));
    
    public Generator Create(DistributionSystemProxy distributionSystem) =>
        Create(distributionSystem.Name, distributionSystem);
    public Generator Create(string name, DistributionSystemProxy distributionSystem) =>
        Create(() => new Generator(NewId(), name, distributionSystem));
    
    public Generator CreateInContext(DistributionSystemProxy distributionSystem) =>
        CreateInContext(distributionSystem.Name, distributionSystem);
    public Generator CreateInContext(string name, DistributionSystemProxy distributionSystem) =>
        CreateInContext(() => new Generator(NewId(), name, distributionSystem));
}