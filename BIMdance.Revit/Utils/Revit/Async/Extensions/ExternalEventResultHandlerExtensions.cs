using BIMdance.Revit.Utils.Revit.Async.Interfaces;

namespace BIMdance.Revit.Utils.Revit.Async.Extensions;

public static class ExternalEventResultHandlerExtensions
{
    public static IExternalEventResponseHandler<TResponse> Await<TSource, TResponse>(
        this IExternalEventResponseHandler<TResponse> responseHandler,
        Task<TSource> source,
        Action<TSource, IExternalEventResponseHandler<TResponse>> onComplete)
    {
        source.ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                onComplete(task.Result, responseHandler);
            }
            else if (task.IsFaulted)
            {
                responseHandler.ThrowException(task.Exception ?? new Exception("Unknown Exception"));
            }
            else if (task.IsCanceled)
            {
                responseHandler.Cancel();
            }
        });
        return responseHandler;
    }

    public static IExternalEventResponseHandler<TResponse> Await<TSource, TResponse>(
        this IExternalEventResponseHandler<TResponse> responseHandler,
        Task<TSource> source,
        Action<TSource> onComplete)
    {
        source.ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                onComplete(task.Result);
            }
            else if (task.IsFaulted)
            {
                responseHandler.ThrowException(task.Exception ?? new Exception("Unknown Exception"));
            }
            else if (task.IsCanceled)
            {
                responseHandler.Cancel();
            }
        });
        return responseHandler;
    }
        
    public static async void Await<TResponse>(
        this IExternalEventResponseHandler<TResponse> responseHandler,
        Task<TResponse> task)
    {
        try
        {
            var response = await task;
            if (task.IsCompleted)
            {
                responseHandler.SetResult(response);
            }
        
            if (task.IsCanceled)
            {
                responseHandler.Cancel();
            }
        
            if (task.IsFaulted)
            {
                responseHandler.ThrowException(task.Exception ?? new Exception("Unknown Exception"));
            }
        }
        catch (Exception e)
        {
            responseHandler.ThrowException(e);
        }
    }
        
    public static void Wait<TResponse>(
        this IExternalEventResponseHandler<TResponse> responseHandler,
        Func<TResponse> function)
    {
        try
        {
            var response = function();
            responseHandler.SetResult(response);
        }
        catch (Exception e)
        {
            responseHandler.ThrowException(e);
        }
    }
}