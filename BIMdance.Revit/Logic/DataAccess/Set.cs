namespace BIMdance.Revit.Logic.DataAccess;

public abstract class Set<T> : ElementProvider, IList<T> where T : class
{
    private readonly List<T> _items = new();
    private readonly PropertyInfo _changedElementIdProperty;
    private readonly PropertyInfo _keyProperty;
    protected internal readonly IRepositoryInitializer<T> RepositoryInitializer;

    protected Set(IRepositoryInitializer<T> repositoryInitializer) : this()
    {
        RepositoryInitializer = repositoryInitializer;
    }

    protected Set()
    {
        var typeProperties = typeof(T).GetProperties();
        var intProperties = typeProperties.Where(x => x.PropertyType == typeof(int)).ToArray();

        _changedElementIdProperty =
            intProperties.FirstOrDefault(x => x.Name is "Id") ??
            intProperties.FirstOrDefault(x => x.Name.EndsWith("Id"));

        _keyProperty =
            typeProperties.FirstOrDefault(x => x.Name is "Guid") ??
            typeProperties.FirstOrDefault(x => x.Name is "Id") ??
            typeProperties.FirstOrDefault(x => x.Name is "Key") ??
            typeProperties.FirstOrDefault(x => x.Name is "Name");
    }

    public void ReplaceAll(IEnumerable<T> collection)
    {
        _items.Clear();
        _items.AddRange(collection);
    }

    public ChangedElements ChangedElements { get; } = new();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

    public T this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    public void AddRange(IEnumerable<T> items) => items.ForEach(Add);

    public void Add(T item)
    {
        // Logger.Debug($"{GetType().Name}.{nameof(Add)}({item})");

        _items.Add(item);

        if (TryGetChangedElementId(item, out var id))
            ChangedElements.Create(id);
    }

    public void Insert(int index, T item)
    {
        // Logger.Debug($"{GetType().Name}.{nameof(Insert)}({index}, {item})");

        _items.Insert(index, item);

        if (TryGetChangedElementId(item, out var id))
            ChangedElements.Create(id);
    }

    public void Remove(IEnumerable<T> items) => items.ToArray().ForEach(x => Remove(x));

    public bool Remove(T item)
    {
        // Logger.Debug($"{GetType().Name}.{nameof(Remove)}({item})");

        var remove = _items.Remove(item);

        if (remove && TryGetChangedElementId(item, out var id))
            ChangedElements.Delete(id);

        return remove;
    }

    public void RemoveAt(int index) => Remove(_items[index]);
    public void Update(IEnumerable<T> items) => items.ForEach(Update);

    public void Update(T item)
    {
        // Logger.Debug($"{GetType().Name}.{nameof(Update)}({item})");

        if (TryGetChangedElementId(item, out var id))
            ChangedElements.Modify(id);
    }

    public void Clear() => _items.Clear();
    public bool Contains(T item) => _items.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
    public int IndexOf(T item) => _items.IndexOf(item);
    public int Count => _items.Count;
    public bool IsReadOnly => false;

    public virtual T Get(object key) => _keyProperty != null
        ? this.FirstOrDefault(x => Equals(_keyProperty.GetValue(x), key))
        : throw new InvalidOperationException($"Type {typeof(T)} doesn't have any key property: Guid, Id, Key or Name");

    private bool TryGetChangedElementId(T item, out int id) => int.TryParse(
        _changedElementIdProperty?.GetValue(item)?.ToString(),
        out id);
}