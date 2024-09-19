namespace BIMdance.Revit.Logic.DataAccess;

/// <summary>
/// Используется для передачи объектов между несвязанными классами.
/// </summary>
public class ElementClipboard
{
    private readonly List<Instance> _instances;

    public ElementClipboard() => _instances = new List<Instance>();

    public void Push<T>(T element, int index = 0) where T : class
    {
        Drop<T>(index);
        _instances.Add(new Instance(element, index));
    }

    public T Peak<T>(int index = 0) where T : class
    {
        _instances.RemoveAll(n => n == null);
        var type = typeof(T);
            
        return type.IsInterface
            ? _instances.FirstOrDefault(n => n.Index == index && n.Element.GetType().GetInterface(type.Name) != null)?.Element as T
            : _instances.FirstOrDefault(n => n.Index == index && n.Element is T)?.Element as T;
    }

    public T Pop<T>(int index = 0) where T : class
    {
        try { return Peak<T>(index); }
        finally { Drop<T>(index); }
    }

    public void Drop<T>(int index = 0) where T : class => _instances.RemoveAll(n => n == null || (n.Index == index && n.Element is T));
    public void DropAll<T>() where T : class => _instances.RemoveAll(n => n == null || n.Element is T);
    public void DropAll() => _instances.Clear();

    private class Instance
    {
        public Instance(object element, int index = 0)
        {
            Guid = Guid.NewGuid();
            Element = element;
            Index = index;
        }
            
        public object Element { get; }
        public int Index { get; }
        public Guid Guid { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((Instance)obj);
        }
        private bool Equals(Instance other) => Guid.Equals(other.Guid);
        public override int GetHashCode() => Guid.GetHashCode();
    }
}