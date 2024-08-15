namespace BIMdance.Revit.Utils.Revit.Electrical;

public static class ElectricalSystemUtils
{
    public static IEnumerable<FamilyInstance> GetAllBaseEquipments(Document document)
    {
        var electricalSystems = document.GetElectricalSystems();
        return electricalSystems.Select(n => n.BaseEquipment).Distinct(new ElementComparer<FamilyInstance>());
    }
        
    public static IEnumerable<ElectricalSystem> GetAllChildElectricalCircuits(FamilyInstance baseEquipment, ElectricalSystemType electricalSystemType = ElectricalSystemType.UndefinedSystemType)
    {
        var allChildElectricalSystems = new List<ElectricalSystem>();
        var childElectricalSystems = new Stack<ElectricalSystem>(GetChildElectricalCircuits(baseEquipment, electricalSystemType));

        while (childElectricalSystems.Any())
        {
            var electricalSystem = childElectricalSystems.Pop();
            allChildElectricalSystems.Add(electricalSystem);

            foreach (var element in electricalSystem.Elements)
            {
                if (element is not FamilyInstance familyInstance)
                    continue;

                var elementChildElectricalSystems = GetChildElectricalCircuits(familyInstance);

                foreach (var elementChildElectricalSystem in elementChildElectricalSystems)
                    childElectricalSystems.Push(elementChildElectricalSystem);
            }
        }

        return allChildElectricalSystems;
    }

    public static IEnumerable<ElectricalSystem> GetChildElectricalCircuits(FamilyInstance baseEquipment, ElectricalSystemType electricalSystemType = ElectricalSystemType.UndefinedSystemType) =>
        baseEquipment?.MEPModel != null
            ? electricalSystemType != ElectricalSystemType.UndefinedSystemType
                ? RevitVersionResolver.MEPModel.GetAssignedElectricalSystems(baseEquipment).Where(n => n.SystemType == electricalSystemType)
                : RevitVersionResolver.MEPModel.GetAssignedElectricalSystems(baseEquipment)
            : new List<ElectricalSystem>();

    public static IEnumerable<ElectricalSystem> GetParentElectricalCircuits(FamilyInstance baseEquipment, ElectricalSystemType electricalSystemType = ElectricalSystemType.UndefinedSystemType) =>
        baseEquipment?.MEPModel != null
            ? electricalSystemType != ElectricalSystemType.UndefinedSystemType
                ? GetAllParentElectricalCircuits(baseEquipment).Where(n => n.SystemType == electricalSystemType)
                : GetAllParentElectricalCircuits(baseEquipment)
            : new List<ElectricalSystem>();

    private static IEnumerable<ElectricalSystem> GetAllParentElectricalCircuits(FamilyInstance baseEquipment) =>
        RevitVersionResolver.MEPModel.GetElectricalSystems(baseEquipment)
            .Where(n => n.BaseEquipment?.Id != baseEquipment.Id);

    public static IEnumerable<Connector> GetElementConnectors(ElectricalSystem electricalSystem)
    {
        var idBaseEquipment = electricalSystem.BaseEquipment?.Id;
            
        return electricalSystem.GetConnectors()
            .Select(n => n.AllRefs?.OfType<Connector>().FirstOrDefault())
            .Where(n => n != null && n.Owner.Id != idBaseEquipment)
            .ToList();
    }
}