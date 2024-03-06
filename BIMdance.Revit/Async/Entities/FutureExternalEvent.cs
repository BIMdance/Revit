using BIMdance.Revit.Async.Extensions;
using BIMdance.Revit.Async.ExternalEvents;
using BIMdance.Revit.Async.Utils;

namespace BIMdance.Revit.Async.Entities;

internal class FutureExternalEvent : ICloneable, IDisposable
{
    private FutureExternalEvent(IExternalEventHandler handler, ExternalEvent externalEvent)
    {
        Handler = handler;
        CreatedExternalEvent = externalEvent;
        ExternalEventTaskCreator = () => TaskUtils.FromResult(externalEvent);
    }

    public FutureExternalEvent(IExternalEventHandler handler)
    {
        Handler = handler;
        ExternalEventTaskCreator = () => CreateExternalEvent(handler)?.ContinueWith(task =>
        {
            CreatedExternalEvent = task.Result;
            return CreatedExternalEvent;
        });
    }

    private static FutureExternalEvent FutureExternalEventCreator { get; set; }
    private static bool HasInitialized => FutureExternalEventCreator != null;
    private static AsyncLocker Locker { get; } = new();
    private static ConcurrentQueue<UnlockKey> UnlockKeys { get; } = new();
    public IExternalEventHandler Handler { get; }
    private ExternalEvent CreatedExternalEvent { get; set; }
    private Func<Task<ExternalEvent>> ExternalEventTaskCreator { get; }

    public object Clone()
    {
        var handler = Handler is ICloneable cloneable ? cloneable.Clone() as IExternalEventHandler : Handler;
        return new FutureExternalEvent(handler);
    }

    public void Dispose()
    {
        CreatedExternalEvent?.Dispose();
    }

    internal static void Initialize(UIControlledApplication application)
    {
        if (HasInitialized)
            return;
            
        application.Idling += Application_Idling;
        var handler = new ExternalEventHandlerCreator();
        FutureExternalEventCreator = new FutureExternalEvent(handler, ExternalEvent.Create(handler));
    }

    internal static void Initialize(UIApplication application)
    {
        if (HasInitialized)
            return;
            
        application.Idling += Application_Idling;
        var handler = new ExternalEventHandlerCreator();
        FutureExternalEventCreator = new FutureExternalEvent(handler, ExternalEvent.Create(handler));
    }

    internal static void Finalize(UIControlledApplication application)
    {
        application.Idling -= Application_Idling;
        FutureExternalEventCreator = null;
    }

    internal static void Finalize(UIApplication application)
    {
        application.Idling -= Application_Idling;
        FutureExternalEventCreator = null;
    }
        
    private static void Application_Idling(object sender, IdlingEventArgs e)
    {
        // e.SetRaiseWithoutDelay();
            
        while (UnlockKeys.TryDequeue(out var unlockKey))
        {
            unlockKey.Dispose();
        }
    }

    private static Task<ExternalEvent> CreateExternalEvent(IExternalEventHandler handler)
    {
        if (FutureExternalEventCreator is null)
        {
            return null;
        }
        
        return new TaskCompletionSource<ExternalEvent>().Await(Locker.LockAsync(), (unlockKey, tcs) =>
        {
            var creationTask = FutureExternalEventCreator.RunAsync<IExternalEventHandler, ExternalEvent>(handler);
            tcs.Await(creationTask, () => UnlockKeys.Enqueue(unlockKey));
        }).Task;
    }
        
    internal async Task<TResponse> RunAsync<TRequest, TResponse>(TRequest request)
    {
        try
        {
            var genericHandler = (ExternalEventHandler<TRequest, TResponse>) Handler;
            var task = genericHandler.Prepare(request);
            var externalEvent = await GetExternalEvent();

            if (Raise(externalEvent))
            {
                return await task;
            }

            Logger.Warn($"ExternalEvent not accepted.\n{nameof(IExternalEventHandler)}: {Handler.GetName()}");
            return default;
        }
        catch (Exception exception)
        {
#if DEBUG
            throw;
#else
            Logger.Error(exception);
            return default;
#endif
        }
    }

    private Task<ExternalEvent> GetExternalEvent()
    {
        return CreatedExternalEvent != null ? TaskUtils.FromResult(CreatedExternalEvent) : ExternalEventTaskCreator();
    }

    private static bool Raise(ExternalEvent externalEvent)
    {
        var request = externalEvent.Raise();
        return request == ExternalEventRequest.Accepted;
    }
}