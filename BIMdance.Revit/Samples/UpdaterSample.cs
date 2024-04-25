namespace BIMdance.Revit.Samples;

public class UpdaterSample : Updater<CableTray>
{
    private readonly ElementCategoryFilter _cableTrayFilter = new(BuiltInCategory.OST_CableTray);
    protected override Guid Guid { get; } = new(/* TODO must be unique */ "357db14e-1245-4e97-9444-e261a86a7709");

    public UpdaterSample(AddInId addInId, UpdateStatus<CableTray> updateStatus) :
        base(addInId, updateStatus) { }
    
    public override void AddTriggers()
    {
        UpdaterRegistry.AddTrigger(Id, _cableTrayFilter, Element.GetChangeTypeElementAddition());
    }

    protected override void OnUpdated(UpdaterData data)
    {
        var elementIds = data.GetAddedElementIds();
        // TODO
    }
}