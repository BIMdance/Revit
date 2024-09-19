namespace BIMdance.Revit.Model.Electrical.Base;
public class BlankSource : ElectricalBase
{
    public BlankSource(int revitId) : base(revitId, null) =>
        SetBaseConnector(new ConnectorProxy(this, 1, ElectricalSystemTypeProxy.UndefinedSystemType));
}