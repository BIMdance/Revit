namespace BIMdance.Revit.Logic.CableRouting;

public class NetworkConverter : ProxyConverter<TraceNetwork>
{
    private readonly BuildingProxy _building;
    private readonly Stack<Element> _stackElements = new();
    private Dictionary<long, CableTrayConduitBase> _cableTrayConduits;
    
    public NetworkConverter(Document document, BuildingProxy building) : base(document)
    {
        _building = building;
    }

    public override List<TraceNetwork> Convert()
    {
        var cableTrays = new FilteredElementCollector(Document).OfClass(typeof(CableTray)).WhereElementIsNotElementType().OfType<CableTray>();
        var conduits = new FilteredElementCollector(Document).OfClass(typeof(Conduit)).WhereElementIsNotElementType().OfType<Conduit>();
        _cableTrayConduits = cableTrays.OfType<CableTrayConduitBase>().Concat(conduits).ToDictionary(x => x.Id.GetValue());
        
        var traceNetworks = new List<TraceNetwork>();
        var idTraceNetwork = 0;
        
        while (_cableTrayConduits.Any())
        {
            var firstCableTrayConduit = _cableTrayConduits.ElementAt(0);
            _cableTrayConduits.Remove(firstCableTrayConduit.Key);

            traceNetworks.Add(Build(idTraceNetwork++, firstCableTrayConduit.Value));
        }
        
        return traceNetworks;
    }

    private TraceNetwork Build(int idTraceNetwork, CableTrayConduitBase firstCableTrayConduit)
    {
        var traceNetwork = new TraceNetwork(idTraceNetwork);
        _stackElements.Push(firstCableTrayConduit);

        while (_stackElements.Any())
        {
            var currentElement = _stackElements.Pop();
            _cableTrayConduits.Remove(currentElement.Id.GetValue());
            var traceElement = GetTraceElement(traceNetwork, currentElement);
            var connectors = currentElement.GetConnectors(Domain.DomainCableTrayConduit).Where(x => x.IsConnected).OrderBy(n => n.Id);
            
            foreach (var connector in connectors)
                AddTraceElement(traceNetwork, traceElement, connector);
        }

        return traceNetwork;
    }

    private void AddTraceElement(TraceNetwork traceNetwork, TraceElement traceElement, Connector connector)
    {
        if (connector.ConnectorType is ConnectorType.MasterSurface ||
            !connector.IsConnected ||
            traceElement.ContainsConnector(connector.Id))
            return;

        var traceConnector = RevitMapper.Map<Connector, TraceConnectorProxy>(connector);
        traceElement.AddConnector(traceConnector);

        var refConnector = connector.GetRefConnector();
        
        if (refConnector is null)
            return;

        var refElement = refConnector.Owner;
        var refElementInNetwork = traceNetwork.ElementInNetwork(refElement.Id.GetValue());
        var refTraceElement = GetTraceElement(traceNetwork, refElement);
        
        switch (traceElement)
        {
            case CableTrayConduitBaseProxy cableTrayConduitProxy when refTraceElement is TraceElectricalElementProxy electricalElement:
                electricalElement.AddTraceElement(cableTrayConduitProxy);
                break;

            case TraceElectricalElementProxy electricalElement when refTraceElement is CableTrayConduitBaseProxy cableTrayConduitProxy:
                electricalElement.AddTraceElement(cableTrayConduitProxy);
                break;

            case TraceElectricalElementProxy electricalElement when refTraceElement is not null:
                electricalElement.AddTraceElement(refTraceElement);
                break;

            case not null when refTraceElement is TraceElectricalElementProxy electricalElement:
                electricalElement.AddTraceElement(traceElement);
                break;
        }

        if (refTraceElement is null ||
            refTraceElement.ContainsConnector(refConnector.Id))
            return;

        var refConnectorProxy = RevitMapper.Map<Connector, TraceConnectorProxy>(refConnector);
        
        traceConnector.AddRefConnector(refConnectorProxy);
        refTraceElement.AddConnector(refConnectorProxy);

        if (!refElementInNetwork && !_stackElements.Contains(refElement))
            _stackElements.Push(refElement);
    }

    private TraceElement GetTraceElement(TraceNetwork traceNetwork, Element currentElement)
    {
        var traceElement = traceNetwork.GetElement(currentElement.Id.GetValue());

        if (traceElement is not null)
            return traceElement;
        
        traceElement = currentElement switch
        {
            CableTray cableTray => RevitMapper.Map<CableTray, CableTrayProxy>(cableTray),
            Conduit conduit => RevitMapper.Map<Conduit, ConduitProxy>(conduit),
            FamilyInstance familyInstance
                when (BuiltInCategory)familyInstance.Category.Id.GetValue()
                is BuiltInCategory.OST_CableTrayFitting
                or BuiltInCategory.OST_ConduitFitting =>
                RevitMapper.Map<FamilyInstance, CableTrayConduitFittingProxy>(familyInstance),
            FamilyInstance familyInstance => RevitMapper.Map<FamilyInstance, TraceElectricalElementProxy>(familyInstance),
            _ => throw new ArgumentOutOfRangeException(nameof(currentElement), currentElement, null)
        };

        if (traceElement is CableTrayConduitBaseProxy cableTrayConduit)
        {
            AddLevels(cableTrayConduit);
            AddRooms(cableTrayConduit);
        }
        
        return traceNetwork.AddElement(traceElement);
    }

    private void AddLevels(CableTrayConduitBaseProxy cableTrayConduit)
    {
        var levels = _building.GetLevels(cableTrayConduit.Point1, cableTrayConduit.Point2);

        foreach (var level in levels.Where(level => !cableTrayConduit.Levels.Contains(level)))
        {
            cableTrayConduit.Levels.Add(level);
        }
    }

    private void AddRooms(CableTrayConduitBaseProxy cableTrayConduit)
    {
        cableTrayConduit.Rooms.AddRange(_building.GetRooms(cableTrayConduit));
    }
}