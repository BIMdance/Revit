namespace BIMdance.Revit.Logic.DataAccess;

public class ChangedElements : IPropertyPrototype<ChangedElements>
{
    private HashSet<int> _addedElements = new();
    private HashSet<int> _deletedElements = new();
    private HashSet<int> _modifiedElements = new();
    
    public void Create(int id) { if (id < 0) _addedElements.Add(id); }
    public void Delete(int id)
    {
        if (id > 0) _deletedElements.Add(id);
        else { _addedElements.Remove(id); _modifiedElements.Remove(id); }
    }
    public void Modify(int id) { _modifiedElements.Add(id); }
    public void UpdateStorage() => IsStorageUpdated = true;
    
    public bool IsAnyChanged => IsAnyAdded || IsAnyDeleted || IsAnyModified || IsStorageUpdated;
    public bool IsAnyAdded => _addedElements.Any();
    public bool IsAnyDeleted => _deletedElements.Any();
    public bool IsAnyModified => _modifiedElements.Any();
    public bool IsStorageUpdated { get; private set; }
    
    public bool IsAdded(int id) => _addedElements.Contains(id);
    public bool IsDeleted(int id) => _deletedElements.Contains(id);
    public bool IsModified(int id) => _modifiedElements.Contains(id);
    
    public void ResetAll() { ResetAddedElements(); ResetDeletedElements(); ResetModifiedElements(); ResetUpdatedStorage(); }
    public void ResetAddedElements() => _addedElements = new HashSet<int>();
    public void ResetDeletedElements() => _deletedElements = new HashSet<int>();
    public void ResetModifiedElements() => _modifiedElements = new HashSet<int>();
    public void ResetUpdatedStorage() => IsStorageUpdated = false;
    public void PullProperties(ChangedElements prototype)
    {
        _addedElements = prototype._addedElements.ToHashSet();
        _deletedElements = prototype._deletedElements.ToHashSet();
        _modifiedElements = prototype._modifiedElements.ToHashSet();
        IsStorageUpdated = prototype.IsStorageUpdated;
    }
}