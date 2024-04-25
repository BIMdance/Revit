namespace BIMdance.Revit.Utils.Async.ExternalEvents;

internal class SyncDelegateExternalEventHandler<TResponse> : SyncExternalEventHandler<Func<UIApplication, TResponse>, TResponse>
{
    protected override TResponse Handle(
        UIApplication app,
        Func<UIApplication, TResponse> request)
    {
        return request(app);
    }

    public override Task<TResponse> Raise(Func<UIApplication, TResponse> request = default)
    {
        return default;
    }
}