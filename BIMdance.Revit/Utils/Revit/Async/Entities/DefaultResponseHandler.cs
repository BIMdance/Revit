using BIMdance.Revit.Utils.Revit.Async.Interfaces;

namespace BIMdance.Revit.Utils.Revit.Async.Entities;

internal class DefaultResponseHandler<TResponse> : IExternalEventResponseHandler<TResponse>
{
    public DefaultResponseHandler(TaskCompletionSource<TResponse> taskCompletionSource)
    {
        TaskCompletionSource = taskCompletionSource;
    }
        
    private TaskCompletionSource<TResponse> TaskCompletionSource { get; }

    public void Cancel()
    {
        TaskCompletionSource.TrySetCanceled();
    }

    public void SetResult(TResponse response)
    {
        TaskCompletionSource.TrySetResult(response);
    }

    public void ThrowException(Exception exception)
    {
        TaskCompletionSource.TrySetException(exception);
    }
}