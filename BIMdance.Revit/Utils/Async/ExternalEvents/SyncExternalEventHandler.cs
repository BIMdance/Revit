using BIMdance.Revit.Utils.Async.Extensions;
using BIMdance.Revit.Utils.Async.Interfaces;

namespace BIMdance.Revit.Utils.Async.ExternalEvents;

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