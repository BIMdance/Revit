using Autodesk.Revit.DB;

namespace BIMdance.Revit.ObsoleteAPI;

public class OverrideGraphicSettingsRevit2016
{
    public static void SetSurfaceForegroundPatternColor(
        OverrideGraphicSettings overrideGraphicSettings,
        Color color)
    {
        overrideGraphicSettings.SetProjectionFillColor(color);
    }

    public static void SetSurfaceForegroundPatternId(
        OverrideGraphicSettings overrideGraphicSettings,
        ElementId elementId)
    {
        overrideGraphicSettings.SetProjectionFillPatternId(elementId);
    }

    public static void SetSurfaceForegroundPatternVisible(
        OverrideGraphicSettings overrideGraphicSettings,
        bool fillPatternVisible)
    {
        overrideGraphicSettings.SetProjectionFillPatternVisible(fillPatternVisible);
    }
}