namespace BIMdance.Revit.ObsoleteAPI;

public static partial class Revit2016
{
    public static class Filters
    {
        public static ParameterFilterElement CreateParameterFilterElement(
            Document document,
            string name,
            ICollection<Autodesk.Revit.DB.ElementId> idsCategories,
            IList<FilterRule> rules) =>
            ParameterFilterElement.Create(
                document,
                name,
                idsCategories,
                rules);
    }
}