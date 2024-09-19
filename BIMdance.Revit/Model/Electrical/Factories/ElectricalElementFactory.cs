namespace BIMdance.Revit.Model.Electrical.Factories;

public class ElectricalElementFactory : ElectricalFactoryBase<ElectricalElementProxy>
{
    public ElectricalElementFactory(ElectricalContext electricalContext) : base(electricalContext, electricalContext.ElectricalElements) { }
    
    public ElectricalElementProxy Create(BuiltInCategoryProxy category) =>
        Create(GetName(category), category);
    public ElectricalElementProxy Create(string name, BuiltInCategoryProxy category) =>
        Create(() => new ElectricalElementProxy(NewId(), name, category));
    
    public ElectricalElementProxy CreateInContext(BuiltInCategoryProxy category) =>
        CreateInContext(GetName(category), category);
    public ElectricalElementProxy CreateInContext(string name, BuiltInCategoryProxy category) =>
        CreateInContext(() => new ElectricalElementProxy(NewId(), name, category));
    
    public ElectricalElementProxy Create(
        DistributionSystemProxy distributionSystem,
        LoadClassificationProxy loadClassification = null,
        BuiltInCategoryProxy category = BuiltInCategoryProxy.OST_ElectricalFixtures) =>
        Create(distributionSystem.Name, distributionSystem, loadClassification, category);
    public ElectricalElementProxy Create(
        string name,
        DistributionSystemProxy distributionSystem,
        LoadClassificationProxy loadClassification = null,
        BuiltInCategoryProxy category = BuiltInCategoryProxy.OST_ElectricalFixtures) =>
        Create(name, distributionSystem.PhasesNumber, distributionSystem.GetLineToGroundVoltage(), loadClassification, category);
    
    public ElectricalElementProxy CreateInContext(
        DistributionSystemProxy distributionSystem,
        LoadClassificationProxy loadClassification = null,
        BuiltInCategoryProxy category = BuiltInCategoryProxy.OST_ElectricalFixtures) =>
        CreateInContext(distributionSystem.Name, distributionSystem, loadClassification, category);
    public ElectricalElementProxy CreateInContext(
        string name,
        DistributionSystemProxy distributionSystem,
        LoadClassificationProxy loadClassification = null,
        BuiltInCategoryProxy category = BuiltInCategoryProxy.OST_ElectricalFixtures) =>
        CreateInContext(name, distributionSystem.PhasesNumber, distributionSystem.GetLineToGroundVoltage(), loadClassification, category);
    
    public ElectricalElementProxy Create(
        PhasesNumber phasesNumber, double voltage,
        LoadClassificationProxy loadClassification = null,
        BuiltInCategoryProxy category = BuiltInCategoryProxy.OST_ElectricalFixtures) =>
        Create(GetName(category), phasesNumber, voltage, loadClassification, category);
    public ElectricalElementProxy Create(
        string name,
        PhasesNumber phasesNumber, double voltage,
        LoadClassificationProxy loadClassification = null,
        BuiltInCategoryProxy category = BuiltInCategoryProxy.OST_ElectricalFixtures) =>
        Create(() => new ElectricalElementProxy(NewId(), name, phasesNumber, voltage)
        {
            PowerParameters = { LoadClassification = loadClassification ?? ElectricalContext.LoadClassifications.FirstOrDefault()} ,
            Category = category
        });
    
    public ElectricalElementProxy CreateInContext(
        PhasesNumber phasesNumber, double voltage,
        LoadClassificationProxy loadClassification = null,
        BuiltInCategoryProxy category = BuiltInCategoryProxy.OST_ElectricalFixtures) =>
        CreateInContext(GetName(category), phasesNumber, voltage, loadClassification, category);
    public ElectricalElementProxy CreateInContext(
        string name,
        PhasesNumber phasesNumber, double voltage,
        LoadClassificationProxy loadClassification = null,
        BuiltInCategoryProxy category = BuiltInCategoryProxy.OST_ElectricalFixtures) =>
        CreateInContext(() => new ElectricalElementProxy(NewId(), name, phasesNumber, voltage)
        {
            PowerParameters = { LoadClassification = loadClassification ?? ElectricalContext.LoadClassifications.FirstOrDefault()} ,
            Category = category
        });
}