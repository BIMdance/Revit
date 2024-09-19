namespace BIMdance.Revit.Utils.Common;

public class UniqueCollection<T> : ICollection<T>, IPrototype<UniqueCollection<T>>
{
    private readonly List<T> _items;

    public UniqueCollection() : this(new List<T>()) { }

    public UniqueCollection(IEnumerable<T> source, bool isReadOnly = false)
    {
        _items = new List<T>(source);
        IsReadOnly = isReadOnly;
    }

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(T item)
    {
        if (item == null)
            return;

        if (IsReadOnly)
            throw new InvalidOperationException($"Collection is read only");
            
        if (Contains(item))
            throw new ArgumentException($"An item has already been added to the collection: {item} ({typeof(T)})");

        _items.Add(item);
    }

    public void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
            Add(item);
    }

    public void Clear() => _items.Clear();
    public bool Contains(T item) => _items.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
    public bool Remove(T item) => _items.Remove(item);
        
    public int Count => _items.Count;
    public bool IsReadOnly { get; set; }
    public UniqueCollection<T> Clone() => new(_items, IsReadOnly);
}