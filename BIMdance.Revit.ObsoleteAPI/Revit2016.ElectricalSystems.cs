namespace BIMdance.Revit.ObsoleteAPI;

public static partial class Revit2016
{
    public static class ElectricalSystems
    {
        public static ElectricalSystem Create(
            Document document,
            Connector connector,
            ElectricalSystemType electricalSystemType) =>
            document.Create.NewElectricalSystem(
                connector,
                electricalSystemType);

        public static ElectricalSystem Create(
            Document document,
            ICollection<Autodesk.Revit.DB.ElementId> elementIds,
            ElectricalSystemType electricalSystemType) =>
            document.Create.NewElectricalSystem(
                elementIds,
                electricalSystemType);
    }
}
