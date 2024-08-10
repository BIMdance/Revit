namespace BIMdance.Revit.Utils.Revit.RevitVersions;

public static class Revit2018
{
    public static class Element
    {
        public static IList<ElementId> GetDependentElements<T>(Autodesk.Revit.DB.Element element) =>
            element.GetDependentElements(new ElementClassFilter(typeof(T)));
            
        public static IList<ElementId> GetDependentElements(Autodesk.Revit.DB.Element element, BuiltInCategory builtInCategory) =>
            element.GetDependentElements(new ElementCategoryFilter(builtInCategory));
    }
}