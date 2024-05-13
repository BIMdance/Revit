namespace BIMdance.Revit.Logic.CableRouting;

public class NetworkElements
{
    private List<ElectricalElementProxy> _allDoneElements;

    public NetworkElements(NetworkConverter networkConverter) :
        this(networkConverter.Convert()) { }

    public NetworkElements(IEnumerable<TraceNetwork> traceNetworks)
    {
        CableTrayConduits = traceNetworks
            .SelectMany(x => x.TraceElements.Values)
            .OfType<CableTrayConduitBaseProxy>()
            .ToList();
    }
    
    public List<CableTrayConduitBaseProxy> CableTrayConduits { get; }

    public void SetTraceBinding(ElectricalElementProxy baseEquipment)
    {
        var traceNetwork = GetTraceNetwork(baseEquipment);
        baseEquipment.ElectricalSystems.ForEach(proxy => SetTraceBinding(proxy, traceNetwork));
    }

    public void SetTraceBinding(ElectricalSystemProxy electricalSystem, TraceNetwork traceNetwork = null)
    {
        var baseEquipment = electricalSystem.BaseEquipment;
        var elements = electricalSystem.Elements;
        traceNetwork ??= baseEquipment.TraceNetwork;

        ResetTraceBindings(elements);
        
        _allDoneElements = new List<ElectricalElementProxy> { /* baseEquipment */ };

        var (withTraceElements, withoutTraceElements, withTraceOutRoomsElements, withoutTraceOutRoomsElements) =
            SplitElements(elements, traceNetwork);

        SetBindingsWithTraceElements(withTraceElements);
        SetBindingsWithTraceElements(withTraceOutRoomsElements);
        SetBindingsWithoutTraceElements(withoutTraceElements, baseEquipment);
        SetBindingsWithoutTraceElements(withoutTraceOutRoomsElements, baseEquipment);

        _allDoneElements.Where(n => n.TraceBinding == null && !n.TraceElements.Any()).ToList()
            .ForEach(n => n.SetTraceBinding(baseEquipment));
    }
    
    private TraceNetwork GetTraceNetwork(ElectricalElementProxy electricalElement)
    {
        if (electricalElement.TraceNetwork == null)
            SetTraceNetwork(electricalElement);

        return electricalElement.TraceNetwork;
    }

    private void SetTraceNetwork(ElectricalElementProxy electricalElement)
    {
        var nearestTraceElement =
            FindNearestTraceElementInRoom(electricalElement) ??
            FindNearestTraceElementInLevel(electricalElement);

        electricalElement.SetTraceBinding(nearestTraceElement);
    }

    private static void ResetTraceBindings(List<ElectricalElementProxy> elements)
    {
        elements.ForEach(n => n.SetTraceBinding(null));
    }

    #region Binding Elements

    private (List<ElectricalElementProxy> ElementsWithTrace, List<ElectricalElementProxy> ElementsWithoutTrace,
        List<ElectricalElementProxy> ElementsWithTraceOutRooms, List<ElectricalElementProxy>
        ElementsWithoutTraceOutRooms)
        SplitElements(IEnumerable<ElectricalElementProxy> elements, TraceNetwork traceNetwork)
    {
        var withTraceElements = new List<ElectricalElementProxy>();
        var withoutTraceElements = new List<ElectricalElementProxy>();
        var withTraceOutRoomsElements = new List<ElectricalElementProxy>();
        var withoutTraceOutRoomsElements = new List<ElectricalElementProxy>();

        foreach (var element in elements)
        {
            SetTraceBinding(element, traceNetwork);

            if ((element.Room?.RevitId ?? -1) > 0)
            {
                if (element.TraceNetwork != null)
                    withTraceElements.Add(element);
                else
                    withoutTraceElements.Add(element);
            }
            else
            {
                if (element.TraceNetwork != null)
                    withTraceOutRoomsElements.Add(element);
                else
                    withoutTraceOutRoomsElements.Add(element);
            }
        }

        return (withTraceElements, withoutTraceElements, withTraceOutRoomsElements, withoutTraceOutRoomsElements);
    }

    internal void SetTraceBinding(ElectricalElementProxy element, TraceNetwork traceNetwork)
    {
        if (traceNetwork?.ElementInNetwork(element) ?? false)
            return;

        var nearestTraceElement = FindNearestTraceElementInRoom(element, traceNetwork); // ??
        //FindNearestTraceElementInLevel(element, _panelTraceNetwork);

        element.SetTraceBinding(nearestTraceElement);
    }

    private void SetBindingsWithTraceElements(IList<ElectricalElementProxy> elementsWithTrace)
    {
        while (elementsWithTrace.Any())
        {
            var currentElement = elementsWithTrace[0];
            var room = currentElement.Room;
            var elementsInRoom = elementsWithTrace.Where(n => n.Room?.Equals(room) ?? room == null).ToList();

            foreach (var elementInRoom in elementsInRoom)
                elementsWithTrace.Remove(elementInRoom);

            SetTraceBindings(elementsInRoom);
        }
    }

    #region SetLinksWithTraceElements

    private void SetTraceBindings(IEnumerable<ElectricalElementProxy> elementsInRoom)
    {
        var doneElementsInRoom = new List<ElectricalElementProxy>();
        var sortElementsInRoom = elementsInRoom
            .Select(n => new { Element = n, RatingDistance = GetRatingDistanceToTraceBinding(n) })
            .OrderBy(n => n.RatingDistance)
            .ToList();

        foreach (var elementRating in sortElementsInRoom)
        {
            var element = elementRating.Element;
            var nearestElement = FindNearestElement(doneElementsInRoom, element);

            doneElementsInRoom.Add(element);

            if (nearestElement == null)
                continue;

            var rating = elementRating.RatingDistance;
            var ratingNearestElement = nearestElement.LocationPoint.RatingDistanceTo(element.LocationPoint);
            var isElementLinkByServiceType =
                !string.IsNullOrEmpty(element.ServiceType) &&
                element.ServiceType != nearestElement.ServiceType &&
                element.TraceBinding is CableTrayConduitBaseProxy cableTrayConduit &&
                cableTrayConduit.ServiceType == element.ServiceType;

            if (rating < ratingNearestElement ||
                isElementLinkByServiceType)
                continue;

            element.ResetTraceElements();
            element.SetTraceBinding(nearestElement);
        }

        _allDoneElements.AddRange(doneElementsInRoom);
    }

    private double GetRatingDistanceToTraceBinding(ElectricalElementProxy n)
    {
        return GetTraceBinding(n) switch
        {
            CableTrayConduitBaseProxy traceCableTrayConduit => n.LocationPoint.RatingDistanceToSegment(traceCableTrayConduit.Point1, traceCableTrayConduit.Point2),
            ElectricalElementProxy electricalElement => n.LocationPoint.RatingDistanceTo(electricalElement.LocationPoint),
            _ => double.PositiveInfinity
        };
    }

    public TraceElement GetTraceBinding(ElectricalElementProxy element)
    {
        if (element.TraceBinding != null)
            return element.TraceBinding;

        if (element.TraceElements.Any())
            return element.TraceElements.ElementAt(0);

        //if (element.IdsTraceElements.Any())
        //    return _traceElements.FirstOrDefault(n => n.Id == element.IdsTraceElements[0]);

        return null;
    }

    public List<TraceElement> GetTraceElements(ElectricalElementProxy element)
    {
        var traceElements = new List<TraceElement>(element.TraceElements);

        if (element.TraceBinding is CableTrayConduitBaseProxy traceCableTrayConduit)
            traceElements.Add(traceCableTrayConduit);

        return traceElements;
    }

    #endregion

    private void SetBindingsWithoutTraceElements(
        List<ElectricalElementProxy> withoutTraceElements,
        ElectricalElementProxy baseEquipment)
    {
        while (withoutTraceElements.Any())
        {
            var nearestToPanelElement = FindNearestElement(withoutTraceElements, baseEquipment);
            var room = nearestToPanelElement?.Room;
            var elementsInRoom = withoutTraceElements.Where(n => n.Room?.Equals(room) ?? room == null).ToList();

            foreach (var elementInRoom in elementsInRoom)
                withoutTraceElements.Remove(elementInRoom);

            var farthestFromPanelElementInRoom = FindFarthestElement(elementsInRoom, baseEquipment);
            var boundingTrace = GetBoundingTrace(farthestFromPanelElementInRoom, baseEquipment);
            // var doneElementsFiltered = new[] { baseEquipment }.ToList();
            var doneElementsFiltered = GetDoneElementsFiltered(boundingTrace).ToList();
            var traceElementsFiltered = GetTraceElementsFiltered(farthestFromPanelElementInRoom, boundingTrace, baseEquipment.TraceNetwork);
            var currentElement = GetCurrentElement(baseEquipment, elementsInRoom, nearestToPanelElement, doneElementsFiltered, traceElementsFiltered);

            SetTraceBindings(elementsInRoom, currentElement);
        }
    }

    #region SetBindingsWithoutTraceElements

    private (double Left, double Top, double Right, double Bottom) GetBoundingTrace(
        ElectricalElementProxy farthestFromPanelElementInRoom,
        ElectricalElementProxy baseEquipment)
    {
        var radius = farthestFromPanelElementInRoom.LocationPoint.DistanceToOnPlane(baseEquipment.LocationPoint, Normal.Z);
        var left = farthestFromPanelElementInRoom.LocationPoint.X - radius;
        var top = farthestFromPanelElementInRoom.LocationPoint.Y + radius;
        var right = farthestFromPanelElementInRoom.LocationPoint.X + radius;
        var bottom = farthestFromPanelElementInRoom.LocationPoint.Y - radius;

        return (left, top, right, bottom);
    }

    private IEnumerable<ElectricalElementProxy> GetDoneElementsFiltered(
        (double Left, double Top, double Right, double Bottom) boundingTrace)
    {
        return _allDoneElements.Where(n =>
            n.LocationPoint.X >= boundingTrace.Left &&
            n.LocationPoint.X <= boundingTrace.Right &&
            n.LocationPoint.Y >= boundingTrace.Bottom &&
            n.LocationPoint.Y <= boundingTrace.Top);
    }

    private List<CableTrayConduitBaseProxy> GetTraceElementsFiltered(
        ElectricalElementProxy farthestFromPanelElementInRoom,
        (double Left, double Top, double Right, double Bottom) boundingTrace,
        TraceNetwork traceNetwork)
    {
        return FindTraceElementsInLevel(farthestFromPanelElementInRoom, traceNetwork)
            .Where(n =>
            {
                var endPoints = new[] { n.Point1, n.Point2 };
                return endPoints.Any(
                    p => p.X >= boundingTrace.Left &&
                         p.X <= boundingTrace.Right &&
                         p.Y >= boundingTrace.Bottom &&
                         p.Y <= boundingTrace.Top);
            }).ToList();
    }

    private static ElectricalElementProxy GetCurrentElement(
        ElectricalElementProxy baseEquipment,
        IEnumerable<ElectricalElementProxy> elementsInRoom,
        ElectricalElementProxy nearestToPanelElement,
        IReadOnlyCollection<ElectricalElementProxy> doneElementsFiltered,
        IReadOnlyCollection<CableTrayConduitBaseProxy> traceElementsFiltered)
    {
        var minRating = double.PositiveInfinity;
        var currentElement = nearestToPanelElement;

        currentElement.ResetTraceElements();

        foreach (var elementInRoom in elementsInRoom)
        {
            foreach (var doneElement in doneElementsFiltered)
            {
                var rating = elementInRoom.LocationPoint.RatingDistanceTo(doneElement.LocationPoint);

                if (rating >= minRating - 1e-3)
                    continue;
                
                minRating = rating;
                currentElement = elementInRoom;
                currentElement.SetTraceBinding(doneElement);
            }

            if (currentElement.TraceBinding is CableTrayConduitBaseProxy)
                continue;

            foreach (var traceElement in traceElementsFiltered)
            {
                var rating = elementInRoom.LocationPoint.RatingDistanceToSegment(traceElement.Point1, traceElement.Point2);
            
                if (rating >= minRating - 1e-3)
                    continue;
                
                if (!traceElement.Rooms.Contains(currentElement.Room))
                {
                    if (Equals(baseEquipment.Room, currentElement.Room))
                        continue;
                    
                    var baseEquipmentRating = baseEquipment.LocationPoint.RatingDistanceToSegment(traceElement.Point1, traceElement.Point2);
                    
                    if (baseEquipmentRating >= minRating - 1e-3)
                        continue;
                }
                
                minRating = rating;
                currentElement = elementInRoom;
                currentElement.SetTraceBinding(traceElement);
            }
        }

        return currentElement;
    }

    private void SetTraceBindings(
        IEnumerable<ElectricalElementProxy> elementsInRoom,
        ElectricalElementProxy currentElement)
    {
        var doneElementsInRoom = new List<ElectricalElementProxy>();
        var sortElementsInRoom = elementsInRoom.OrderBy(n => currentElement.LocationPoint.RatingDistanceTo(n.LocationPoint)).ToList();

        foreach (var element in sortElementsInRoom)
        {
            doneElementsInRoom.Add(element);
            _allDoneElements.Add(element);

            var nearestElement = InRoom(element)
                ? FindNearestElement(doneElementsInRoom, element)
                : FindNearestElement(_allDoneElements, element);

            if (nearestElement == null)
                continue;

            element.SetTraceBinding(nearestElement);
        }
    }

    #endregion

    #endregion

    internal double GetDistanceToTraceNode(ElectricalElementProxy element)
    {
        return GetTraceBinding(element) switch
        {
            CableTrayConduitBaseProxy cableTrayConduit => (element.LocationPoint - element.LocationPoint.ProjectToSegment(cableTrayConduit.Point1, cableTrayConduit.Point2)).SumAbsXYZ(),
            ElectricalElementProxy electricalElement => (element.LocationPoint - electricalElement.LocationPoint).SumAbsXYZ(),
            _ => 0
        };
    }
    
    private CableTrayConduitBaseProxy FindNearestTraceElementInRoom(
        ElectricalElementProxy electricalElement,
        TraceNetwork traceNetwork = null)
    {
        if (electricalElement == null) throw new NullReferenceException(nameof(electricalElement));
        if (electricalElement.LocationPoint == null) throw new NullReferenceException(nameof(electricalElement.LocationPoint));

        var traceElementsInRoom = FindTraceElementsInRoom(electricalElement, traceNetwork);

        return FindNearestTraceElement(electricalElement.LocationPoint, traceElementsInRoom);
    }

    private TraceElement FindNearestTraceElementInLevel(ElectricalElementProxy electricalElement,
        TraceNetwork traceNetwork = null)
    {
        if (electricalElement == null) throw new NullReferenceException(nameof(electricalElement));
        if (electricalElement.LocationPoint == null) throw new NullReferenceException(nameof(electricalElement.LocationPoint));

        var traceElementsInLevel = FindTraceElementsInLevel(electricalElement, traceNetwork);

        return FindNearestTraceElement(electricalElement.LocationPoint, traceElementsInLevel);
    }

    internal List<CableTrayConduitBaseProxy> FindTraceElementsInRoom(ElectricalElementProxy element, TraceNetwork traceNetwork)
    {
        var room = element.Room;
        var traceCableTrayConduitsInRoom = (traceNetwork != null
                ? CableTrayConduits.Where(x => Equals(x.TraceNetwork, traceNetwork) && x.Rooms.Contains(room))
                : CableTrayConduits.Where(x => x.Rooms.Contains(room)))
            .ToList();

        if (!traceCableTrayConduitsInRoom.Any())
            return new List<CableTrayConduitBaseProxy>();

        if (InRoom(element))
        {
            var traceCableTrayConduitsInWalls = traceNetwork != null
                ? CableTrayConduits.Where(x => Equals(x.TraceNetwork, traceNetwork) && OutsideRoom(x) && x.Levels.Contains(element.Level) && element.LocationPoint.RatingDistanceToSegment(x.Point1, x.Point2) < 0.1)
                : CableTrayConduits.Where(x => OutsideRoom(x) && x.Levels.Contains(element.Level));

            traceCableTrayConduitsInRoom.AddRange(traceCableTrayConduitsInWalls);
        }

        return GetTraceCableTrayConduitServiceType(traceCableTrayConduitsInRoom, element);
    }

    internal List<CableTrayConduitBaseProxy> FindTraceElementsInLevel(ElectricalElementProxy element, TraceNetwork traceNetwork = null)
    {
        var level = element.Level;

        var traceCableTrayConduitsInLevel = (traceNetwork != null
                ? CableTrayConduits.Where(n => Equals(n.TraceNetwork, traceNetwork) && n.Levels.Contains(level))
                : CableTrayConduits.Where(n => n.Levels.Contains(level)))
            .ToList();

        return GetTraceCableTrayConduitServiceType(traceCableTrayConduitsInLevel, element);
    }

    private static List<CableTrayConduitBaseProxy> GetTraceCableTrayConduitServiceType(
        List<CableTrayConduitBaseProxy> traceCableTrayConduits,
        ElectricalElementProxy element)
    {
        var serviceType = element.ServiceType;

        if (string.IsNullOrEmpty(serviceType))
            return traceCableTrayConduits;

        var traceCableTrayConduitsServiceType = traceCableTrayConduits
            .Where(n => n.ServiceType == serviceType)
            .ToList();

        return traceCableTrayConduitsServiceType.Any()
            ? traceCableTrayConduitsServiceType
            : traceCableTrayConduits;
    }

    private static CableTrayConduitBaseProxy FindNearestTraceElement(
        XYZProxy xyz, List<CableTrayConduitBaseProxy> traceElements, int idTraceNetwork = -1)
    {
        if (idTraceNetwork >= 0 &&
            false == traceElements.Any(n => (n.TraceNetwork?.Id ?? -1) == idTraceNetwork))
            return null;

        CableTrayConduitBaseProxy nearestTraceElement = null;

        var minRatingDistance = double.PositiveInfinity;

        foreach (var traceElement in traceElements)
        {
            var ratingDistance = xyz.RatingDistanceToSegment(traceElement.Point1, traceElement.Point2);

            if (ratingDistance >= minRatingDistance)
                continue;

            minRatingDistance = ratingDistance;
            nearestTraceElement = traceElement;
        }

        return nearestTraceElement;
    }

    internal static ElectricalElementProxy FindNearestElement(
        List<ElectricalElementProxy> elements,
        ElectricalElementProxy currentElement)
    {
        if (currentElement?.LocationPoint == null || !elements.Any())
            return null;

        ElectricalElementProxy nearestElement = null;

        var currentPoint = currentElement.LocationPoint; 
        var minRatingDistance = // currentElement.TraceBinding is CableTrayConduitBaseProxy cableTrayConduit ? currentPoint.RatingDistanceToSegment(cableTrayConduit.Point1, cableTrayConduit.Point2) :
            double.PositiveInfinity;
        var elementsSystemType = GetElementsSystemType(elements, currentElement);

        foreach (var element in elementsSystemType)
        {
            if (element.RevitId == currentElement.RevitId)
                continue;

            var ratingDistance = currentPoint.RatingDistanceTo(element.LocationPoint);

            if (ratingDistance >= minRatingDistance)
                continue;

            minRatingDistance = ratingDistance;
            nearestElement = element;
        }

        return nearestElement;
    }

    private static List<ElectricalElementProxy> GetElementsSystemType(
        List<ElectricalElementProxy> elements,
        ElectricalElementProxy currentElement)
    {
        var serviceType = currentElement.ServiceType;

        if (string.IsNullOrEmpty(serviceType))
            return elements;

        var elementsSystemType = elements
            .Where(n => n.ServiceType == serviceType && n.RevitId != currentElement.RevitId)
            .ToList();

        return elementsSystemType.Any()
            ? elementsSystemType
            : elements;
    }

    internal ElectricalElementProxy FindFarthestElement(IList<ElectricalElementProxy> elements,
        ElectricalElementProxy currentElement)
    {
        if (currentElement?.LocationPoint == null || !elements.Any())
            return null;

        ElectricalElementProxy nearestElement = null;

        var maxRatingDistance = 0d;
        var currentPoint = currentElement.LocationPoint;

        foreach (var element in elements)
        {
            if (element.RevitId == currentElement.RevitId)
                continue;

            var ratingDistance = currentPoint.RatingDistanceTo(element.LocationPoint);

            if (ratingDistance <= maxRatingDistance)
                continue;

            maxRatingDistance = ratingDistance;
            nearestElement = element;
        }

        return nearestElement;
    }

    private static bool InRoom(ElectricalElementProxy element) => element.Room is { RevitId: > 0 };
    private static bool OutsideRoom(CableTrayConduitBaseProxy element) => !element.Rooms.Any() || element.Rooms.All(x => x.RevitId < 0);
}