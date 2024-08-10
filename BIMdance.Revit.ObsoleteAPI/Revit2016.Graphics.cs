namespace BIMdance.Revit.ObsoleteAPI;

public static partial class Revit2016
{
    public static class Graphics
    {
        public static void SetSurfaceForegroundPatternColor(
            OverrideGraphicSettings overrideGraphicSettings,
            Color color) =>
            overrideGraphicSettings.SetProjectionFillColor(color);

        public static void SetSurfaceForegroundPatternId(
            OverrideGraphicSettings overrideGraphicSettings,
            Autodesk.Revit.DB.ElementId elementId) =>
            overrideGraphicSettings.SetProjectionFillPatternId(elementId);

        public static void SetSurfaceForegroundPatternVisible(
            OverrideGraphicSettings overrideGraphicSettings,
            bool fillPatternVisible) =>
            overrideGraphicSettings.SetProjectionFillPatternVisible(fillPatternVisible);
    }
}
