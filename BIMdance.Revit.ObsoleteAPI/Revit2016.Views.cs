using System.Collections.Generic;
using System.Linq;

namespace BIMdance.Revit.ObsoleteAPI;

public static partial class Revit2016
{
    public static class Views
    {
        public static List<Autodesk.Revit.DB.ElementId> GetGenericAnnotationElements(ViewSheet viewSheet) =>
            new FilteredElementCollector(viewSheet.Document)
                .OfCategory(BuiltInCategory.OST_GenericAnnotation)
                .Where(n => n.OwnerViewId == viewSheet.Id).Select(n => n.Id).ToList();
        
        public static IList<Autodesk.Revit.DB.ElementId> GetViewPlans(Level level) =>
            new FilteredElementCollector(level.Document)
                .OfClass(typeof(ViewPlan)).OfType<ViewPlan>()
                .Where(n => n.GenLevel.Id.IntegerValue == level.Id.IntegerValue)
                .Select(n => n.Id).ToList();
    }
}