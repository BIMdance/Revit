namespace BIMdance.Revit.Utils.Revit;

public static class VisibilityUtils
{
    private const int CoarseBitPosition = 13;
    private const int MediumBitPosition = 14;
    private const int FineBitPosition = 15;
    
    public static void SetVisibility(this Element element, params ViewDetailLevel[] viewDetailLevels)
    {
        var visibilityParameter = element is SymbolicCurve symbolicCurve
            ? symbolicCurve.get_Parameter(BuiltInParameter.CURVE_VISIBILITY_PARAM)
            : element.get_Parameter(BuiltInParameter.GEOM_VISIBILITY_PARAM);

        if (visibilityParameter is null) throw new NullReferenceException($"{nameof(visibilityParameter)} wasn't found in {nameof(Element)}: {element.Name}");

        var visibilityValue = visibilityParameter.AsInteger();

        if (viewDetailLevels is null || viewDetailLevels.IsEmpty())
        {
            Switch(ref visibilityValue, CoarseBitPosition, true);
            Switch(ref visibilityValue, MediumBitPosition, true);
            Switch(ref visibilityValue, FineBitPosition, true);
        }
        else
        {
            Switch(ref visibilityValue, CoarseBitPosition, viewDetailLevels.Contains(ViewDetailLevel.Coarse));
            Switch(ref visibilityValue, MediumBitPosition, viewDetailLevels.Contains(ViewDetailLevel.Medium));
            Switch(ref visibilityValue, FineBitPosition, viewDetailLevels.Contains(ViewDetailLevel.Fine));
        }

        visibilityParameter.Set(visibilityValue);
    }
    
    private static void Switch(ref int x, int position, bool value)
    {
        if (value) SwitchOn(ref x, position);
        else SwitchOff(ref x, position);
    }
    
    private static void SwitchOn(ref int x, int position) => x |= 1 << position;

    private static void SwitchOff(ref int x, int position) => x &= ~(1 << position);
}