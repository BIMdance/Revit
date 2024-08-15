// ReSharper disable InconsistentNaming

using BIMdance.Revit.ObsoleteAPI;
using Binding = Autodesk.Revit.DB.Binding;
using Color = Autodesk.Revit.DB.Color;

namespace BIMdance.Revit.Utils.Revit.RevitVersions;

public static class RevitVersionResolver
{
    public static int RevitVersionNumber { get; set; }

    public static long GetValue(this ElementId elementId) =>
        RevitVersionNumber < 2024
            ? Revit2016.ElementId.GetValue(elementId)
            : Revit2024.ElementId.GetValue(elementId);
    
    public static ElementId NewElementId(int id) =>
        RevitVersionNumber < 2024
            ? Revit2016.ElementId.NewElementId(id)
            : Revit2024.ElementId.NewElementId(id);
    
    public static ElementId NewElementId(long id) =>
        RevitVersionNumber < 2024
            ? Revit2016.ElementId.NewElementId((int)id)
            : Revit2024.ElementId.NewElementId(id);
    
    public static class Electrical
    {
        public static ElectricalSystem CreateElectricalSystem(Document document, Connector connector, ElectricalSystemType electricalSystemType) =>
            RevitVersionNumber < 2019
                ? Revit2016.ElectricalSystems.Create(document, connector, electricalSystemType)
                : Revit2019.ElectricalSystems.Create(connector, electricalSystemType);
           
        public static ElectricalSystem CreateElectricalSystem(Document document, IList<ElementId> elementIds, ElectricalSystemType electricalSystemType) =>
            RevitVersionNumber < 2019
                ? Revit2016.ElectricalSystems.Create(document, elementIds, electricalSystemType)
                : Revit2019.ElectricalSystems.Create(document, elementIds, electricalSystemType);
    }
    
    public static class Filters
    {
        public static ParameterFilterElement CreateParameterFilterElement(
            Document document, string name,
            IList<ElementId> categoryIds,
            IList<FilterRule> rules) =>
            RevitVersionNumber < 2019
                ? Revit2016.Filters.CreateParameterFilterElement(document, name, categoryIds, rules)
                : Revit2019.Filters.CreateParameterFilterElement(document, name, categoryIds, rules);
    }

    public static class Formats
    {
        public static FormatOptions GetFormatOptions(DisplayUnitTypeProxy displayUnitType) =>
            RevitVersionNumber < 2022
                ? Revit2016.Formats.GetFormatOptions((int)displayUnitType)
                : Revit2022.Formats.GetFormatOptions(displayUnitType);
    }
    
    public static class Graphics
    {
        internal static void SetProjectionLinePatternId(
            Category category,
            ElementId linePatternId)
        {
            if (RevitVersionNumber > 2016)
                Revit2017.Graphics.SetProjectionLinePatternId(category, linePatternId);
        }
        
        public static void SetSurfaceForegroundPatternColor(OverrideGraphicSettings graphicSettings, Color color)
        {
            if (RevitVersionNumber < 2019)
                Revit2016.Graphics.SetSurfaceForegroundPatternColor(graphicSettings, color);
            else
                Revit2019.Graphics.SetSurfaceForegroundPatternColor(graphicSettings, color);
        }

        public static void SetSurfaceForegroundPatternId(OverrideGraphicSettings graphicSettings, FillPatternElement solidPattern)
        {
            if (RevitVersionNumber < 2019)
                Revit2016.Graphics.SetSurfaceForegroundPatternId(graphicSettings, solidPattern.Id);
            else
                Revit2019.Graphics.SetSurfaceForegroundPatternId(graphicSettings, solidPattern);
        }

        public static void SetSurfaceForegroundPatternVisible(OverrideGraphicSettings graphicSettings, bool fillPatternVisible)
        {
            if (RevitVersionNumber < 2019)
                Revit2016.Graphics.SetSurfaceForegroundPatternVisible(graphicSettings, fillPatternVisible);
            else
                Revit2019.Graphics.SetSurfaceForegroundPatternVisible(graphicSettings, fillPatternVisible);
        }
    }
    
    public static class InternalDefinitions
    {
        public static bool Equals(InternalDefinition internalDefinition1, InternalDefinition internalDefinition2) =>
            RevitVersionNumber < 2017
                ? Revit2016.InternalDefinitions.Equals(internalDefinition1, internalDefinition2)
                : Revit2017.InternalDefinitions.Equals(internalDefinition1, internalDefinition2);
    }

    public static class Level
    {
        public static IList<ElementId> GetViewPlans(Autodesk.Revit.DB.Level level) =>
            RevitVersionNumber < 2018
                ? Revit2016.Views.GetViewPlans(level)
                : Revit2018.Element.GetDependentElements<ViewPlan>(level);
    }

    public static class MEPModel
    {
        internal static IEnumerable<ElectricalSystem> GetElectricalSystems(
            FamilyInstance familyInstance) =>
            RevitVersionNumber < 2021
                ? Revit2016.MEPModel.GetElectricalSystems(familyInstance)
                : Revit2021.MEPModel.GetElectricalSystems(familyInstance);

        public static IEnumerable<ElectricalSystem> GetAssignedElectricalSystems(FamilyInstance familyInstance) =>
            RevitVersionNumber < 2021
                ? Revit2016.MEPModel.GetAssignedElectricalSystems(familyInstance)
                : Revit2021.MEPModel.GetAssignedElectricalSystems(familyInstance);
        
        public static IEnumerable<ElectricalSystem> GetElectricalSystems(Autodesk.Revit.DB.MEPModel mepModel) =>
            RevitVersionNumber < 2021
                ? Revit2016.MEPModel.GetElectricalSystems(mepModel)
                : Revit2021.MEPModel.GetElectricalSystems(mepModel);

        public static IEnumerable<ElectricalSystem> GetAssignedElectricalSystems(Autodesk.Revit.DB.MEPModel mepModel) =>
            RevitVersionNumber < 2021
                ? Revit2016.MEPModel.GetAssignedElectricalSystems(mepModel)
                : Revit2021.MEPModel.GetAssignedElectricalSystems(mepModel);
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
        
        public static FamilyParameter AddFamilyParameter(FamilyManager familyManager, string parameterName, ParameterTypeProxy parameterType, ParameterGroupProxy builtInParameterGroup, bool isInstance) =>
            RevitVersionNumber < 2022
                ? Revit2016.FamilyManager.AddParameter(
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

        public static FamilyParameter AddFamilyParameter(FamilyManager familyManager, ExternalDefinition externalDefinition, ParameterGroupProxy builtInParameterGroup, bool isInstance) =>
            RevitVersionNumber < 2022
                ? Revit2016.FamilyManager.AddParameter(
                    familyManager,
                    externalDefinition,
                    (int)builtInParameterGroup,
                    isInstance)
                : Revit2022.FamilyManager.AddParameter(
                    familyManager,
                    externalDefinition,
                    builtInParameterGroup,
                    isInstance);

        public static FamilyParameter ReplaceFamilyParameter(FamilyManager familyManager, FamilyParameter familyParameter, ExternalDefinition externalDefinition, ParameterGroupProxy builtInParameterGroup, bool isInstance) =>
            RevitVersionNumber < 2022
                ? Revit2016.FamilyManager.ReplaceParameter(
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

        public static FamilyParameter ReplaceFamilyParameter(FamilyManager familyManager, FamilyParameter familyParameter, string parameterName, ParameterGroupProxy builtInParameterGroup, bool isInstance) =>
            RevitVersionNumber < 2022
                ? Revit2016.FamilyManager.ReplaceParameter(
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

        public static ParameterGroupProxy GetParameterGroup(Parameter parameter) =>
            RevitVersionNumber < 2022
                ? (ParameterGroupProxy)Revit2016.Parameters.GetParameterGroupIndex(parameter)
                : Revit2022.Parameters.GetParameterGroup(parameter);

        public static ParameterTypeProxy GetParameterType(Parameter parameter) =>
            RevitVersionNumber < 2022
                ? (ParameterTypeProxy)Revit2016.Parameters.GetParameterTypeIndex(parameter)
                : Revit2022.Parameters.GetParameterType(parameter);

        public static ParameterGroupProxy GetParameterGroup(FamilyParameter parameter) =>
            RevitVersionNumber < 2022
                ? (ParameterGroupProxy)Revit2016.Parameters.GetParameterGroupIndex(parameter)
                : Revit2022.Parameters.GetParameterGroup(parameter);

        public static ParameterTypeProxy GetParameterType(FamilyParameter parameter) =>
            RevitVersionNumber < 2022
                ? (ParameterTypeProxy)Revit2016.Parameters.GetParameterTypeIndex(parameter)
                : Revit2022.Parameters.GetParameterType(parameter);

        public static class BindingMaps
        {
            public static void Insert(BindingMap bindingMap, ExternalDefinition definition, Binding binding, ParameterGroupProxy parameterGroup)
            {
                if (RevitVersionNumber < 2024)
                    Revit2016.Parameters.BindingMaps.Insert(bindingMap, definition, binding, (int)parameterGroup);
                else
                    Revit2024.Parameters.BindingMaps.Insert(bindingMap, definition, binding, parameterGroup);
            }
        }
    }

    public static class Planes
    {
        public static Plane CreatePlane(XYZ normal, XYZ origin) =>
            RevitVersionNumber < 2017
                ? Revit2016.Geometry.CreatePlaneByNormalAndOrigin(normal, origin)
                : Revit2017.Geometry.CreatePlaneByNormalAndOrigin(normal, origin);
    }

    public static class Schedules
    {
        public static void SetCountTotal(ScheduleField scheduleField)
        {
            if (RevitVersionNumber < 2017)
                Revit2016.Schedules.SetHasTotal(scheduleField);
            else
                Revit2017.Schedules.SetTotalDisplayType(scheduleField);
        }
    }

    public static class Units
    {
        public static double ConvertFromInternalUnits(Document document, double value, UnitTypeProxy unitType) =>
            RevitVersionNumber < 2022
                ? Revit2016.Units.ConvertFromInternalUnits(document, value, (int)unitType)
                : Revit2022.Units.ConvertFromInternalUnits(document, value, unitType);
        
        public static double ConvertToInternalUnits(Document document, double value, UnitTypeProxy unitType) =>
            RevitVersionNumber < 2022
                ? Revit2016.Units.ConvertToInternalUnits(document, value, (int)unitType)
                : Revit2022.Units.ConvertToInternalUnits(document, value, unitType);

        public static DisplayUnitTypeProxy GetDisplayUnitType(Document document, UnitTypeProxy unitType) =>
            RevitVersionNumber < 2022
                ? (DisplayUnitTypeProxy)Revit2016.Units.GetDisplayUnitType(document, (int)unitType)
                : Revit2022.Units.GetDisplayUnit(document, unitType);
    }

    public static class ViewSheet
    {
        public static IEnumerable<ElementId> GetGenericAnnotationElements(Autodesk.Revit.DB.ViewSheet viewSheet) =>
            RevitVersionNumber < 2018
                ? Revit2016.Views.GetGenericAnnotationElements(viewSheet)
                : Revit2018.Element.GetDependentElements(viewSheet, BuiltInCategory.OST_GenericAnnotation);
    }
}