namespace BIMdance.Revit.Utils.Common;

public abstract class UpdatableList<T> : List<T>, IPrototype<UpdatableList<T>>
    where T : class
{
    private readonly IEqualityComparer<T> _itemEqualityComparer;
    private readonly IEqualityComparer<T> _propertiesEqualityComparer;
    
    protected UpdatableList() { }
    protected UpdatableList(
        IEqualityComparer<T> itemEqualityComparer,
        IEqualityComparer<T> propertiesEqualityComparer = null)
    {
        _itemEqualityComparer = itemEqualityComparer;
        _propertiesEqualityComparer = propertiesEqualityComparer;
    }

    public void Sync(IEnumerable<T> newItems)
    {
        var itemsToDelete = this.ToList();
        foreach (var newItem in newItems) UpdateItem(newItem, itemsToDelete);
        foreach (var item in itemsToDelete) this.Remove(item);
    }

    private void UpdateItem(T newItem, ICollection<T> itemsToDelete)
    {
        var oldItem = this.FirstOrDefault(x => _itemEqualityComparer?.Equals(x, newItem) ?? Equals(x, newItem));

        if (oldItem != null)
        {
            if (oldItem is IPropertyPrototype<T> oldPropertyItem)
            {
                if (_propertiesEqualityComparer == null || !_propertiesEqualityComparer.Equals(oldItem, newItem))
                    oldPropertyItem.PullProperties(newItem);
            }
            else
            {
                this.Remove(oldItem);
                this.Add(newItem);
            }
            
            itemsToDelete.Remove(oldItem);
        }
        else
        {
            this.Add(newItem);
        }
    }

    public virtual UpdatableList<T> Clone()
    {
        var constructor = GetType().GetConstructor(Type.EmptyTypes);
        var clone = constructor?.Invoke(Array.Empty<object>()) as UpdatableList<T>;

        clone?.Clear();
        clone?.AddRange(typeof(IPrototype<T>).IsAssignableFrom(typeof(T))
            ? this.Select(x => (x as IPrototype<T>)?.Clone())
            : typeof(ICloneable).IsAssignableFrom(typeof(T))
                ? this.Select(x => (x as ICloneable)?.Clone() as T)
                : this);
        
        return clone;
    }
}