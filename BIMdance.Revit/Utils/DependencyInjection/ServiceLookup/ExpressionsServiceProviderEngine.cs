﻿namespace BIMdance.Revit.Utils.DependencyInjection.ServiceLookup;

internal class ExpressionsServiceProviderEngine : ServiceProviderEngine
{
    private readonly ExpressionResolverBuilder _expressionResolverBuilder;
    public ExpressionsServiceProviderEngine(IEnumerable<ServiceDescriptor> serviceDescriptors, IServiceProviderEngineCallback callback) : base(serviceDescriptors, callback)
    {
        _expressionResolverBuilder = new ExpressionResolverBuilder(RuntimeResolver, this, Root);
    }

    protected override Func<ServiceProviderEngineScope, object> RealizeService(IServiceCallSite callSite)
    {
        var realizedService = _expressionResolverBuilder.Build(callSite);
        RealizedServices[callSite.ServiceType] = realizedService;
        return realizedService;
    }
}