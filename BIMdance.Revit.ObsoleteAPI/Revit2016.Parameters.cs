namespace BIMdance.Revit.ObsoleteAPI;

public static partial class Revit2016
{
    public static class Parameters
    {
        public static int GetParameterGroupIndex(Parameter parameter) =>
            (int)parameter.Definition.ParameterGroup;
        
        public static int GetParameterTypeIndex(Parameter parameter) =>
            (int)parameter.Definition.ParameterType;
        
        public static int GetParameterGroupIndex(FamilyParameter parameter) =>
            (int)parameter.Definition.ParameterGroup;
        
        public static int GetParameterTypeIndex(FamilyParameter parameter) =>
            (int)parameter.Definition.ParameterType;
        
        public static class BindingMaps
        {
            public static void Insert(BindingMap bindingMap, Definition definition, Binding binding, int parameterGroup) =>
                bindingMap.Insert(definition, binding, (BuiltInParameterGroup)parameterGroup);
        }
    }
}