using Color = Autodesk.Revit.DB.Color;

namespace BIMdance.Revit.Logic.RevitVersions;

internal static class Revit2019
{
    internal static ParameterFilterElement CreateParameterFilterElement(
        Document document, string name, ICollection<ElementId> categoryIds, IList<FilterRule> rules) =>
        ParameterFilterElement.Create(document, name, categoryIds, new ElementParameterFilter(rules));

    internal static void SetSurfaceForegroundPatternColor(
        OverrideGraphicSettings graphicSettings, Color color) =>
        graphicSettings.SetSurfaceForegroundPatternColor(color);

    internal static void SetSurfaceForegroundPatternId(
        OverrideGraphicSettings graphicSettings, FillPatternElement solidPattern) =>
        graphicSettings.SetSurfaceForegroundPatternId(solidPattern.Id);

    internal static void SetSurfaceForegroundPatternVisible(
        OverrideGraphicSettings graphicSettings, bool fillPatternVisible) =>
        graphicSettings.SetSurfaceForegroundPatternVisible(fillPatternVisible);
}