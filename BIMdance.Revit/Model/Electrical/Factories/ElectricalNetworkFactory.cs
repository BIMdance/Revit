namespace BIMdance.Revit.Model.Electrical.Factories;

public class ElectricalNetworkFactory : ElectricalFactoryBase<Network>
{
    public ElectricalNetworkFactory(ElectricalContext electricalContext) : base(electricalContext, electricalContext.Networks) { }

    public Network Create(ElectricalSystemTypeProxy systemType) =>
        Create(GetName(systemType), systemType);
    public Network Create(string name, ElectricalSystemTypeProxy systemType) =>
        Create(() => new Network(NewId(), name, systemType));
    
    public Network CreateInContext(ElectricalSystemTypeProxy systemType) =>
        CreateInContext(GetName(systemType), systemType);
    public Network CreateInContext(string name, ElectricalSystemTypeProxy systemType) =>
        CreateInContext(() => new Network(NewId(), name, systemType));
    
    public Network Create(DistributionSystemProxy distributionSystem) =>
        Create(distributionSystem.Name, distributionSystem);
    public Network Create(string name, DistributionSystemProxy distributionSystem) =>
        Create(() => new Network(NewId(), name, distributionSystem));
    
    public Network CreateInContext(DistributionSystemProxy distributionSystem) =>
        CreateInContext(distributionSystem.Name, distributionSystem);
    public Network CreateInContext(string name, DistributionSystemProxy distributionSystem) =>
        CreateInContext(() => new Network(NewId(), name, distributionSystem));
    
    public Network CreateCompatibleNetwork(ElectricalBase electrical) => electrical.IsPower
        ? Create(GetDistributionSystem(electrical))
        : Create(electrical.SystemType);
    
    public Network CreateCompatibleNetworkInContext(ElectricalBase electrical) => electrical.IsPower
        ? CreateInContext(GetDistributionSystem(electrical))
        : CreateInContext(electrical.SystemType);
    
    private DistributionSystemProxy GetDistributionSystem(ElectricalBase electrical) => electrical is ElectricalEquipmentProxy electricalEquipment
        ? electricalEquipment.DistributionSystem
        : ElectricalContext.DistributionSystems.FirstOrDefault(x => x.IsCompatible(electrical));
}