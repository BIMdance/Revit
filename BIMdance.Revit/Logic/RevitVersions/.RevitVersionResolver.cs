using BIMdance.Revit.ObsoleteAPI;
using Color = Autodesk.Revit.DB.Color;

namespace BIMdance.Revit.Logic.RevitVersions;

public static class RevitVersionResolver
{
    public static int RevitVersionNumber { get; set; }

    public static long GetValue(this ElementId elementId) =>
        RevitVersionNumber < 2024
            ? ElementId2016.GetValue(elementId)
            : Revit2024.ElementId.GetValue(elementId);
    
    public static ElementId NewElementId(int id) =>
        RevitVersionNumber < 2024
            ? ElementId2016.NewElementId(id)
            : Revit2024.ElementId.NewElementId(id);
    
    public static ElementId NewElementId(long id) =>
        RevitVersionNumber < 2024
            ? ElementId2016.NewElementId((int)id)
            : Revit2024.ElementId.NewElementId(id);

    public static class Electrical
    {
        internal static IEnumerable<ElectricalSystem> GetElectricalSystems(
            FamilyInstance familyInstance) =>
            RevitVersionNumber < 2021
                ? MEPModelRevit2016.GetElectricalSystems(familyInstance)
                : Revit2021.GetElectricalSystems(familyInstance);
    }
    
    public static class Filters
    {
        public static ParameterFilterElement CreateParameterFilterElement(
            Document document, string name,
            IList<ElementId> categoryIds,
            IList<FilterRule> rules) =>
            RevitVersionNumber < 2019
                ? ParameterFilterElementRevit2016.Create(document, name, categoryIds, rules)
                : Revit2019.CreateParameterFilterElement(document, name, categoryIds, rules);
    }

    public static class Graphics
    {
        internal static void SetProjectionLinePatternId(
            Category category,
            ElementId linePatternId)
        {
            if (RevitVersionNumber > 2016)
                Revit2017.SetProjectionLinePatternId(category, linePatternId);
        }
        
        public static void SetSurfaceForegroundPatternColor(OverrideGraphicSettings graphicSettings, Color color)
        {
            if (RevitVersionNumber < 2019)
                OverrideGraphicSettingsRevit2016.SetSurfaceForegroundPatternColor(graphicSettings, color);
            else
                Revit2019.SetSurfaceForegroundPatternColor(graphicSettings, color);
        }

        public static void SetSurfaceForegroundPatternId(OverrideGraphicSettings graphicSettings, FillPatternElement solidPattern)
        {
            if (RevitVersionNumber < 2019)
                OverrideGraphicSettingsRevit2016.SetSurfaceForegroundPatternId(graphicSettings, solidPattern.Id);
            else
                Revit2019.SetSurfaceForegroundPatternId(graphicSettings, solidPattern);
        }

        public static void SetSurfaceForegroundPatternVisible(OverrideGraphicSettings graphicSettings, bool fillPatternVisible)
        {
            if (RevitVersionNumber < 2019)
                OverrideGraphicSettingsRevit2016.SetSurfaceForegroundPatternVisible(graphicSettings, fillPatternVisible);
            else
                Revit2019.SetSurfaceForegroundPatternVisible(graphicSettings, fillPatternVisible);
        }
    }
    
    public static class Parameters
    {
        public static FamilyParameter InsertFamilyParameter(FamilyManager familyManager, int index, string parameterName, ParameterTypeProxy parameterType, ParameterGroupProxy builtInParameterGroup, bool isInstance)
        {
            var parameters = familyManager.GetParameters();
            var newParameter = AddFamilyParameter(familyManager, parameterName, parameterType, builtInParameterGroup, isInstance);
            parameters.Insert(index, newParameter);
            familyManager.ReorderParameters(parameters);
            return newParameter;
        }
        
        public static FamilyParameter AddFamilyParameter(FamilyManager familyManager, string parameterName, ParameterTypeProxy parameterType, ParameterGroupProxy builtInParameterGroup, bool isInstance)
        {
            return RevitVersionNumber < 2022
                ? FamilyManagerRevit2016.AddParameter(
                    familyManager,
                    parameterName,
                    (int)parameterType,
                    (int)builtInParameterGroup,
                    isInstance)
                : Revit2022.FamilyManager.AddParameter(
                    familyManager,
                    parameterName,
                    parameterType,
                    builtInParameterGroup,
                    isInstance);
        }
        
        public static FamilyParameter AddFamilyParameter(FamilyManager familyManager, ExternalDefinition externalDefinition, ParameterGroupProxy builtInParameterGroup, bool isInstance)
        {
            return RevitVersionNumber < 2022
                ? FamilyManagerRevit2016.AddParameter(
                    familyManager,
                    externalDefinition,
                    (int)builtInParameterGroup,
                    isInstance)
                : Revit2022.FamilyManager.AddParameter(
                    familyManager,
                    externalDefinition,
                    builtInParameterGroup,
                    isInstance);
        }
        
        public static FamilyParameter ReplaceFamilyParameter(FamilyManager familyManager, FamilyParameter familyParameter, ExternalDefinition externalDefinition, ParameterGroupProxy builtInParameterGroup, bool isInstance)
        {
            return RevitVersionNumber < 2022
                ? FamilyManagerRevit2016.ReplaceParameter(
                    familyManager,
                    familyParameter,
                    externalDefinition,
                    (int)builtInParameterGroup,
                    isInstance)
                : Revit2022.FamilyManager.AddParameter(
                    familyManager,
                    externalDefinition,
                    builtInParameterGroup,
                    isInstance);
        }
        
        public static FamilyParameter ReplaceFamilyParameter(FamilyManager familyManager, FamilyParameter familyParameter, string parameterName, ParameterGroupProxy builtInParameterGroup, bool isInstance)
        {
            return RevitVersionNumber < 2022
                ? FamilyManagerRevit2016.ReplaceParameter(
                    familyManager,
                    familyParameter,
                    parameterName,
                    (int)builtInParameterGroup,
                    isInstance)
                : Revit2024.FamilyManager.ReplaceParameter(
                    familyManager,
                    familyParameter,
                    parameterName,
                    builtInParameterGroup,
                    isInstance);
        }

        public static ParameterGroupProxy GetParameterGroup(Parameter parameter)
        {
            return RevitVersionNumber < 2022
                ? (ParameterGroupProxy)ParametersRevit2016.GetParameterGroupIndex(parameter)
                : Revit2022.Parameters.GetParameterGroup(parameter);
        }

        public static ParameterTypeProxy GetParameterType(Parameter parameter)
        {
            return RevitVersionNumber < 2022
                ? (ParameterTypeProxy)ParametersRevit2016.GetParameterTypeIndex(parameter)
                : Revit2022.Parameters.GetParameterType(parameter);
        }

        public static ParameterGroupProxy GetParameterGroup(FamilyParameter parameter)
        {
            return RevitVersionNumber < 2022
                ? (ParameterGroupProxy)ParametersRevit2016.GetParameterGroupIndex(parameter)
                : Revit2022.Parameters.GetParameterGroup(parameter);
        }

        public static ParameterTypeProxy GetParameterType(FamilyParameter parameter)
        {
            return RevitVersionNumber < 2022
                ? (ParameterTypeProxy)ParametersRevit2016.GetParameterTypeIndex(parameter)
                : Revit2022.Parameters.GetParameterType(parameter);
        }
    }

    public static class Units
    {
        public static double ConvertFromInternalUnits(Document document, double value, UnitTypeProxy unitType) =>
            RevitVersionNumber < 2022
                ? Units2016.ConvertFromInternalUnits(document, value, (int)unitType)
                : Revit2022.Units.ConvertFromInternalUnits(document, value, unitType);
        
        public static double ConvertToInternalUnits(Document document, double value, UnitTypeProxy unitType) =>
            RevitVersionNumber < 2022
                ? Units2016.ConvertToInternalUnits(document, value, (int)unitType)
                : Revit2022.Units.ConvertToInternalUnits(document, value, unitType);

        public static DisplayUnitTypeProxy GetDisplayUnitType(Document document, UnitTypeProxy unitType)
        {
            return RevitVersionNumber < 2022
                ? (DisplayUnitTypeProxy)Units2016.GetDisplayUnitType(document, (int)unitType)
                : Revit2022.Units.GetDisplayUnit(document, unitType);
        }
    }
}