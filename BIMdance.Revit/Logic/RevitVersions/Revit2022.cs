namespace BIMdance.Revit.Logic.RevitVersions;

internal static class Revit2022
{
    public static class FamilyManager
    {
        public static FamilyParameter AddParameter(Autodesk.Revit.DB.FamilyManager familyManager,
            string parameterName, ParameterTypeProxy parameterType, ParameterGroupProxy parameterGroup, bool isInstance)
        {
            var groupId = ForgeConverter.GetGroupId(parameterGroup);
            var typeId = ForgeConverter.GetParameterTypesId(parameterType);
            return familyManager.AddParameter(parameterName, groupId, typeId, isInstance);
        }

        public static FamilyParameter AddParameter(Autodesk.Revit.DB.FamilyManager familyManager,
            ExternalDefinition externalDefinition, ParameterGroupProxy parameterGroup, bool isInstance)
        {
            var groupId = ForgeConverter.GetGroupId(parameterGroup);
            return familyManager.AddParameter(externalDefinition, groupId, isInstance);
        }
    }

    public static class Parameters
    {
        public static ParameterGroupProxy GetParameterGroup(Parameter parameter)
        {
            return ForgeConverter.GetParameterGroup(parameter.Definition.GetGroupTypeId());
        }

        public static ParameterTypeProxy GetParameterType(Parameter parameter)
        {
            return ForgeConverter.GetParameterType(parameter.GetTypeId());
        }
        
        public static ParameterGroupProxy GetParameterGroup(FamilyParameter parameter)
        {
            return ForgeConverter.GetParameterGroup(parameter.Definition.GetGroupTypeId());
        }

        public static ParameterTypeProxy GetParameterType(FamilyParameter parameter)
        {
            return ForgeConverter.GetParameterType(parameter.GetUnitTypeId());
        }
    }
    
    public static class Units
    {
        public static double ConvertFromInternalUnits(Document document, double value, UnitTypeProxy unitType)
        {
            var unitTypeId = GetUnitTypeId(document, unitType);
            return UnitUtils.ConvertFromInternalUnits(value, unitTypeId);
        }

        public static double ConvertToInternalUnits(Document document, double value, UnitTypeProxy unitType)
        {
            var unitTypeId = GetUnitTypeId(document, unitType);
            return UnitUtils.ConvertToInternalUnits(value, unitTypeId);
        }

        public static DisplayUnitTypeProxy GetDisplayUnit(Document document, UnitTypeProxy unitType)
        {
            var unitTypeId = GetUnitTypeId(document, unitType);
            return ForgeConverter.GetDisplayUnit(unitTypeId);
        }

        private static ForgeTypeId GetUnitTypeId(Document document, UnitTypeProxy unitType)
        {
            var unitTypeId = ForgeConverter.GetUnitTypeId(unitType);
            return document.GetUnits().GetFormatOptions(unitTypeId).GetUnitTypeId();
        }
    }
}