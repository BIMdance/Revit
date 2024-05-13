using StringComparer = BIMdance.Revit.Utils.StringComparer;

namespace BIMdance.Revit.Logic.CableRouting;

public class NetworkCalculator
{
    private readonly Document _document;
    private readonly NetworkConverter _networkConverter;
    private readonly ElectricalSystemConverter _electricalSystemConverter;
    private readonly CableRoutingSetting _cableRoutingSetting;
    
    private NetworkElements _networkElements;
    
    private NetworkPathfinder _networkPathfinder;

    public NetworkCalculator(
        Document document,
        CableRoutingSetting cableRoutingSetting)
    {
        _document = document;

        var building = new BuildingConverter(document).Convert();
        _electricalSystemConverter = new ElectricalSystemConverter(document, building);
        _networkConverter = new NetworkConverter(document, building);
        _cableRoutingSetting = cableRoutingSetting;
    }
    
    public string Calculate()
    {
        var electricalSystems = _electricalSystemConverter.Convert().Where(x => x.BaseEquipment != null).ToList();
        var baseEquipments = electricalSystems.Select(x => x.BaseEquipment).Distinct(new RevitIdEqualityComparer<ElectricalElementProxy>()).ToList();
        electricalSystems.ForEach(electricalSystem =>
        {
            var baseEquipment = baseEquipments.FirstOrDefault(x => x.RevitId == electricalSystem.BaseEquipment.RoomId);
            
            if (baseEquipment != null && !baseEquipment.ElectricalSystems.Contains(electricalSystem))
                baseEquipment.ElectricalSystems.Add(electricalSystem);
        });
        
        var networks = _networkConverter.Convert();
        _networkElements = new NetworkElements(networks);

        foreach (var baseEquipment in baseEquipments)
        {
            var network = networks.FirstOrDefault(x => x.GetElement(baseEquipment.RevitId) != null);
            var prototype = network?.GetElement(baseEquipment.RevitId) as ElectricalElementProxy;
            baseEquipment.PullTraceBinding(prototype);
            Calculate(network, baseEquipment);
        }

        SaveChanges(electricalSystems);
        // ShowTracePaths(baseEquipments);
        
        return $"\n\t{string.Join("\n\t", electricalSystems.Select(x => $"{x}"))}" +
               $"\n\t{string.Join("\n\t", networks.Select(x => $"{x}"))}";
    }

    private void Calculate(TraceNetwork network, ElectricalElementProxy baseEquipment)
    {
        _networkElements.SetTraceBinding(baseEquipment);
        _networkPathfinder = new NetworkPathfinder(baseEquipment, _networkElements);
        var distanceFromPanelToTraceNode = _networkElements.GetDistanceToTraceNode(baseEquipment);
        
        foreach (var electricalSystem in baseEquipment.ElectricalSystems)
        {
            // if (electricalCircuit.LockCableLength)
            //     continue;
            
            electricalSystem.Elements.ForEach(x => x.PullTraceBinding(network?.GetElement(x.RevitId) as ElectricalElementProxy));

            switch (electricalSystem.Topology)
            {
                case ConnectionTopology.Star:
                case ConnectionTopology.Tree:
                    CalculateStarTree(electricalSystem, distanceFromPanelToTraceNode);
                    break;
                
                // case Topology.Bus:
                //     CalculateBus(electricalSystem, distanceFromPanelToTraceNode);
                //     break;
                //
                // case Topology.Ring:
                //     CalculateRing(electricalSystem, distanceFromPanelToTraceNode);
                //     break;
            }
            
        }
    }

    private void CalculateStarTree(ElectricalSystemProxy electricalSystem, double distanceFromPanelToTraceNode)
    {
        var baseEquipment = electricalSystem.BaseEquipment;
        var cableLength = 0d;
        var cableLengthInCableTray = 0d;
        var maxCableLength = 0d;
        var elements = GetElementsToElements(electricalSystem);
        var traceByCableTrayConduit = false;

        _networkPathfinder.SetElectricalSystem(electricalSystem);
        
        while (elements.Any())
        {
            var currentElement = PopElement(elements);

            if (!currentElement.ElectricalSystems.Any())
                continue;

            var tracePath = _networkPathfinder.BuildTracePath(currentElement);

            // if (tracePath == null)
            // {
            //     try
            //     {
            //         FailureManager.AddFailure(BuiltInFailure.TracePathNotFound, currentElement.Id);
            //     }
            //     catch (FileLoadException) { }
            //     catch (FileNotFoundException) { }
            //     catch (TypeInitializationException) { }
            //     continue;
            // }

            traceByCableTrayConduit |= tracePath.TracePathType == TracePathType.ByCableTrayConduit;

            var distanceToNode = tracePath.DistanceToBinding + currentElement.CableReserve;
            var distanceInCableTray = tracePath.DistanceInCableTray;
            var elementMaxCableLength =
                tracePath.DistanceToBaseEquipmentBinding
                + distanceToNode
                + (tracePath.TracePathType == TracePathType.ByCableTrayConduit
                    ? distanceFromPanelToTraceNode
                    : 0);
            var elementCableLength = electricalSystem.Topology == ConnectionTopology.Tree
                ? (cableLength > 0 ? tracePath.Distance : tracePath.DistanceToBaseEquipmentBinding) + distanceToNode
                : elementMaxCableLength;

            if (elementMaxCableLength > maxCableLength)
                maxCableLength = elementMaxCableLength;

            cableLength += elementCableLength;
            cableLengthInCableTray += distanceInCableTray;

            // Debug.WriteLine("");
            // Debug.WriteLine($"\t currentElement.TraceBinding = {currentElement.TraceBinding}");
            // Debug.WriteLine($"\t tracePath.TracePathType = {tracePath.TracePathType}");
            // Debug.WriteLine($"\t tracePath.Distance = {tracePath.Distance.FeetToMillimeters()}");
            // Debug.WriteLine($"\t tracePath.DistanceToBinding = {tracePath.DistanceToBinding.FeetToMillimeters()}");
            // Debug.WriteLine($"\t tracePath.DistanceToBaseEquipmentBinding = {tracePath.DistanceToBaseEquipmentBinding.FeetToMillimeters()}");
            // Debug.WriteLine($"\t tracePath.DistanceInCableTray = {tracePath.DistanceInCableTray}");
            // Debug.WriteLine($"\t elementCableLength = {elementCableLength.FeetToMillimeters()}");
            // Debug.WriteLine($"\t elementMaxCableLength = {elementMaxCableLength.FeetToMillimeters()}");
            // Debug.WriteLine($"\t cableLength = {cableLength.FeetToMillimeters()}");
            // Debug.WriteLine($"\t cableLengthInCableTray = {cableLengthInCableTray}");
            // Debug.WriteLine($"\t maxCableLength = {maxCableLength.FeetToMillimeters()}");
            
            //if (traceNetwork == null || traceNetwork.Id != _panelTraceNetwork?.Id)
            //{
            //    // TODO : # | Petrov | Добавить в список элементов не соединённых с панелью и вывести в сообщении
            //    continue;
            //}

            SetCircuitsToTraceElements(electricalSystem, tracePath);
        }

        if (electricalSystem.Topology == ConnectionTopology.Tree &&
            traceByCableTrayConduit)
        {
            cableLength += distanceFromPanelToTraceNode;
        }

        cableLength *= 1 + _cableRoutingSetting.CableReservePercent;
        cableLengthInCableTray *= 1 + _cableRoutingSetting.CableReservePercent;
        maxCableLength *= 1 + _cableRoutingSetting.CableReservePercent;

        cableLength += baseEquipment.CableReserve;
        maxCableLength += baseEquipment.CableReserve;

        cableLength *= electricalSystem.CablesCount;
        cableLengthInCableTray *= electricalSystem.CablesCount;

        electricalSystem.CableLength = cableLength.MillimetersFromInternal();
        electricalSystem.CableLengthInCableTray = cableLengthInCableTray.MillimetersFromInternal();
        electricalSystem.CableLengthOutsideCableTray = electricalSystem.CableLength - electricalSystem.CableLengthInCableTray;
        electricalSystem.CableLengthMax = maxCableLength.MillimetersFromInternal();
    }

    private void CalculateBus(ElectricalSystemProxy electricalCircuit, double distanceFromPanelToTraceNode)
    {
    }

    private void CalculateRing(ElectricalSystemProxy electricalCircuit, double distanceFromPanelToTraceNode)
    {
    }

    private static List<ElectricalElementProxy> GetElementsToElements(ElectricalSystemProxy electricalSystem)
    {
        var electricalPanel = electricalSystem.BaseEquipment;
        var elementsToValidBindings = GetElementsToValidBindings(electricalSystem);
        var elementsToElements = GetElementsToElements(electricalSystem, electricalPanel);

        while (elementsToElements.Any())
        {
            var element = elementsToElements[0];
            elementsToElements.RemoveAt(0);

            OrderElementsBindings(element, elementsToValidBindings, elementsToElements);
        }

        return elementsToValidBindings;
    }

    private static List<ElectricalElementProxy> GetElementsToElements(ElectricalSystemProxy electricalSystem,
        ElectricalElementProxy baseEquipment)
    {
        return electricalSystem.Elements
            .Where(n => n.TraceBinding is ElectricalElementProxy)
            .OrderBy(n => n.LocationPoint.RatingDistanceTo(baseEquipment.LocationPoint))
            .ToList();
    }

    private static List<ElectricalElementProxy> GetElementsToValidBindings(ElectricalSystemProxy electricalSystem)
    {
        var baseEquipment = electricalSystem.BaseEquipment;

        return electricalSystem.Elements
            .Where(n => n.TraceBinding is CableTrayConduitBaseProxy ||
                        n.TraceElements.Any())
            .OrderBy(n => n.LocationPoint.RatingDistanceTo(baseEquipment.LocationPoint))
            .ToList();
    }

    private static void OrderElementsBindings(ElectricalElementProxy element,
        List<ElectricalElementProxy> elementsToValidBindings,
        List<ElectricalElementProxy> elementsToElements)
    {
        var binding = element.TraceBinding as ElectricalElementProxy;
        var bindings = new List<ElectricalElementProxy> { element };

        while (binding is { TraceBinding: not CableTrayConduitBaseProxy and not ElectricalElementProxy })
        {
            if (!elementsToValidBindings.Contains(binding))
                bindings.Insert(0, binding);

            if (elementsToElements.Contains(binding))
                elementsToElements.Remove(binding);

            binding = binding.TraceBinding as ElectricalElementProxy;
        }

        elementsToValidBindings.AddRange(bindings);
    }

    private static ElectricalElementProxy PopElement(ICollection<ElectricalElementProxy> elements)
    {
        var element = elements.First();
        elements.Remove(element);
        return element;
    }

    private static void SetCircuitsToTraceElements(ElectricalSystemProxy electricalCircuit, TracePath tracePath)
    {
        var traceElements = tracePath.TraceElements.Where(x =>
            electricalCircuit.Topology == ConnectionTopology.Star ||
            electricalCircuit.Topology == ConnectionTopology.Tree &&
            !x.ElectricalSystems.Contains(electricalCircuit));

        foreach (var traceElement in traceElements)
        {
            traceElement.ElectricalSystems.Add(electricalCircuit);
        }
    }
    
    private void SaveChanges(List<ElectricalSystemProxy> electricalSystems)
    {
        _document.Transaction(_ =>
        {
            electricalSystems.ForEach(proxy =>
            {
                if (_document.GetElement(RevitVersionResolver.NewElementId(proxy.RevitId)) is ElectricalSystem revitElement)
                    RevitMapper.Map(proxy, revitElement);
            });
            
            _networkElements.CableTrayConduits.ForEach(proxy =>
            {
                proxy.CableTrace = string.Join("\n", proxy.ElectricalSystems.Select(GetElectricalCircuitName).OrderBy(n => n, new StringComparer()));

                // if (proxy is CableTrayProxy cableTray)
                //     cableTray.Filling = cableTray.ElectricalSystems.Sum(x => x.CableDiameter * x.CableDiameter);
                
                if (_document.GetElement(RevitVersionResolver.NewElementId(proxy.RevitId)) is CableTrayConduitBase revitElement)
                    RevitMapper.Map(proxy, revitElement);
            });
            
        }, TransactionNames.ModifyElementAttributes);
    }

    private static string GetElectricalCircuitName(ElectricalSystemProxy electricalSystem)
    {
        return !string.IsNullOrWhiteSpace(electricalSystem.CableDesignation)
            ? electricalSystem.CableDesignation
            : !string.IsNullOrWhiteSpace(electricalSystem.CircuitDesignation)
                ? electricalSystem.CircuitDesignation
                : electricalSystem.CircuitNumber;
    }

    private void ShowTracePaths(IReadOnlyCollection<ElectricalElementProxy> baseEquipments)
    {
        _document.Transaction(_ =>
        {
            var networkDrawer = new NetworkDrawer(_document);
            networkDrawer.ClearBindingLines();
            
            foreach (var electricalSystem in baseEquipments.SelectMany(baseEquipment => baseEquipment.ElectricalSystems))
            {
                networkDrawer.DrawBinding(electricalSystem);
            }
        }, TransactionNames.DrawCableRouting);
    }
}