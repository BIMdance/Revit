using BIMdance.Revit.Async.Entities;
using BIMdance.Revit.Async.Interfaces;

namespace BIMdance.Revit.Async.ExternalEvents;

public abstract class ExternalEventHandler<TRequest, TResponse> : IExternalEventHandler, ICloneable
{
    protected ExternalEventHandler() => Id = Guid.NewGuid();
    private Guid Id { get; }
    private TRequest Request { get; set; }
    private IExternalEventResponseHandler<TResponse> ResponseHandler { get; set; }

    public void Execute(UIApplication app) => Execute(app, Request, ResponseHandler);
    protected abstract void Execute(UIApplication app, TRequest request, IExternalEventResponseHandler<TResponse> responseHandler);
    public virtual async Task<TResponse> Raise(TRequest request = default)
    {
        return await RaiseGlobal(request);
    }
    private async Task<TResponse> RaiseGlobal(TRequest request = default)
    {
        return await RevitTask.RaiseGlobal<TRequest, TResponse>(GetType(), request);
    }
        
    public Task<TResponse> Prepare(TRequest request)
    {
        Request = request;
        var tcs = new TaskCompletionSource<TResponse>();
        ResponseHandler = new DefaultResponseHandler<TResponse>(tcs);
        return tcs.Task;
    }

    public string GetName() => $"{GetType().Name}_{Id}";
        
    public virtual object Clone()
    {
        return GetType().GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>()) ?? throw new InvalidOperationException();
    }
}