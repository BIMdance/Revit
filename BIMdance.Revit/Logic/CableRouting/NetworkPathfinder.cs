namespace BIMdance.Revit.Logic.CableRouting;

public class NetworkPathfinder
{
    private readonly TraceElectricalElementProxy _baseEquipment;
    private readonly NetworkElements _networkElements;
    private readonly bool _existsDirectTraceToBaseEquipment;
    private HashSet<TraceElement> _circuitTraceElements = new();
    private Dictionary<TraceElectricalElementProxy, TracePath> _fromElementToBaseEquipmentTracePaths = new();
    
    private TraceElectricalElementProxy _electricalElement;
    private TraceElectricalSystemProxy _electricalSystem;
    private TracePath _elementTracePathResult;
    private ConnectionTopology _topology;
    private List<TracePath> _elementTracePaths;
    private List<TraceElement> _lockElements;
    private double _distanceToBinding;

    public NetworkPathfinder(TraceElectricalElementProxy baseEquipment, NetworkElements networkElements)
    {
        _baseEquipment = baseEquipment;
        _networkElements = networkElements;
        var cableTrayConduits = _networkElements.GetTraceElements(baseEquipment).ToList();
        cableTrayConduits.ForEach(n => CreateBaseEquipmentConnector(baseEquipment, n));
        _existsDirectTraceToBaseEquipment = cableTrayConduits.Any();
    }

    private void CreateBaseEquipmentConnector(
        TraceElectricalElementProxy baseEquipment,
        TraceElement traceElement)
    {
        var baseEquipmentConnectorId = traceElement.GetNewStartConnectorId();
        var baseEquipmentProjectPoint = traceElement is CableTrayConduitBaseProxy cableTrayConduit
            ? baseEquipment.LocationPoint.ProjectToLine(cableTrayConduit.Point1, cableTrayConduit.Point2)
            : traceElement.GetNearestConnector(baseEquipment.LocationPoint).Point;
        var baseEquipmentConnector = new TraceConnectorProxy(traceElement, baseEquipmentConnectorId, baseEquipmentProjectPoint);
        var directTracePathToBaseEquipment = new TracePath(baseEquipment, TracePathType.ByCableTrayConduit, true);

        baseEquipmentConnector.AddTracePathToBaseEquipment(baseEquipment, directTracePathToBaseEquipment);
    }

    public void SetElectricalSystem(TraceElectricalSystemProxy electricalSystem)
    {
        _electricalSystem = electricalSystem; 
        _topology = _electricalSystem.Topology;
    }
    
    public TracePath BuildTracePath(TraceElectricalElementProxy electricalElement)
    {
        ResetTracePathsToBaseEquipment();

        TracePath tracePath;

        _electricalElement = electricalElement;
        _electricalSystem ??= _electricalElement.ElectricalSystems.FirstOrDefault();
        _distanceToBinding = _networkElements.GetDistanceToTraceNode(_electricalElement);

        if (!_existsDirectTraceToBaseEquipment && !_fromElementToBaseEquipmentTracePaths.Any())
            tracePath = BuildTracePathFromBaseEquipment();
        else if (_baseEquipment.TraceNetwork != null && _electricalElement.TraceElements.Any(n => _baseEquipment.TraceNetwork.ElementInNetwork(n)))
            tracePath = BuildTracePathFromTraceElement();
        else
            tracePath = BuildTracePathFromTraceBinding();

        if (tracePath != null) _circuitTraceElements.AddRange(tracePath.TraceElements);

        return tracePath;
    }

    private TracePath BuildTracePathFromTraceBinding()
    {
        if (_electricalElement.TraceBinding != null)
        {
            return _electricalElement.TraceBinding switch
            {
                TraceElectricalElementProxy bindingElement => bindingElement.Equals(_baseEquipment)
                    ? BuildTracePathFromBaseEquipment()
                    : GetTracePathFromBinding(bindingElement),
                { } traceElement => FindPathToBaseEquipment(traceElement)
            };
        }

        LogTraceElementError();
        return null;
    }

    private TracePath BuildTracePathFromBaseEquipment()
    {
        var distance = (_electricalElement.LocationPoint - _baseEquipment.LocationPoint).SumAbsXYZ();

        var tracePathToBaseEquipment = new TracePath(_electricalElement, TracePathType.ByElement, true)
        {
            DistanceToBinding = distance,
        };

        AddTracePathToBaseEquipmentFromElement(tracePathToBaseEquipment);

        return tracePathToBaseEquipment;
    }

    private void LogTraceElementError()
    {
        // Logger.Error($"TraceElements is empty and TraceNode == null: ElectricalElement {ElectricalElement}");
    }

    #region BuildTracePath

    private TracePath BuildTracePathFromTraceElement()
    {
        var shortDistance = double.PositiveInfinity;
        TracePath shortPathToBaseEquipment = null;

        foreach (var traceElement in _electricalElement.TraceElements.OrderBy(n => n.RevitId))
        {
            var pathToBaseEquipment = FindPathToBaseEquipment(traceElement);

            if (pathToBaseEquipment == null || pathToBaseEquipment.Distance >= shortDistance) continue;

            shortDistance = pathToBaseEquipment.Distance;
            shortPathToBaseEquipment = pathToBaseEquipment;
        }

        return shortPathToBaseEquipment;
    }

    private TracePath FindPathToBaseEquipment(TraceElement startTraceElement)
    {
        CreateStartTracePaths(startTraceElement);

        var tracePathFromBaseEquipment = FindShortTracePath();

        if (tracePathFromBaseEquipment == null) return null;

        AddTracePathToBaseEquipmentFromElement(tracePathFromBaseEquipment);

        return tracePathFromBaseEquipment;
    }

    private TracePath GetTracePathFromBinding(TraceElectricalElementProxy bindingElement)
    {
        if (!_fromElementToBaseEquipmentTracePaths.TryGetValue(bindingElement, out var nodeTracePath))
        {
            var networkPathfinder = new NetworkPathfinder(_baseEquipment, _networkElements);
            networkPathfinder.SetElectricalSystem(_electricalSystem);
            nodeTracePath = networkPathfinder.BuildTracePath(bindingElement);

            if (nodeTracePath != null && !_fromElementToBaseEquipmentTracePaths.ContainsKey(bindingElement))
            {
                _fromElementToBaseEquipmentTracePaths.Add(bindingElement, nodeTracePath);
            }
            else
            {
                // Logger.Error($"{nameof(_fromElementToBaseEquipmentTracePaths)} does not contains key {bindingElement}.\n" +
                //              $"{nameof(ElectricalElement)}: {ElectricalElement}");
                return null;
            }
        }

        var elementTracePath = new TracePath(_electricalElement, nodeTracePath.TracePathType)
        {
            Distance = _topology == ConnectionTopology.Star ? nodeTracePath.Distance + nodeTracePath.DistanceToBinding : 0,
            DistanceToBaseEquipmentBinding = nodeTracePath.DistanceToBaseEquipmentBinding + nodeTracePath.DistanceToBinding,
            DistanceToBinding = _distanceToBinding,
            DistanceInCableTray = _topology == ConnectionTopology.Star ? nodeTracePath.DistanceInCableTray : 0,
        };

        elementTracePath.TraceElements.AddRange(nodeTracePath.TraceElements);

        AddTracePathToBaseEquipmentFromElement(elementTracePath);

        return elementTracePath;
    }

    private void CreateStartTracePaths(TraceElement traceElement)
    {
        var startConnector = traceElement is CableTrayConduitBaseProxy cableTrayConduit
            ? cableTrayConduit.CreateStartConnector(_electricalElement)
            : traceElement.CreateStartConnector(_electricalElement);
        var elementTracePath = new TracePath(_electricalElement, startConnector) { DistanceToBinding = _distanceToBinding, };

        if (ExistsPathToBaseEquipment(traceElement))
        {
            _elementTracePathResult = elementTracePath.SetTrace(_electricalSystem, startConnector, _topology);
            return;
        }

        CreateStartTracePaths(startConnector);
    }

    private void CreateStartTracePaths(TraceConnectorProxy startConnector)
    {
        var startTraceCableTrayConduit = startConnector.Element;
        _lockElements.Add(startTraceCableTrayConduit);

        startTraceCableTrayConduit.Connectors.ToList().ForEach(n => CreateStartTracePath(startConnector, n));
    }

    private void CreateStartTracePath(TraceConnectorProxy startConnector, TraceConnectorProxy traceConnector)
    {
        var startTraceCableTrayConduit = startConnector.Element;
        var tracePath = new TracePath(_electricalElement, startConnector) { DistanceToBinding = _distanceToBinding, };
        var distance = startConnector.DistanceTo(traceConnector);
        var distanceInCableTray = startTraceCableTrayConduit is CableTrayProxy ? distance : 0;

        tracePath.AddTrackConnector(startConnector, 0, 0);
        tracePath.AddTrackConnector(traceConnector, distance, distanceInCableTray);

        var refTraceConnector = traceConnector.RefConnector;

        if (refTraceConnector == null) return;

        tracePath.AddConnector(refTraceConnector);
        tracePath.AddDistance(distance, startTraceCableTrayConduit);

        _elementTracePaths.Add(tracePath);
    }

    private void AddTracePathToBaseEquipmentFromElement(TracePath tracePathFromBaseEquipment)
    {
        var distanceToBaseEquipment = tracePathFromBaseEquipment.Distance;
        var distanceToBaseEquipmentInCableTray = tracePathFromBaseEquipment.DistanceInCableTray;

        if (_fromElementToBaseEquipmentTracePaths.TryGetValue(_electricalElement, out var tracePath))
        {
            if (tracePath.Distance <= distanceToBaseEquipment) return;

            tracePath.Distance = distanceToBaseEquipment;
            tracePath.DistanceInCableTray = distanceToBaseEquipmentInCableTray;
        }
        else
        {
            _fromElementToBaseEquipmentTracePaths.Add(_electricalElement, tracePathFromBaseEquipment);
        }
    }

    #region FindShortTracePath

    private TracePath FindShortTracePath()
    {
        const int limitIteration = 10000;
        var i = 0;

        while (_elementTracePaths.Any() && _elementTracePathResult == null && i++ < limitIteration)
        {
            var shortTracePath = GetShortTracePath();
            var traceConnector = shortTracePath.Connectors.LastOrDefault();

            if (ExistsPathToBaseEquipment(traceConnector?.Element))
            {
                _elementTracePathResult = shortTracePath.SetTrace(_electricalSystem, traceConnector, _topology);
                break; // break или continue чтобы проверить другие варианты, надо протестировать
            }

            AddTracePathBranchesOfTraceElement(shortTracePath, traceConnector);
        }

        // if (i >= limitIteration)
        //     Logger.Error($"Превышено допустимое число итераций цикла" +
        //                  $"\n{GetType().FullName}.{nameof(FindShortTracePath)}: i > {i}" +
        //                  $"\n _baseEquipment: [{_baseEquipment?.Id}] {_baseEquipment?.Name}" +
        //                  $"\n ElectricalCircuit: [{ElectricalCircuit?.Id}] {ElectricalCircuit?.Name}" +
        //                  $"\n ElectricalElement: [{ElectricalElement?.Id}] {ElectricalElement?.Name}");

        return _elementTracePathResult;
    }

    private TracePath GetShortTracePath()
    {
        _elementTracePaths = _elementTracePaths.OrderBy(n => n.Distance).ToList();

        var tracePath = _elementTracePaths.ElementAt(0);
        _elementTracePaths.RemoveAt(0);

        return tracePath;
    }

    private void AddTracePathBranchesOfTraceElement(TracePath tracePath, TraceConnectorProxy inTraceConnector)
    {
        var elementFitting = inTraceConnector.Element;

        if (elementFitting == null || _lockElements.Contains(elementFitting)) return;

        _lockElements.Add(elementFitting);

        foreach (var fittingTraceConnector in elementFitting.Connectors)
        {
            if (fittingTraceConnector.Id == inTraceConnector.Id) continue;

            var newTracePath = tracePath.Clone();

            newTracePath.AddDistance(fittingTraceConnector.DistanceTo(inTraceConnector), elementFitting);
            newTracePath.AddTrackConnector(fittingTraceConnector, newTracePath.Distance,
                newTracePath.DistanceInCableTray);

            AddTracePathBranch(newTracePath, fittingTraceConnector);

            if (_elementTracePathResult != null) return;
        }

        _elementTracePaths = _elementTracePaths.OrderBy(n => n.Distance).ToList();
    }

    private void AddTracePathBranch(TracePath elementTracePath, TraceConnectorProxy fittingOutTraceConnector)
    {
        var fittingTraceElement = fittingOutTraceConnector?.Element;
        var nextInTraceConnector = fittingOutTraceConnector?.RefConnector;
        var nextTraceElement = nextInTraceConnector?.Element;

        while (nextTraceElement != null && nextTraceElement.RevitId != fittingTraceElement?.RevitId)
        {
            if (SpecialTraceElement(elementTracePath, nextInTraceConnector)) break;

            nextInTraceConnector = elementTracePath.GetNextTraceConnector(nextInTraceConnector);
            nextTraceElement = nextInTraceConnector?.Element;
        }
    }

    private bool SpecialTraceElement(TracePath elementTracePath, TraceConnectorProxy inTraceConnector)
    {
        if (ExistsPathToBaseEquipment(inTraceConnector.Element) || ElementIsFitting(inTraceConnector.Element))
        {
            AddTracePathToList(elementTracePath, inTraceConnector);
            return true;
        }

        return false;
    }

    private bool ExistsPathToBaseEquipment(TraceElement traceElement) =>
        traceElement?.GetTraceConnectorToBaseEquipment(_baseEquipment) != null;

    private static bool ElementIsFitting(TraceElement traceElement) =>
        traceElement?.ConnectorsCount > 2;

    private void AddTracePathToList(TracePath tracePath, TraceConnectorProxy inTraceConnector)
    {
        if (_lockElements.Contains(inTraceConnector.Element)) return;

        tracePath.AddConnector(inTraceConnector);
        _elementTracePaths.Add(tracePath);
    }

    #endregion

    #endregion

    private void ResetTracePathsToBaseEquipment()
    {
        _circuitTraceElements = new HashSet<TraceElement>();
        _fromElementToBaseEquipmentTracePaths = new Dictionary<TraceElectricalElementProxy, TracePath>();
        _lockElements = new List<TraceElement>();
        _elementTracePathResult = null;
        _elementTracePaths = new List<TracePath>();
    }
}