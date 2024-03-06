namespace BIMdance.Revit.Async.ExternalEvents;

internal class ExternalEventHandlerCreator : SyncExternalEventHandler<IExternalEventHandler, ExternalEvent>
{
    protected override ExternalEvent Handle(UIApplication app, IExternalEventHandler request)
    {
        return ExternalEvent.Create(request);
    }

    public override Task<ExternalEvent> Raise(IExternalEventHandler request = default)
    {
        return default;
    }
}