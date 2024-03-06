using BIMdance.Revit.Async.Extensions;
using BIMdance.Revit.Async.Interfaces;

namespace BIMdance.Revit.Async.ExternalEvents;

public abstract class SyncExternalEventHandler<TRequest, TResponse> : ExternalEventHandler<TRequest, TResponse>
{
    protected sealed override void Execute(
        UIApplication app,
        TRequest request,
        IExternalEventResponseHandler<TResponse> responseHandler)
    {
        responseHandler.Wait(() => Handle(app, request));
    }
        
    protected abstract TResponse Handle(UIApplication app, TRequest request);
}