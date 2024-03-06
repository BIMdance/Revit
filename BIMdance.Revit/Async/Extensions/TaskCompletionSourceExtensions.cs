namespace BIMdance.Revit.Async.Extensions;

internal static class TaskCompletionSourceExtensions
{
    public static TaskCompletionSource<TResponse> Await<TSource, TResponse>(
        this TaskCompletionSource<TResponse> tcs,
        Task<TSource> source,
        Action<TSource, TaskCompletionSource<TResponse>> onComplete,
        Action final = null)
    {
        source.ContinueWith(task =>
        {
            try
            {
                if (task.IsCompleted)
                {
                    onComplete(task.Result, tcs);
                }
                else if (task.IsFaulted)
                {
                    tcs.TrySetException(task.Exception ?? new Exception("Unknown Exception"));
                }
                else if (task.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }
            }
            finally
            {
                final?.Invoke();
            }
        }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        return tcs;
    }

    public static async void Await<TResponse>(this TaskCompletionSource<TResponse> tcs, Task<TResponse> task, Action final = null)
    {
        try
        {
            var response = await task;
            if (task.IsCompleted)
            {
                tcs.TrySetResult(response);
            }
        
            if (task.IsCanceled)
            {
                tcs.TrySetCanceled();
            }
        
            if (task.IsFaulted)
            {
                tcs.TrySetException(task.Exception ?? new Exception("Unknown Exception"));
            }
        }
        catch (Exception e)
        {
            tcs.TrySetException(e);
        }
        finally
        {
            final?.Invoke();
        }
    }
}