namespace BIMdance.Revit.Utils.DependencyInjection.ServiceLookup;

internal abstract class CallSiteVisitor<TArgument, TResult>
{
    protected virtual TResult VisitCallSite(IServiceCallSite callSite, TArgument argument)
    {
        return callSite.Kind switch
        {
            CallSiteKind.Factory => VisitFactory((FactoryCallSite)callSite, argument),
            CallSiteKind.IEnumerable => VisitIEnumerable((IEnumerableCallSite)callSite, argument),
            CallSiteKind.Constructor => VisitConstructor((ConstructorCallSite)callSite, argument),
            CallSiteKind.Transient => VisitTransient((TransientCallSite)callSite, argument),
            CallSiteKind.Singleton => VisitSingleton((SingletonCallSite)callSite, argument),
            CallSiteKind.Scope => VisitScoped((ScopedCallSite)callSite, argument),
            CallSiteKind.Constant => VisitConstant((ConstantCallSite)callSite, argument),
            CallSiteKind.CreateInstance => VisitCreateInstance((CreateInstanceCallSite)callSite, argument),
            CallSiteKind.ServiceProvider => VisitServiceProvider((ServiceProviderCallSite)callSite, argument),
            CallSiteKind.ServiceScopeFactory => VisitServiceScopeFactory((ServiceScopeFactoryCallSite)callSite,
                argument),
            _ => throw new NotSupportedException($"Call site type {callSite.GetType()} is not supported")
        };
    }

    protected abstract TResult VisitTransient(TransientCallSite transientCallSite, TArgument argument);

    protected abstract TResult VisitConstructor(ConstructorCallSite constructorCallSite, TArgument argument);

    protected abstract TResult VisitSingleton(SingletonCallSite singletonCallSite, TArgument argument);

    protected abstract TResult VisitScoped(ScopedCallSite scopedCallSite, TArgument argument);

    protected abstract TResult VisitConstant(ConstantCallSite constantCallSite, TArgument argument);

    protected abstract TResult VisitCreateInstance(CreateInstanceCallSite createInstanceCallSite, TArgument argument);

    protected abstract TResult VisitServiceProvider(ServiceProviderCallSite serviceProviderCallSite, TArgument argument);

    protected abstract TResult VisitServiceScopeFactory(ServiceScopeFactoryCallSite serviceScopeFactoryCallSite, TArgument argument);

    protected abstract TResult VisitIEnumerable(IEnumerableCallSite enumerableCallSite, TArgument argument);

    protected abstract TResult VisitFactory(FactoryCallSite factoryCallSite, TArgument argument);
}