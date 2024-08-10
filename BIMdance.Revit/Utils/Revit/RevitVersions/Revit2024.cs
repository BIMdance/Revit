using Binding = Autodesk.Revit.DB.Binding;

namespace BIMdance.Revit.Utils.Revit.RevitVersions;

internal static class Revit2024
{
    public static class ElementId
    {
        public static Autodesk.Revit.DB.ElementId NewElementId(long id) => new(id);
        public static long GetValue(Autodesk.Revit.DB.ElementId elementId) => elementId.Value;
    }
    
    public static class FamilyManager
    {
        public static FamilyParameter ReplaceParameter(Autodesk.Revit.DB.FamilyManager familyManager,
            FamilyParameter familyParameter, ExternalDefinition externalDefinition,
            ParameterGroupProxy parameterGroup, bool isInstance)
        {
            var groupId = ForgeConverter.GetGroupId(parameterGroup);
            return familyManager.ReplaceParameter(familyParameter, externalDefinition, groupId, isInstance);
        }
        
        public static FamilyParameter ReplaceParameter(Autodesk.Revit.DB.FamilyManager familyManager,
            FamilyParameter familyParameter, string parameterName,
            ParameterGroupProxy parameterGroup, bool isInstance)
        {
            var groupId = ForgeConverter.GetGroupId(parameterGroup);
            return familyManager.ReplaceParameter(familyParameter, parameterName, groupId, isInstance);
        }
    }
    
    public static class Parameters
    {
        public static class BindingMaps
        {
            public static void Insert(BindingMap bindingMap, Definition definition, Binding binding, ParameterGroupProxy parameterGroup)
            {
                var groupId = ForgeConverter.GetGroupId(parameterGroup);
                bindingMap.Insert(definition, binding, groupId);
            }
        }
    }
}