namespace BIMdance.Revit.Async.ExternalEvents;

internal class AsyncDelegateExternalEventHandler<TResponse> :
    AsyncExternalEventHandler<Func<UIApplication, Task<TResponse>>, TResponse>
{
    protected override Task<TResponse> Handle(
        UIApplication app,
        Func<UIApplication, Task<TResponse>> request)
    {
        return request(app);
    }

    public override Task<TResponse> Raise(Func<UIApplication, Task<TResponse>> request = default)
    {
        return default;
    }
}