namespace BIMdance.Revit.Utils.DependencyInjection.ServiceLookup;

internal class CallSiteValidator: CallSiteVisitor<CallSiteValidator.CallSiteValidatorState, Type>
{
    // Keys are services being resolved via GetService, values - first scoped service in their call site tree
    private readonly ConcurrentDictionary<Type, Type> _scopedServices = new();

    public void ValidateCallSite(IServiceCallSite callSite)
    {
        var scoped = VisitCallSite(callSite, default);
        if (scoped != null)
        {
            _scopedServices[callSite.ServiceType] = scoped;
        }
    }

    public void ValidateResolution(Type serviceType, IServiceScope scope, IServiceScope rootScope)
    {
        if (ReferenceEquals(scope, rootScope)
            && _scopedServices.TryGetValue(serviceType, out var scopedService))
        {
            if (serviceType == scopedService)
            {
                throw new InvalidOperationException(
                    Resources.FormatDirectScopedResolvedFromRootException(serviceType,
                        nameof(ServiceLifetime.Scoped).ToLowerInvariant()));
            }

            throw new InvalidOperationException(
                Resources.FormatScopedResolvedFromRootException(
                    serviceType,
                    scopedService,
                    nameof(ServiceLifetime.Scoped).ToLowerInvariant()));
        }
    }

    protected override Type VisitTransient(TransientCallSite transientCallSite, CallSiteValidatorState state)
    {
        return VisitCallSite(transientCallSite.ServiceCallSite, state);
    }

    protected override Type VisitConstructor(ConstructorCallSite constructorCallSite, CallSiteValidatorState state)
    {
        Type result = null;
        foreach (var parameterCallSite in constructorCallSite.ParameterCallSites)
        {
            var scoped =  VisitCallSite(parameterCallSite, state);
            result ??= scoped;
        }
        return result;
    }

    protected override Type VisitIEnumerable(IEnumerableCallSite enumerableCallSite,
        CallSiteValidatorState state)
    {
        Type result = null;
        foreach (var serviceCallSite in enumerableCallSite.ServiceCallSites)
        {
            var scoped = VisitCallSite(serviceCallSite, state);
            result ??= scoped;
        }
        return result;
    }

    protected override Type VisitSingleton(SingletonCallSite singletonCallSite, CallSiteValidatorState state)
    {
        state.Singleton = singletonCallSite;
        return VisitCallSite(singletonCallSite.ServiceCallSite, state);
    }

    protected override Type VisitScoped(ScopedCallSite scopedCallSite, CallSiteValidatorState state)
    {
        // We are fine with having ServiceScopeService requested by singletons
        if (scopedCallSite.ServiceCallSite is ServiceScopeFactoryCallSite)
        {
            return null;
        }
        if (state.Singleton != null)
        {
            throw new InvalidOperationException(Resources.FormatScopedInSingletonException(
                scopedCallSite.ServiceType,
                state.Singleton.ServiceType,
                nameof(ServiceLifetime.Scoped).ToLowerInvariant(),
                nameof(ServiceLifetime.Singleton).ToLowerInvariant()
            ));
        }

        VisitCallSite(scopedCallSite.ServiceCallSite, state);
        return scopedCallSite.ServiceType;
    }

    protected override Type VisitConstant(ConstantCallSite constantCallSite, CallSiteValidatorState state) => null;

    protected override Type VisitCreateInstance(CreateInstanceCallSite createInstanceCallSite, CallSiteValidatorState state) => null;

    protected override Type VisitServiceProvider(ServiceProviderCallSite serviceProviderCallSite, CallSiteValidatorState state) => null;

    protected override Type VisitServiceScopeFactory(ServiceScopeFactoryCallSite serviceScopeFactoryCallSite, CallSiteValidatorState state) => null;

    protected override Type VisitFactory(FactoryCallSite factoryCallSite, CallSiteValidatorState state) => null;

    internal struct CallSiteValidatorState
    {
        public SingletonCallSite Singleton { get; set; }
    }
}