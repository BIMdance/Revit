using Autodesk.Revit.DB;

namespace BIMdance.Revit.ObsoleteAPI;

public static class ParametersRevit2016
{
    public static int GetParameterGroupIndex(Parameter parameter) =>
        (int)parameter.Definition.ParameterGroup;
    
    public static int GetParameterTypeIndex(Parameter parameter) =>
        (int)parameter.Definition.ParameterType;
    
    public static int GetParameterGroupIndex(FamilyParameter parameter) =>
        (int)parameter.Definition.ParameterGroup;
    
    public static int GetParameterTypeIndex(FamilyParameter parameter) =>
        (int)parameter.Definition.ParameterType;
}