using Color = Autodesk.Revit.DB.Color;

namespace BIMdance.Revit.Utils.Revit;

internal static class FilterUtils
{
    internal static void SetProjectionFill(this OverrideGraphicSettings graphicSettings, FillPatternElement solidPattern, Color color)
    {
        if (solidPattern != null)
            RevitVersionResolver.Graphics.SetSurfaceForegroundPatternId(graphicSettings, solidPattern);
        else
            graphicSettings.SetProjectionLineColor(color);

        RevitVersionResolver.Graphics.SetSurfaceForegroundPatternColor(graphicSettings, color);
    }
    
    internal static FilterRule CreateEqualsFilterRule(ElementId parameterId, object value)
    {
        return value switch
        {
            double doubleValue => ParameterFilterRuleFactory.CreateEqualsRule(parameterId, doubleValue, 0.001),
            ElementId id => ParameterFilterRuleFactory.CreateEqualsRule(parameterId, id),
            int intValue => ParameterFilterRuleFactory.CreateEqualsRule(parameterId, intValue),
            _ => ParameterFilterRuleFactory.CreateEqualsRule(parameterId, value.ToString())
        };
    }

    internal static FilterRule CreateLessFilterRule(ElementId idParameter, object value)
    {
        return value switch
        {
            double doubleValue => ParameterFilterRuleFactory.CreateLessRule(idParameter, doubleValue, 0.001),
            ElementId id => ParameterFilterRuleFactory.CreateLessRule(idParameter, id),
            int intValue => ParameterFilterRuleFactory.CreateLessRule(idParameter, intValue),
            _ => ParameterFilterRuleFactory.CreateLessRule(idParameter, value.ToString())
        };
    }

    internal static FilterRule CreateLessOrEqualFilterRule(ElementId idParameter, object value)
    {
        return value switch
        {
            double doubleValue => ParameterFilterRuleFactory.CreateLessOrEqualRule(idParameter, doubleValue, 0.001),
            ElementId id => ParameterFilterRuleFactory.CreateLessOrEqualRule(idParameter, id),
            int intValue => ParameterFilterRuleFactory.CreateLessOrEqualRule(idParameter, intValue),
            _ => ParameterFilterRuleFactory.CreateLessOrEqualRule(idParameter, value.ToString())
        };
    }

    internal static FilterRule CreateGreaterFilterRule(ElementId idParameter, object value)
    {
        return value switch
        {
            double doubleValue => ParameterFilterRuleFactory.CreateGreaterRule(idParameter, doubleValue, 0.001),
            ElementId id => ParameterFilterRuleFactory.CreateGreaterRule(idParameter, id),
            int intValue => ParameterFilterRuleFactory.CreateGreaterRule(idParameter, intValue),
            _ => ParameterFilterRuleFactory.CreateGreaterRule(idParameter, value.ToString())
        };
    }

    internal static FilterRule CreateGreaterOrEqualFilterRule(ElementId idParameter, object value)
    {
        return value switch
        {
            double doubleValue => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(idParameter, doubleValue, 0.001),
            ElementId id => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(idParameter, id),
            int intValue => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(idParameter, intValue),
            _ => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(idParameter, value.ToString())
        };
    }

    internal static FilterRule CreateNotEqualsFilterRule(ElementId idParameter, object value)
    {
        return value switch
        {
            double doubleValue => ParameterFilterRuleFactory.CreateNotEqualsRule(idParameter, doubleValue, 0.001),
            ElementId id => ParameterFilterRuleFactory.CreateNotEqualsRule(idParameter, id),
            int intValue => ParameterFilterRuleFactory.CreateNotEqualsRule(idParameter, intValue),
            _ => ParameterFilterRuleFactory.CreateNotEqualsRule(idParameter, value.ToString())
        };
    }
}