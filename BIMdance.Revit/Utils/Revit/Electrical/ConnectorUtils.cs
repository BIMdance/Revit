namespace BIMdance.Revit.Utils.Revit.Electrical;

public static class ConnectorUtils
{
    public static Connector GetConnector(this FamilyInstance familyInstance, Domain domain = Domain.DomainUndefined) =>
        GetConnectors(familyInstance, domain)
            .OrderBy(n => n.Id)
            .FirstOrDefault();

    public static Connector GetConnector(this FamilyInstance familyInstance, ElectricalSystem electricalSystem)
    {
        return GetConnectors(electricalSystem)
            .FirstOrDefault(n =>
                n.AllRefs != null &&
                n.AllRefs.OfType<Connector>().First().Owner.Id == familyInstance.Id)
            ?.AllRefs.OfType<Connector>()
            .First();
    }

    public static Connector GetPrimaryElectricalConnector(this Element element) => element switch
    {
        FamilyInstance => element.GetConnectors().FirstOrDefault(n => n.Domain == Domain.DomainElectrical && n.GetMEPConnectorInfo().IsPrimary),
        _ => null
    };
    
    public static IEnumerable<Connector> GetConnectors(this Element element, Domain domain = Domain.DomainUndefined)
    {
        var connectors = element.GetConnectorManager()?.Connectors.OfType<Connector>();

        if (connectors == null)
            return new List<Connector>();

        return domain is not Domain.DomainUndefined
            ? connectors.Where(n => n.Domain == domain)
            : connectors;
    }

    public static ConnectorManager GetConnectorManager(this Element element) => element switch
    {
        CableTrayConduitBase cableTrayConduitBase => cableTrayConduitBase.ConnectorManager,
        FamilyInstance familyInstance => familyInstance.MEPModel?.ConnectorManager,
        ElectricalSystem electricalSystem => electricalSystem.ConnectorManager,
        _ => null
    };

    public static ConnectorElement GetConnectorElement(this Connector connector, Document familyDocument = null)
    {
        if (connector.Owner is not FamilyInstance familyInstance)
            return null;

        var connectorIndexes = familyInstance.GetConnectors().Select(n => n.Id).OrderBy(n => n).ToList();
        var id = connector.Id;
        var indexId = connectorIndexes.IndexOf(id);

        familyDocument ??=
            familyInstance.Document.IsModifiable ||
            familyInstance.Document.IsReadOnly
                ? null
                : familyInstance.Document.EditFamily(familyInstance.Symbol.Family);

        if (familyDocument == null)
            return null;

        var connectorElements = new FilteredElementCollector(familyDocument)
            .WhereElementIsNotElementType()
            .OfClass(typeof(ConnectorElement))
            .OfType<ConnectorElement>()
            .OrderBy(n => n.Id.GetValue())
            .ToList();

        return indexId < connectorElements.Count
            ? connectorElements[indexId]
            : null;
    }

    public static int GetValueAsInteger(
        this ConnectorElement connectorElement,
        FamilyInstance familyInstance,
        BuiltInParameter builtInParameter)
    {
        var parameter = connectorElement.GetConnectorElementParameter(familyInstance, builtInParameter);
        return parameter?.GetValueAsInt() ?? 0;
    }

    public static double GetVoltAmperesFromInternal(
        this ConnectorElement connectorElement,
        FamilyInstance familyInstance,
        BuiltInParameter builtInParameter)
    {
        var parameter = connectorElement.GetConnectorElementParameter(familyInstance, builtInParameter);
        var valueInternal = parameter?.GetValueAsDouble() ?? 0;
        return valueInternal.Convert(Converting.VoltAmperesFromInternal);
    }

    public static double GetVoltAmperesFromInternal(
        this MEPFamilyConnectorInfo connectorInfo,
        BuiltInParameter builtInParameter)
    {
        var parameterValue = connectorInfo.GetConnectorParameterValue(new ElementId(builtInParameter));
        var valueInternal = ((DoubleParameterValue)parameterValue)?.Value ?? 0;
        return valueInternal.Convert(Converting.VoltAmperesFromInternal);
    }

    public static int GetIntegerValue(
        this MEPFamilyConnectorInfo connectorInfo,
        BuiltInParameter builtInParameter)
    {
        var parameterValue = connectorInfo?.GetConnectorParameterValue(new ElementId(builtInParameter));
        return ((IntegerParameterValue)parameterValue)?.Value ?? 0;
    }

    public static ElementId GetElementIdValue(
        this MEPFamilyConnectorInfo connectorInfo,
        BuiltInParameter builtInParameter)
    {
        var parameterValue = connectorInfo?.GetConnectorParameterValue(new ElementId(builtInParameter));
        return ((ElementIdParameterValue)parameterValue)?.Value ?? ElementId.InvalidElementId;
    }

    public static Parameter GetConnectorElementParameter(
        this ConnectorElement connectorElement,
        FamilyInstance familyInstance,
        BuiltInParameter builtInParameter)
    {
        var connectorParameter = connectorElement?.get_Parameter(builtInParameter);

        if (connectorParameter == null)
            return null;

        var fDoc = connectorElement.Document;
        var associatedParameter = fDoc.FamilyManager.GetAssociatedFamilyParameter(connectorParameter);

        return associatedParameter switch
        {
            null => connectorParameter,
            _ => associatedParameter.IsShared
                ? GetAssociatedParameterShared(familyInstance, associatedParameter)
                : GetAssociatedParameter(familyInstance, associatedParameter)
        };
    }

    private static Parameter GetAssociatedParameterShared(
        FamilyInstance familyInstance,
        FamilyParameter associatedParameter)
    {
        var guid = associatedParameter.GUID;

        return associatedParameter.IsInstance
            ? familyInstance.get_Parameter(guid)
            : familyInstance.Symbol.get_Parameter(guid);
    }

    private static Parameter GetAssociatedParameter(FamilyInstance familyInstance,
        FamilyParameter associatedParameter)
    {
        var name = associatedParameter.Definition?.Name;

        return associatedParameter.IsInstance
            ? familyInstance.LookupParameter(name)
            : familyInstance.Symbol.LookupParameter(name);
    }

    public static ConnectorElement GetConnectorElementParameters(this Connector connector, Document familyDocument = null)
    {
        if (connector.Owner is not FamilyInstance familyInstance)
            return null;

        var connectorIndexes = familyInstance.GetConnectors().Select(n => n.Id).OrderBy(n => n).ToList();
        var id = connector.Id;
        var indexId = connectorIndexes.IndexOf(id);
        var familySymbol = familyInstance.Symbol;
        var family = familySymbol.Family;

        familyDocument ??= familyInstance.Document.EditFamily(family);

        var connectorElements = new FilteredElementCollector(familyDocument)
            .WhereElementIsNotElementType()
            .OfClass(typeof(ConnectorElement))
            .OfType<ConnectorElement>()
            .OrderBy(n => n.Id.GetValue())
            .ToList();

        var connectorElement = indexId < connectorElements.Count
            ? connectorElements[indexId]
            : null;

        // TODO : | Petrov | Определить параметры влияющие на соединитель из семейства.

        if (connectorElement == null)
            return null;

        switch (connectorElement.SystemClassification)
        {
            case MEPSystemClassification.PowerBalanced:

                break;

            case MEPSystemClassification.PowerUnBalanced:

                break;
        }

        return connectorElement;
    }

    public static Connector GetRefConnector(this Connector connector, Domain domain) =>
        connector.Domain != Domain.DomainUndefined &&
        connector.Domain == domain &&
        connector.ConnectorType != ConnectorType.MasterSurface &&
        connector.IsConnected
            ? GetRefConnector(connector)
            : null;

    public static Element GetRefElement(this Connector connector, Domain domain) =>
        connector.Domain is not Domain.DomainUndefined &&
        connector.Domain == domain &&
        connector.ConnectorType is not ConnectorType.MasterSurface &&
        connector.IsConnected
            ? GetRefElement(connector)
            : null;
    
    public static Connector GetRefConnector(this Connector connector) => connector.AllRefs
        .OfType<Connector>()
        .FirstOrDefault(connRef => connRef.Owner.Id != connector.Owner.Id);

    public static Element GetRefElement(this Connector connector) =>
        (from Connector connReference in connector.AllRefs
            where connReference.Owner.Id != connector.Owner.Id
            select connReference.Owner)
        .FirstOrDefault();
}