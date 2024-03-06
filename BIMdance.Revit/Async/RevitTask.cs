using BIMdance.Revit.Async.Entities;
using BIMdance.Revit.Async.ExternalEvents;
using BIMdance.Revit.Async.Utils;

namespace BIMdance.Revit.Async;

public class RevitTask : IDisposable
{
    private static ConcurrentDictionary<Type, FutureExternalEvent> _registeredExternalEvents;
    private ConcurrentDictionary<Type, FutureExternalEvent> _scopedRegisteredExternalEvents;

    private static ConcurrentDictionary<Type, FutureExternalEvent> RegisteredExternalEvents =>
        _registeredExternalEvents ??= new ConcurrentDictionary<Type, FutureExternalEvent>();

    private ConcurrentDictionary<Type, FutureExternalEvent> ScopedRegisteredExternalEvents =>
        _scopedRegisteredExternalEvents ??= new ConcurrentDictionary<Type, FutureExternalEvent>();
        
    public static void Initialize(UIControlledApplication application)
    {
        FutureExternalEvent.Initialize(application);
    }

    public static void Initialize(UIApplication application)
    {
        FutureExternalEvent.Initialize(application);
    }
        
    public static void Finalize(UIControlledApplication application)
    {
        FutureExternalEvent.Finalize(application);
    }

    public static void Finalize(UIApplication application)
    {
        FutureExternalEvent.Finalize(application);
    }

    public Task<TResponse> Raise<THandler, TRequest, TResponse>(TRequest request)
        where THandler : ExternalEventHandler<TRequest, TResponse>
    {
        return ScopedRegisteredExternalEvents.TryGetValue(typeof(THandler), out var futureExternalEvent)
            ? futureExternalEvent.RunAsync<TRequest, TResponse>(request)
            : TaskUtils.FromResult(default(TResponse));
    }

    public Task<TResponse> RaiseNew<THandler, TRequest, TResponse>(TRequest request)
        where THandler : ExternalEventHandler<TRequest, TResponse>
    {
        return ScopedRegisteredExternalEvents.TryGetValue(typeof(THandler), out var futureExternalEvent)
            ? (futureExternalEvent.Clone() as FutureExternalEvent)?.RunAsync<TRequest, TResponse>(request)
            : TaskUtils.FromResult(default(TResponse));
    }

    public void Register<TRequest, TResponse>(ExternalEventHandler<TRequest, TResponse> handler)
    {
        ScopedRegisteredExternalEvents.TryAdd(handler.GetType(), new FutureExternalEvent(handler));
    }

    public static Task<TResponse> RaiseGlobal<THandler, TRequest, TResponse>(TRequest request)
        where THandler : ExternalEventHandler<TRequest, TResponse>
    {
        return RegisteredExternalEvents.TryGetValue(typeof(THandler), out var futureExternalEvent)
            ? futureExternalEvent.RunAsync<TRequest, TResponse>(request)
            : TaskUtils.FromResult(default(TResponse));
    }

    internal static Task<TResponse> RaiseGlobal<TRequest, TResponse>(Type handlerType, TRequest request)
    {
        return RegisteredExternalEvents.TryGetValue(handlerType, out var futureExternalEvent)
            ? futureExternalEvent.RunAsync<TRequest, TResponse>(request)
            : TaskUtils.FromResult(default(TResponse));
    }

    public static Task<TResponse> RaiseGlobalNew<THandler, TRequest, TResponse>(TRequest request)
        where THandler : ExternalEventHandler<TRequest, TResponse>
    {
        return RegisteredExternalEvents.TryGetValue(typeof(THandler), out var futureExternalEvent)
            ? (futureExternalEvent.Clone() as FutureExternalEvent)?.RunAsync<TRequest, TResponse>(request)
            : TaskUtils.FromResult(default(TResponse));
    }

    public static void RegisterGlobal<TRequest, TResponse>(
        ExternalEventHandler<TRequest, TResponse> handler)
    {
        RegisteredExternalEvents.TryAdd(handler.GetType(), new FutureExternalEvent(handler));
    }

    public static Task<TResponse> RunAsync<TResponse>(Func<TResponse> function)
    {
        return RunAsync(_ => function());
    }

    public static Task<TResponse> RunAsync<TResponse>(Func<UIApplication, TResponse> function)
    {
        var handler = new SyncDelegateExternalEventHandler<TResponse>();
        var futureExternalEvent = new FutureExternalEvent(handler);
        return futureExternalEvent.RunAsync<Func<UIApplication, TResponse>, TResponse>(function);
    }

    public static Task<TResponse> RunAsync<TResponse>(Func<Task<TResponse>> function)
    {
        return RunAsync(_ => function());
    }

    public static Task<TResponse> RunAsync<TResponse>(Func<UIApplication, Task<TResponse>> function)
    {
        var handler = new AsyncDelegateExternalEventHandler<TResponse>();
        var futureExternalEvent = new FutureExternalEvent(handler);
        return futureExternalEvent.RunAsync<Func<UIApplication, Task<TResponse>>, TResponse>(function);
    }

    public static Task RunAsync(Action action)
    {
        return RunAsync(_ => action());
    }

    public static Task RunAsync(Action<UIApplication> action) => RunAsync(app =>
    {
        action(app);
        return Task.CompletedTask; // (object)null;
    });

    public static Task RunAsync(Func<Task> function)
    {
        return RunAsync(_ => function());
    }
        
    public static Task RunAsync(Func<UIApplication, Task> function)
    {
        return RunAsync(app => function(app).ContinueWith(_ => (object)null));
    }
        
    public void Dispose()
    {
        _scopedRegisteredExternalEvents = null;
    }
}