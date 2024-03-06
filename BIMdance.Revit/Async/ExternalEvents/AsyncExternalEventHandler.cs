using BIMdance.Revit.Async.Extensions;
using BIMdance.Revit.Async.Interfaces;

namespace BIMdance.Revit.Async.ExternalEvents;

public abstract class AsyncExternalEventHandler<TRequest, TResponse> : ExternalEventHandler<TRequest, TResponse>
{
    protected sealed override void Execute( 
        UIApplication app,
        TRequest request,
        IExternalEventResponseHandler<TResponse> responseHandler)
    {
        responseHandler.Await(Handle(app, request), responseHandler.SetResult);
    }

    protected abstract Task<TResponse> Handle(UIApplication app, TRequest request);
}