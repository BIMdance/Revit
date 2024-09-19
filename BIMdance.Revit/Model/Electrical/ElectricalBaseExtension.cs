namespace BIMdance.Revit.Model.Electrical;

public static class ElectricalBaseExtension
{
    public static IEnumerable<T> GetAllConnectedConsumersOf<T>(
        this ElectricalBase electrical,
        OperatingMode operatingMode = null)
        where T : ElectricalBase
    {
        return GetAllConnectedConsumersOf<T>(electrical, true, true, operatingMode);
    }
        
    private static IEnumerable<T> GetAllConnectedConsumersOf<T>(
        this ElectricalBase electrical,
        bool leftConnections,
        bool rightConnections,
        OperatingMode operatingMode = null)
        where T : ElectricalBase
    {
        operatingMode ??= Locator.Get<ElectricalContext>().CurrentOperatingMode;
            
        var directConsumers = electrical.GetFirstConsumersOf<T>();
        var consumers = directConsumers.Concat(directConsumers.SelectMany(n => GetAllConnectedConsumersOf<T>(n, operatingMode)));

        return electrical is SwitchBoardUnit switchboardUnit
            ? leftConnections
                ? consumers.Concat(GetSideConsumersOf<T>(switchboardUnit, leftConnections: true, rightConnections: false, operatingMode))
                : rightConnections
                    ? consumers.Concat(GetSideConsumersOf<T>(switchboardUnit, leftConnections: false, rightConnections: true, operatingMode))
                    : consumers
            : consumers;
    }

    private static List<T> GetSideConsumersOf<T>(
        EquipmentUnit switchboardUnit,
        bool leftConnections,
        bool rightConnections,
        OperatingMode operatingMode)
        where T : ElectricalBase
    {
        if (leftConnections && rightConnections)
            throw new ArgumentException($"Specify only one direction.");
            
        var reference = leftConnections
            ? switchboardUnit.LeftConnector.ReferenceConnector?.Owner
            : rightConnections
                ? switchboardUnit.RightConnector.ReferenceConnector?.Owner
                : null;
            
        if (reference == null)
            return new List<T>();
            
        var result = new List<T>();

        if (reference is T t)
            result.Add(t);
            
        result.AddRange(GetAllConnectedConsumersOf<T>(reference, leftConnections, rightConnections, operatingMode));
        result.AddRange(GetSideConsumersOf<T>(reference, leftConnections, rightConnections, operatingMode));
            
        return result;
    }

    public static ElectricalBase GetMainConnectedSource(this ElectricalBase electrical, OperatingMode operatingMode = null)
    {
        return electrical.GetAllConnectedSources(operatingMode).LastOrDefault();
    }
        
    public static List<ElectricalBase> GetAllConnectedSources(this ElectricalBase electrical, OperatingMode operatingMode = null, bool includeSelf = false)
    {
        var paths = GetStartState(electrical, operatingMode);
        var sources = new List<ElectricalBase>();

        try
        {
            while (sources.IsEmpty() && paths.Any())
            {
                for (var i = 0; i < paths.Count; i++)
                {
                    var path = paths[i];
                    var lastSource = path.Last();

                    switch (lastSource)
                    {
                        case ElectricalSource:
                            sources = path;
                            break;

                        case null when paths.Count == 1 && path.Count > 1:
                            sources = path.Take(path.Count - 1).ToList();
                            break;
                        
                        case null:
                            paths.Remove(path);
                            i--;
                            break;

                        default:
                            AddNextItems(paths, path, operatingMode);
                            break;
                    }
                }
            }
            
            return sources;
        }
        finally
        {
            if (includeSelf)
                sources.Insert(0, electrical);
        }
    }

    private static void AddNextItems(
        ICollection<List<ElectricalBase>> allPaths, List<ElectricalBase> currentPath,
        OperatingMode operatingMode)
    {
        var lastSource = currentPath.Last();
            
        if (lastSource.BaseConnector.GetState(operatingMode))
        {
            AddSource(currentPath, lastSource.BaseConnector, operatingMode);
        }

        else if (lastSource is EquipmentUnit lastPanelSection)
        {
            var reserveConnector = lastPanelSection.ReserveConnectors.FirstOrDefault(x => x.GetState(operatingMode));
            
            if (reserveConnector is not null)
            {
                AddSource(currentPath, reserveConnector, operatingMode);
            }
            else
            {
                var added = false;

                if (TryGetReference(lastPanelSection.LeftConnector, currentPath, operatingMode, out var leftReference))
                {
                    currentPath.Add(leftReference);
                    added = true;
                }

                if (TryGetReference(lastPanelSection.RightConnector, currentPath, operatingMode, out var rightReference))
                {
                    if (added)
                    {
                        allPaths.Add(new List<ElectricalBase>(currentPath.Take(currentPath.Count - 1))
                        {
                            rightReference
                        });
                    }
                    else
                    {
                        currentPath.Add(rightReference);
                        added = true;
                    }
                }

                if (!added)
                {
                    allPaths.Remove(currentPath);
                }
            }
        }
    }

    private static void AddSource(ICollection<ElectricalBase> path, ConnectorProxy connector, OperatingMode operatingMode)
    {
        if (connector.Source is ElectricalSource source &&
            source.GetState(operatingMode) == false)
            path.Add(null);
        else
            path.Add(connector.Source);
    }

    private static bool TryGetReference(InternalConnector<EquipmentUnit> connector, ICollection<ElectricalBase> path, OperatingMode operatingMode, out ElectricalBase reference)
    {
        reference = connector.ReferenceConnector?.Owner;

        return
            reference != null &&
            connector.GetState(operatingMode) &&
            !path.Contains(reference);
    }
        
    private static List<List<ElectricalBase>> GetStartState(ElectricalBase electrical, OperatingMode operatingMode)
    {
        var paths = new List<List<ElectricalBase>>();
            
        if (electrical.BaseConnector.GetState(operatingMode))
        {
            paths.Add(new List<ElectricalBase> { electrical.BaseConnector.Source });
        }

        else if (electrical is SwitchBoardUnit switchboardUnit)
        {
            var reserveConnector = switchboardUnit.ReserveConnectors.FirstOrDefault(x => x.GetState(operatingMode));
            
            if (reserveConnector is not null)
            {
                paths.Add(new List<ElectricalBase> { reserveConnector.Source });
            }
            else
            {
                if (switchboardUnit.LeftConnector.GetState(operatingMode))
                    paths.Add(new List<ElectricalBase> { switchboardUnit.LeftConnector.ReferenceConnector?.Owner });

                if (switchboardUnit.RightConnector.GetState(operatingMode))
                    paths.Add(new List<ElectricalBase> { switchboardUnit.RightConnector.ReferenceConnector?.Owner });
            }
        }

        return paths;
    }
        
    public static SwitchBoard GetFirstSourceOfPanel(this ElectricalBase electrical)
    {
        return electrical.GetFirstSourceOf<SwitchBoardUnit>()?.SwitchBoard;
    }
        
    public static IEnumerable<SwitchBoard> GetAllSourcesOfSwitchgear(this ElectricalBase electrical, ElectricalBase to = null)
    {
        return GetSwitchBoards(electrical.GetSourceChainOf<SwitchBoardUnit>());
    }
        
    public static IEnumerable<SwitchBoard> GetAllConsumersOfSwitchgear(this ElectricalBase electrical)
    {
        return GetSwitchBoards(electrical.GetAllConsumersOf<SwitchBoardUnit>());
    }
        
    public static IEnumerable<SwitchBoard> GetFirstConsumersOfSwitchBoard(this ElectricalBase electrical)
    {
        return GetSwitchBoards(electrical.GetFirstConsumersOf<SwitchBoardUnit>());
    }

    private static IEnumerable<SwitchBoard> GetSwitchBoards(IEnumerable<SwitchBoardUnit> sections)
    {
        return sections.Select(n => n.SwitchBoard).Distinct(new RevitProxyComparer<SwitchBoard>());
    }

    public static bool IsCompatible(this ElectricalBase electrical, ElectricalBase otherElectrical)
    {
        if (otherElectrical == null)
            return false;

        if (!electrical.IsPower)
            return electrical.BaseConnector.IsCompatible(otherElectrical.BaseConnector);

        if (electrical is ElectricalEquipmentProxy { DistributionSystem: { } } electricalEquipment)
            return IsCompatible(electricalEquipment, otherElectrical);

        if (otherElectrical is ElectricalEquipmentProxy { DistributionSystem: { } } otherElectricalEquipment)
            return IsCompatible(otherElectricalEquipment, electrical);

        return false;
    }

    private static bool IsCompatible(ElectricalEquipmentProxy electricalEquipment, ElectricalBase otherElectrical)
    {
        return otherElectrical is ElectricalEquipmentProxy otherElectricalEquipment
            ? electricalEquipment.DistributionSystem.IsCompatible(otherElectricalEquipment.DistributionSystem)
            : electricalEquipment.DistributionSystem.IsCompatible(otherElectrical);
    }
}