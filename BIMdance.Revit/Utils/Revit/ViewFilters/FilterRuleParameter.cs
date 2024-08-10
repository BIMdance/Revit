namespace BIMdance.Revit.Utils.Revit.ViewFilters;

internal class FilterRuleParameter
{
    internal FilterRuleParameter(Guid sharedParameterGuid, object value, FilterRuleMode filterRuleMode)
    {
        SharedParameterGuid = sharedParameterGuid;
        Value = value;
        FilterRuleMode = filterRuleMode;
    }

    internal Guid SharedParameterGuid { get; }
    internal ElementId ParameterId { get; set; }
    internal object Value { get; }
    internal FilterRuleMode FilterRuleMode { get; }
    
    internal FilterRule CreateFilterRule()
    {
        return FilterRuleMode switch
        {
            FilterRuleMode.Contains => ParameterFilterRuleFactory.CreateContainsRule(ParameterId, Value.ToString(), caseSensitive: false),
            FilterRuleMode.Equals => FilterUtils.CreateEqualsFilterRule(ParameterId, Value),
            FilterRuleMode.Greater => FilterUtils.CreateGreaterFilterRule(ParameterId, Value),
            FilterRuleMode.GreaterOrEqual => FilterUtils.CreateGreaterOrEqualFilterRule(ParameterId, Value),
            FilterRuleMode.Less => FilterUtils.CreateLessFilterRule(ParameterId, Value),
            FilterRuleMode.LessOrEqual => FilterUtils.CreateLessOrEqualFilterRule(ParameterId, Value),
            FilterRuleMode.NotContains => ParameterFilterRuleFactory.CreateNotContainsRule(ParameterId, Value.ToString(), caseSensitive: false),
            FilterRuleMode.NotEquals => FilterUtils.CreateNotEqualsFilterRule(ParameterId, Value),
            _ => null
        };
    }

    public override string ToString()
    {
        return $"['{SharedParameterGuid}'] [{ParameterId}] {FilterRuleMode}: {Value}";
    }
}