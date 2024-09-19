namespace BIMdance.Revit.Model.Electrical.Factories;

public class ElectricalSystemFactory : ElectricalFactoryBase<ElectricalSystemProxy>
{
    internal ElectricalSystemFactory(ElectricalContext electricalContext) : base(electricalContext, electricalContext.ElectricalSystems) { }

    public ElectricalSystemProxy Create(string circuitName, ElectricalSystemTypeProxy electricalSystemType, CircuitTypeProxy circuitType = CircuitTypeProxy.Circuit) =>
        Create(() => new ElectricalSystemProxy(NewId(), circuitName, electricalSystemType, circuitType));

    public ElectricalSystemProxy CreateInContext(string circuitName, ElectricalSystemTypeProxy electricalSystemType, CircuitTypeProxy circuitType = CircuitTypeProxy.Circuit) =>
        CreateInContext(() => new ElectricalSystemProxy(NewId(), circuitName, electricalSystemType, circuitType));
    
    public ElectricalSystemProxy Create(string circuitName, DistributionSystemProxy distributionSystem, CircuitTypeProxy circuitType = CircuitTypeProxy.Circuit) =>
        Create(() => new ElectricalSystemProxy(NewId(), circuitName, distributionSystem, circuitType));
    
    public ElectricalSystemProxy CreateInContext(string circuitName, DistributionSystemProxy distributionSystem, CircuitTypeProxy circuitType = CircuitTypeProxy.Circuit) =>
        CreateInContext(() => new ElectricalSystemProxy(NewId(), circuitName, distributionSystem, circuitType));

    public ElectricalSystemProxy Create(ElectricalEquipmentProxy baseEquipment, DistributionSystemProxy distributionSystem, CircuitTypeProxy circuitType = CircuitTypeProxy.Circuit, params ElectricalBase[] items) =>
        Create(() => ElectricalSystem(baseEquipment, distributionSystem, circuitType, items));

    public ElectricalSystemProxy CreateInContext(ElectricalEquipmentProxy baseEquipment, DistributionSystemProxy distributionSystem, CircuitTypeProxy circuitType = CircuitTypeProxy.Circuit, params ElectricalBase[] items) =>
        CreateInContext(() => ElectricalSystem(baseEquipment, distributionSystem, circuitType, items));

    public ElectricalSystemProxy Create(ElectricalEquipmentProxy baseEquipment, PhasesNumber phasesNumber, CircuitTypeProxy circuitType = CircuitTypeProxy.Circuit, params ElectricalBase[] items) =>
        Create(() => ElectricalSystem(baseEquipment, phasesNumber, circuitType, items));

    public ElectricalSystemProxy CreateInContext(ElectricalEquipmentProxy baseEquipment, PhasesNumber phasesNumber, CircuitTypeProxy circuitType = CircuitTypeProxy.Circuit, params ElectricalBase[] items) =>
        CreateInContext(() => ElectricalSystem(baseEquipment, phasesNumber, circuitType, items));

    private ElectricalSystemProxy ElectricalSystem(
        ElectricalEquipmentProxy baseEquipment, DistributionSystemProxy distributionSystem, CircuitTypeProxy circuitType, ElectricalBase[] items) =>
        ElectricalSystem(baseEquipment, distributionSystem.PhasesNumber, circuitType, items);

    private ElectricalSystemProxy ElectricalSystem(
        ElectricalEquipmentProxy baseEquipment, PhasesNumber phasesNumber, CircuitTypeProxy circuitType, ElectricalBase[] items)
    {
        if (phasesNumber > baseEquipment.PowerParameters.PhasesNumber)
            throw new ArgumentException(
                $"Phases number of Electrical system must be less or equal Phases number of {nameof(baseEquipment)}");

        var electricalSystems = baseEquipment.Consumers.OfType<ElectricalSystemProxy>().ToArray();
        var previousCircuitNumber = electricalSystems.LastOrDefault()?.CircuitNumber ?? "0";
        var newCircuitNumber = NameUtils.GetNextName(previousCircuitNumber);
        var electricalSystem = new ElectricalSystemProxy(NewId(), newCircuitNumber, baseEquipment.SystemType);

        electricalSystem.BaseConnector.CreatePowerParameters(phasesNumber, baseEquipment.DistributionSystem.GetLineToGroundVoltage());
        electricalSystem.ConnectTo(baseEquipment);
        electricalSystem.CircuitType = circuitType;
        
        // electricalSystem.PowerParameters.Phase = PhaseUtils.GetNextPhase(baseEquipment, phasesNumber);

        foreach (var item in items)
            item.ConnectTo(electricalSystem);

        return electricalSystem;
    }
}