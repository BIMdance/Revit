namespace BIMdance.Revit.ObsoleteAPI;

public static partial class Revit2016
{
    public static class ElementId
    {
        public static Autodesk.Revit.DB.ElementId NewElementId(int id) => new(id);
        public static long GetValue(Autodesk.Revit.DB.ElementId elementId) => elementId.IntegerValue;
    }
}