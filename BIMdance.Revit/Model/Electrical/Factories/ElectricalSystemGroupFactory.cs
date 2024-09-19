namespace BIMdance.Revit.Model.Electrical.Factories;

public class ElectricalSystemGroupFactory : ElectricalFactoryBase<ElectricalSystemGroup>
{
    public ElectricalSystemGroupFactory(ElectricalContext electricalContext) : base(electricalContext, electricalContext.ElectricalSystemGroups) { }

    public ElectricalSystemGroup Create(string name, IReadOnlyCollection<ElectricalSystemProxy> circuits, PhasesNumber phasesNumber = PhasesNumber.Undefined) =>
        Create(() => new ElectricalSystemGroup(name, circuits, phasesNumber));
    
    public ElectricalSystemGroup CreateInContext(string name, IReadOnlyCollection<ElectricalSystemProxy> circuits, PhasesNumber phasesNumber = PhasesNumber.Undefined) =>
        CreateInContext(() => new ElectricalSystemGroup(name, circuits, phasesNumber));
}