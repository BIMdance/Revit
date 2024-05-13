using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace BIMdance.Revit.ObsoleteAPI;

public class ParameterFilterElementRevit2016
{
    public static ParameterFilterElement Create(
        Document document,
        string name,
        ICollection<ElementId> idsCategories,
        IList<FilterRule> rules)
    {
        return ParameterFilterElement.Create(
            document,
            name,
            idsCategories,
            rules);
    }
}