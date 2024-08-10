namespace BIMdance.Revit.ObsoleteAPI;

public static partial class Revit2016
{
    public static class Formats
    {
        public static FormatOptions GetFormatOptions(int displayUnitType) =>
            new((DisplayUnitType)displayUnitType);
    }
}