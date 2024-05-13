using Autodesk.Revit.DB;

namespace BIMdance.Revit.ObsoleteAPI;

public static class ElementId2016
{
    public static ElementId NewElementId(int id) => new(id);
    public static long GetValue(ElementId elementId) => elementId.IntegerValue;
}