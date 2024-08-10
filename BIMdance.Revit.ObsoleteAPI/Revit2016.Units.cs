namespace BIMdance.Revit.ObsoleteAPI;

public static partial class Revit2016
{
    public static class Units
    {
        public static double ConvertFromInternalUnits(Document document, double value, int unitTypeIndex)
        {
            var displayUnitType = DisplayUnitType(document, unitTypeIndex);
            return UnitUtils.ConvertFromInternalUnits(value, displayUnitType);
        }
        
        public static double ConvertToInternalUnits(Document document, double value, int unitTypeIndex)
        {
            var displayUnitType = DisplayUnitType(document, unitTypeIndex);
            return UnitUtils.ConvertToInternalUnits(value, displayUnitType);
        }

        public static int GetDisplayUnitType(Document document, int unitTypeIndex) =>
            (int)DisplayUnitType(document, unitTypeIndex);

        private static DisplayUnitType DisplayUnitType(Document document, int unitTypeIndex) =>
            document.GetUnits().GetFormatOptions((UnitType)unitTypeIndex).DisplayUnits;
    }
}