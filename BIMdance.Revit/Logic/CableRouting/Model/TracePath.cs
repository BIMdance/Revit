namespace BIMdance.Revit.Logic.CableRouting.Model;

public class TracePath
{
    public TracePath(
        ElectricalElementProxy electricalElement,
        TracePathType tracePathType = TracePathType.ByCableTrayConduit,
        bool directPathToBaseEquipment = false)
    {
        TracePathType = tracePathType;
        ElectricalElement = electricalElement;
        DirectPathToBaseEquipment = directPathToBaseEquipment;
        Connectors = new List<ConnectorProxy>();
        TraceElements = new HashSet<TraceElement>();
        ConnectorTracks = new Dictionary<ConnectorProxy, double[]>();
    }

    public TracePath(
        ElectricalElementProxy electricalElement, ConnectorProxy connector) : this(electricalElement)
    {
        Connectors.Add(connector);
        TraceElements.Add(connector.Element);
        ConnectorTracks.Add(connector, new[] { 0d, 0d });
    }

    private TracePath(TracePath tracePath)
    {
        TracePathType = tracePath.TracePathType;
        Distance = tracePath.Distance;
        DistanceToBinding = tracePath.DistanceToBinding;
        DistanceToBaseEquipmentBinding = tracePath.DistanceToBaseEquipmentBinding;
        DistanceInCableTray = tracePath.DistanceInCableTray;
        Connectors = new List<ConnectorProxy>(tracePath.Connectors);
        TraceElements = new HashSet<TraceElement>(tracePath.TraceElements);
        ConnectorTracks = new Dictionary<ConnectorProxy, double[]>(tracePath.ConnectorTracks);

        // DirectPathToBaseEquipment = tracePath.DirectPathToBaseEquipment; - нельзя использовать
        // DirectPathToBaseEquipment == true должен быть только один
        // его нельзя копировать в другие экземпляры
        // у остальных экземпляров DirectPathToBaseEquipment должен быть false
    }

    public ElectricalElementProxy ElectricalElement { get; set; }
    public bool DirectPathToBaseEquipment { get; }
    public TracePathType TracePathType { get; set; }
    public double Distance { get; set; }
    public double DistanceToBinding { get; set; }
    public double DistanceToBaseEquipmentBinding { get; set; }
    public double DistanceInCableTray { get; set; }
    public List<ConnectorProxy> Connectors { get; }
    public HashSet<TraceElement> TraceElements { get; }
    public Dictionary<ConnectorProxy, double[]> ConnectorTracks { get; }

    public void AddDistance(double distance, TraceElement traceElement)
    {
        this.Distance += distance;

        switch (traceElement)
        {
            case CableTrayProxy _:
            case CableTrayConduitFittingProxy _:
                this.DistanceInCableTray += distance;
                break;
        }
    }

    public void AddConnector(ConnectorProxy traceConnector) //, double distance)
    {
        Connectors.Add(traceConnector);
    }

    public void AddTrackConnector(ConnectorProxy connector, double distance, double distanceInCableTray)
    {
        if (connector != null &&
            !ConnectorTracks.ContainsKey(connector))
            ConnectorTracks.Add(connector, new[] { distance, distanceInCableTray });
    }

    public ConnectorProxy GetNextTraceConnector(ConnectorProxy connector) //, ref double distance)
    {
        var traceElement = connector?.Element;
        var nextConnector = traceElement?.GetNextConnector(connector);
        AddDistance(connector?.DistanceTo(nextConnector ?? connector) ?? 0, traceElement);
        AddTrackConnector(nextConnector, Distance, DistanceInCableTray);

        return nextConnector?.RefConnector;
    }

    public TracePath Clone()
    {
        return new TracePath(this);
    }

    public TracePath SetTrace(ElectricalSystemProxy electricalSystem, ConnectorProxy inTraceConnector, ConnectionTopology topology)
    {
        var baseEquipment = electricalSystem.BaseEquipment;
        var toBaseEquipmentConnector = GetNextTraceConnectorInTraceElement(electricalSystem, inTraceConnector, topology);
        var connectorTracks = this.ConnectorTracks;

        if (toBaseEquipmentConnector == null) throw new NullReferenceException(nameof(toBaseEquipmentConnector));

        if (!connectorTracks.Any())
            return this;

        var connectorTracePath = toBaseEquipmentConnector.GetTracePathToBaseEquipment(baseEquipment);
        var totalDistance = this.Distance;
        var totalDistanceInCableTray = this.DistanceInCableTray;
        var newCircuitForThisTraceConnector =
            !toBaseEquipmentConnector.IsTraceConnectorOfElectricalCircuit(electricalSystem);
        var traceElements = new HashSet<TraceElement>();

        if (connectorTracePath == null) throw new NullReferenceException(nameof(connectorTracePath));

        if (newCircuitForThisTraceConnector)
            toBaseEquipmentConnector.AddElectricalCircuit(electricalSystem);

        if (newCircuitForThisTraceConnector ||
            topology == ConnectionTopology.Star)
        {
            this.DistanceInCableTray += connectorTracePath.DistanceInCableTray;
        }

        traceElements.AddRange(connectorTracePath.TraceElements);
        traceElements.Add(toBaseEquipmentConnector.Element);

        this.DistanceToBaseEquipmentBinding += connectorTracePath.DistanceToBaseEquipmentBinding + totalDistance;

        for (var i = connectorTracks.Count - 1; i >= 0; i--)
        {
            var connectorTrack = connectorTracks.ElementAt(i);
            var connector = connectorTrack.Key;
            var traceElement = connector.Element;
            var trackDistance = connectorTrack.Value[0];
            var trackDistanceInCableTray = connectorTrack.Value[1];
            var distance = totalDistance - trackDistance;
            var distanceInCableTray = totalDistanceInCableTray - trackDistanceInCableTray;
            var cloneConnectorTracePath = connectorTracePath.Clone();

            traceElements.Add(traceElement);
            cloneConnectorTracePath.TraceElements.Clear();
            cloneConnectorTracePath.TraceElements.AddRange(traceElements);
            cloneConnectorTracePath.Distance = distance;
            cloneConnectorTracePath.DistanceToBaseEquipmentBinding += distance;
            cloneConnectorTracePath.DistanceInCableTray += distanceInCableTray;
            connector.AddTracePathToBaseEquipment(electricalSystem, cloneConnectorTracePath);

            SetToBaseEquipmentConnectorsInCableTrayConduit(electricalSystem, connector, cloneConnectorTracePath);
        }

        this.TraceElements.AddRange(traceElements.Reverse());

        return this;
    }

    private ConnectorProxy GetNextTraceConnectorInTraceElement(
        ElectricalSystemProxy electricalCircuit, ConnectorProxy inTraceConnector, ConnectionTopology topology)
    {
        var baseEquipment = electricalCircuit.BaseEquipment;
        var traceNetworkElement = inTraceConnector.Element;
        var toBaseEquipmentConnector = traceNetworkElement.GetTraceConnectorToBaseEquipment(baseEquipment);
        var distanceBetweenInAndToBaseEquipment = inTraceConnector.DistanceTo(toBaseEquipmentConnector);
        var nextTraceConnector = toBaseEquipmentConnector;
        var distanceToNextTraceConnector = distanceBetweenInAndToBaseEquipment;
        var traceConnectors = traceNetworkElement.Connectors
            .Where(n => n.IsTraceConnectorOfElectricalCircuit(electricalCircuit)).ToList();

        foreach (var currentConnector in traceConnectors)
        {
            if (currentConnector.Id == inTraceConnector.Id ||
                currentConnector.Id == toBaseEquipmentConnector.Id)
                continue;

            var distanceBetweenCurrentAndIn = currentConnector.DistanceTo(inTraceConnector);

            if (distanceBetweenCurrentAndIn.Equals(0))
                return currentConnector;

            var distanceBetweenCurrentAndToBaseEquipment = currentConnector.DistanceTo(toBaseEquipmentConnector);

            if (InBetweenCurrentAndToBaseEquipment(distanceBetweenInAndToBaseEquipment, distanceBetweenCurrentAndIn,
                    distanceBetweenCurrentAndToBaseEquipment))
            {
                if (traceNetworkElement is CableTrayConduitBaseProxy &&
                    topology == ConnectionTopology.Star)
                    this.DistanceInCableTray += distanceBetweenInAndToBaseEquipment;

                this.DistanceToBaseEquipmentBinding = distanceBetweenInAndToBaseEquipment;

                return toBaseEquipmentConnector;
            }

            if (CurrentBetweenInAndToBaseEquipment(distanceBetweenInAndToBaseEquipment, distanceBetweenCurrentAndIn,
                    distanceBetweenCurrentAndToBaseEquipment))
            {
                if (distanceBetweenCurrentAndIn > distanceToNextTraceConnector)
                    continue;

                distanceToNextTraceConnector = distanceBetweenCurrentAndIn;
                nextTraceConnector = currentConnector;
            }
        }

        this.AddDistance(inTraceConnector.DistanceTo(nextTraceConnector), traceNetworkElement);

        return nextTraceConnector;
    }

    private static bool InBetweenCurrentAndToBaseEquipment(
        double distanceBetweenInAndToBaseEquipment,
        double distanceBetweenCurrentAndIn,
        double distanceBetweenCurrentAndToBaseEquipment)
    {
        return distanceBetweenCurrentAndToBaseEquipment > distanceBetweenCurrentAndIn &&
               distanceBetweenCurrentAndToBaseEquipment > distanceBetweenInAndToBaseEquipment;
    }

    private static bool CurrentBetweenInAndToBaseEquipment(
        double distanceBetweenInAndToBaseEquipment,
        double distanceBetweenCurrentAndIn,
        double distanceBetweenCurrentAndToBaseEquipment)
    {
        return distanceBetweenInAndToBaseEquipment > distanceBetweenCurrentAndIn &&
               distanceBetweenInAndToBaseEquipment > distanceBetweenCurrentAndToBaseEquipment;
    }

    private static void SetToBaseEquipmentConnectorsInCableTrayConduit(
        ElectricalSystemProxy electricalCircuit,
        ConnectorProxy traceConnector,
        TracePath trackConnectorTracePath)
    {
        var refTraceConnector = traceConnector.RefConnector;
        var refTraceElement = refTraceConnector?.Element;

        if (refTraceElement is CableTrayConduitBaseProxy &&
            refTraceElement.ConnectorsCount > 2)
        {
            var tracePathToBaseEquipment = trackConnectorTracePath.Clone();

            RemoveInvalidBranch(tracePathToBaseEquipment, refTraceElement);

            refTraceConnector.AddTracePathToBaseEquipment(electricalCircuit, tracePathToBaseEquipment);
        }
    }

    /// <summary>
    /// В коннекторах ([o], [x]) происходит кеширование данных о ранее посчитанных трассах.
    /// Если несколько коробов ([1], [2]) подключено к кабельному лотку, и была посчитана трасса по коробу [1],
    /// то при расчёте трассы по коробу [2] поиск пути будет происходить до коннектора [o].
    /// Информация об остальном пути до панели будет взята из коннектора [o].
    /// Но при этом иногда возникает ситуация, когда в список элементов трассы в коннекторе [o] попадает короб [1],
    /// который не имеет отношения к трассе [2].
    /// Для избежания неверной трассировки необходимо проверять наличие подобных отводов трассы и удалять их.
    /// </summary>
    /// -----------------------------
    ///   - - -[o]         [x]       
    /// --------|-----------|--------
    ///         |           |
    ///        [1]         [2]
    /// <param name="tracePathToBaseEquipment"></param>
    /// <param name="refTraceElement"></param>
    private static void RemoveInvalidBranch(TracePath tracePathToBaseEquipment, TraceElement refTraceElement)
    {
        var lastTraceElement = tracePathToBaseEquipment.TraceElements.Last();

        if (lastTraceElement.RevitId != refTraceElement.RevitId)
            tracePathToBaseEquipment.TraceElements.Remove(lastTraceElement);
    }

    public override string ToString()
    {
        return $"{nameof(TracePath)}" +
               $"\n\tDistance: {Distance}" +
               // $"\n\tDistanceToBaseEquipment: {DistanceToBaseEquipment}" +
               $"\n\tTraceConnectorsTrack:\n\t\t{string.Join("\n\t\t", ConnectorTracks.Select(n => $"{n.Key} - {n.Value}"))}";
    }
}