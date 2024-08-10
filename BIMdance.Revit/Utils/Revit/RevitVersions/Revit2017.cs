namespace BIMdance.Revit.Utils.Revit.RevitVersions;

internal static class Revit2017
{
    internal static class Geometry
    {
        internal static Plane CreatePlaneByNormalAndOrigin(
            XYZ normal,
            XYZ origin) =>
            Plane.CreateByNormalAndOrigin(normal, origin);
    }
    
    internal static class Graphics
    {
        internal static void SetProjectionLinePatternId(
            Category category, ElementId linePatternId) =>
            category.SetLinePatternId(linePatternId, GraphicsStyleType.Projection);
    }

    internal static class InternalDefinitions
    {
        internal static bool Equals(
            InternalDefinition internalDefinition1,
            InternalDefinition internalDefinition2) =>
            internalDefinition1.Id.GetValue() ==
            internalDefinition2.Id.GetValue();
    }
    
    internal static class Schedules
    {
        public static void SetTotalDisplayType(ScheduleField scheduleField)
        {
            if (scheduleField.CanTotal())
                scheduleField.DisplayType = ScheduleFieldDisplayType.Totals;
        }
    }
}