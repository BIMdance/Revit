namespace BIMdance.Revit.Utils.Revit.Updaters;

public abstract class Updater : IUpdater
{
    private readonly AddInId _addInId;
    private readonly string _additionalInformation;
    private UpdaterId _updaterId;

    protected Updater(AddInId addInId, string additionalInformation = null)
    {
        _addInId = addInId;
        _additionalInformation = additionalInformation;
    }

    protected abstract Guid Guid { get; }
    public abstract void AddTriggers();
    public abstract void Execute(UpdaterData data);
    public virtual ChangePriority GetChangePriority() => ChangePriority.MEPAccessoriesFittingsSegmentsWires;
    public UpdaterId Id => GetUpdaterId();
    public UpdaterId GetUpdaterId() => _updaterId ??= new UpdaterId(_addInId, Guid);
    public string GetUpdaterName() => GetType().FullName;
    public string GetAdditionalInformation() => _additionalInformation ?? string.Empty;
}

public abstract class Updater<TElement> : Updater
    where TElement : Element
{
    private readonly UpdateStatus<TElement> _updateStatus;

    protected Updater(AddInId addInId, UpdateStatus<TElement> updateStatus, string additionalInformation = null) :
        base(addInId, additionalInformation)
    {
        _updateStatus = updateStatus;
    }

    public override void Execute(UpdaterData data)
    {
        if (_updateStatus.IsUpdating)
            return;

        try
        {
            _updateStatus.IsUpdating = true;
            OnUpdated(data);
        }
        catch (Exception exception)
        {
            Logger.Error($"[{Guid}] {GetUpdaterName()}");
            Logger.Error(exception);
        }
        finally
        {
            _updateStatus.IsUpdating = false;
        }
    }
    
    protected abstract void OnUpdated(UpdaterData data);
}