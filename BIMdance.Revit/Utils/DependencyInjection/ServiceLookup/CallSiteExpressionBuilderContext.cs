namespace BIMdance.Revit.Utils.DependencyInjection.ServiceLookup;

internal class CallSiteExpressionBuilderContext
{
    public ParameterExpression ScopeParameter { get; set; }
    public bool RequiresResolvedServices { get; set; }
}