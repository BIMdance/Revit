using Autodesk.Revit.DB;

namespace BIMdance.Revit.ObsoleteAPI;

public static class FamilyManagerRevit2016
{
    public static FamilyParameter AddParameter(FamilyManager familyManager,
        string parameterName, int parameterType, int builtInParameterGroup, bool isInstance) =>
        familyManager.AddParameter(parameterName, (BuiltInParameterGroup)builtInParameterGroup, (ParameterType)parameterType, isInstance);
    
    public static FamilyParameter AddParameter(FamilyManager familyManager,
        ExternalDefinition externalDefinition, int builtInParameterGroup, bool isInstance) =>
        familyManager.AddParameter(externalDefinition, (BuiltInParameterGroup)builtInParameterGroup, isInstance);
    
    public static FamilyParameter ReplaceParameter(FamilyManager familyManager,
        FamilyParameter familyParameter, ExternalDefinition externalDefinition,
        int builtInParameterGroup, bool isInstance) =>
        familyManager.ReplaceParameter(familyParameter, externalDefinition, (BuiltInParameterGroup)builtInParameterGroup, isInstance);
    
    public static FamilyParameter ReplaceParameter(FamilyManager familyManager,
        FamilyParameter familyParameter, string parameterName,
        int builtInParameterGroup, bool isInstance) =>
        familyManager.ReplaceParameter(familyParameter, parameterName, (BuiltInParameterGroup)builtInParameterGroup, isInstance);
}