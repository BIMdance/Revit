namespace BIMdance.Revit.Utils;

internal static class RevitConnectorUtils
{
    internal static Connector GetRefConnector(this Connector conn)
    {
        return conn.AllRefs
            .OfType<Connector>()
            .FirstOrDefault(connRef => connRef.Owner.Id != conn.Owner.Id);
    }
    
    internal static IEnumerable<Connector> GetConnectors(Element element, Domain domain = Domain.DomainUndefined)
    {
        return domain != Domain.DomainUndefined
            ? GetConnectorManager(element)?.Connectors.OfType<Connector>().Where(n => n.Domain == domain)
            : GetConnectorManager(element)?.Connectors.OfType<Connector>();
    }

    internal static ConnectorManager GetConnectorManager(Element element) => element switch
    {
        CableTrayConduitBase cableTrayConduitBase => cableTrayConduitBase.ConnectorManager,
        FamilyInstance familyInstance => familyInstance.MEPModel?.ConnectorManager,
        _ => null
    };
}