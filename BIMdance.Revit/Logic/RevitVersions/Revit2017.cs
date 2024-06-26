namespace BIMdance.Revit.Logic.RevitVersions;

internal static class Revit2017
{
    internal static void SetProjectionLinePatternId(
        Category category, ElementId linePatternId)
    {
        category.SetLinePatternId(linePatternId, GraphicsStyleType.Projection);
    }
}