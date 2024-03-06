namespace BIMdance.Revit.DependencyInjection.ServiceLookup;

internal class ConstantCallSite : IServiceCallSite
{
    internal object DefaultValue { get; }

    public ConstantCallSite(object defaultValue)
    {
        DefaultValue = defaultValue;
    }

    public Type ServiceType => DefaultValue.GetType();
    public Type ImplementationType => DefaultValue.GetType();
    public CallSiteKind Kind { get; } = CallSiteKind.Constant;
}