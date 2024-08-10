using BIMdance.Revit.Utils.Revit.Async.Interfaces;
using BIMdance.Revit.Utils.Revit.Async.Extensions;

namespace BIMdance.Revit.Utils.Revit.Async.ExternalEvents;

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