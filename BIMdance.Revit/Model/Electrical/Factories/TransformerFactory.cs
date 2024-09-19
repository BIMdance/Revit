using BIMdance.Revit.Model.Electrical.Transformers;

namespace BIMdance.Revit.Model.Electrical.Factories;

public class TransformerFactory : ElectricalFactoryBase<Transformer>
{
    public TransformerFactory(ElectricalContext electricalContext) :
        base(electricalContext, electricalContext.Transformers, ModelLocalization.PrefixTransformer) { }
    
    public Transformer Create(
        DistributionSystemProxy primaryDistributionSystem = default,
        DistributionSystemProxy secondaryDistributionSystem = default) =>
        Create(NewName(), primaryDistributionSystem, secondaryDistributionSystem);
    public Transformer Create(string name,
        DistributionSystemProxy primaryDistributionSystem = default,
        DistributionSystemProxy secondaryDistributionSystem = default) =>
        Create(() => new Transformer(NewId(), name, primaryDistributionSystem, secondaryDistributionSystem));
    
    public Transformer CreateInContext(
        DistributionSystemProxy primaryDistributionSystem = default,
        DistributionSystemProxy secondaryDistributionSystem = default) =>
        CreateInContext(() => new Transformer(NewId(), NewName(), primaryDistributionSystem, secondaryDistributionSystem));
    public Transformer CreateInContext(string name,
        DistributionSystemProxy primaryDistributionSystem = default,
        DistributionSystemProxy secondaryDistributionSystem = default) =>
        CreateInContext(() => new Transformer(NewId(), name, primaryDistributionSystem, secondaryDistributionSystem));
}