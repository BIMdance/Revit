namespace BIMdance.Revit.Logic.Utils;

public static class StructureUtils
{
    private static readonly List<ElectricalBase> Occupied = new();

    public static IEnumerable<ElectricalEquipmentProxy> GetOrderedConnectedEquipments(
        IEnumerable<ElectricalEquipmentProxy> equipments,
        OperatingMode operatingMode)
    {
        return equipments.SelectMany(x => GetOrderedConnectedEquipments(x, operatingMode));
    }
    
    public static List<ElectricalEquipmentProxy> GetOrderedConnectedEquipments(
        ElectricalEquipmentProxy equipment,
        OperatingMode operatingMode)
    {
        // if (equipment.BaseConnector.GetState(operatingMode) == false)
        //     return new List<ElectricalEquipment>();
            
        Occupied.Add(equipment);
            
        var result = new List<ElectricalEquipmentProxy> { equipment };

        foreach (var consumer in equipment
                     .GetFirstConsumersOf<ElectricalEquipmentProxy>()
                     .Where(n => n.BaseConnector.GetState(operatingMode)))
        {
            result.AddRange(GetOrderedConnectedEquipments(consumer, operatingMode));
        }

        if (equipment is SwitchBoardUnit switchboardUnit &&
            equipment.BaseConnector.GetState(operatingMode))
        {
            result.AddRange(GetSideConnectorConsumers(switchboardUnit.LeftConnector, operatingMode));
            result.AddRange(GetSideConnectorConsumers(switchboardUnit.RightConnector, operatingMode));
        }
                
        Occupied.Remove(equipment);
                
        return result;
    }

    private static IEnumerable<ElectricalEquipmentProxy> GetSideConnectorConsumers(
        InternalConnector<EquipmentUnit> connector, OperatingMode operatingMode)
    {
        if (!connector.GetState(operatingMode))
            return new List<ElectricalEquipmentProxy>();
            
        var reference = connector.ReferenceConnector?.Owner;

        return reference != null && !Occupied.Contains(reference)
            ? GetOrderedConnectedEquipments(reference, operatingMode)
            : new List<ElectricalEquipmentProxy>();
    }

    public static IEnumerable<ElectricalBase> GetOrderedConnectedElectricalItems(ElectricalBase electrical)
    {
        var result = electrical.Consumers.Where(n => n.Consumers.Any()).ToList();

        foreach (var consumer in electrical.Consumers.Where(n => n.Consumers.Any()))
            result.AddRange(GetOrderedConnectedElectricalItems(consumer));

        return result;
    }

    public static IEnumerable<ElectricalElementProxy> GetConnectedElements(ElectricalBase electrical, OperatingMode operatingMode = null)
    {
        var mainSource = electrical is ElectricalSource source
            ? source
            : electrical.GetMainConnectedSource(operatingMode);

        if ((mainSource?.GetState(operatingMode) ?? false) == false)
            return new List<ElectricalElementProxy>();

        return electrical switch
        {
            ElectricalEquipmentProxy equipment => GetOrderedConnectedEquipments(equipment, operatingMode)
                .SelectMany(n => n.GetConsumersOf<ElectricalSystemProxy>())
                .SelectMany(n => n.GetConsumersOf<ElectricalElementProxy>()),
                
            ElectricalSystemProxy circuit => circuit.GetConsumersOf<ElectricalElementProxy>()
                .Concat(circuit.GetConsumersOf<ElectricalEquipmentProxy>()
                    .Where(n => n.BaseConnector.GetState(operatingMode))
                    .SelectMany(n => GetConnectedElements(n, operatingMode))),
                
            _ => new List<ElectricalElementProxy>()
        };
    }

    public static IEnumerable<T> GetConnectedEquipments<T>(ElectricalBase electrical, OperatingMode operatingMode = null)
        where T : ElectricalEquipmentProxy
    {
        var mainSource = electrical is ElectricalSource source
            ? source
            : electrical.GetMainConnectedSource(operatingMode);

        return mainSource?.GetState(operatingMode) ?? false 
            ? GetOrderedConnectedEquipments(electrical as ElectricalEquipmentProxy, operatingMode).OfType<T>()
            : new List<T>();
    }

    private static IEnumerable<ElectricalElementProxy> GetSectionConnectorElements(
        InternalConnector<SwitchBoardUnit> connector, OperatingMode operatingMode)
    {
        if (!connector.ElectricalConnector.GetState(operatingMode))
            return new List<ElectricalElementProxy>();
            
        var reference = connector.ReferenceConnector.Owner;

        return !Occupied.Contains(reference)
            ? GetConnectedElements(reference, operatingMode)
            : new List<ElectricalElementProxy>();
    }
}