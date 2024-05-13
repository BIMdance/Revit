namespace BIMdance.Revit.Logic.CableRouting.Model;

public abstract class ElementProxy
{
    public const int NotCreatedInRevitId = -1;
        
    public Guid Guid { get; set; }
    public long RevitId { get; set; }
    public string Name { get; set; }

    protected ElementProxy(string name = null) :
        this(Guid.NewGuid(), NotCreatedInRevitId, name) { }
    
    protected ElementProxy(int revitId, string name = null) :
        this(Guid.NewGuid(), revitId, name) { }

    protected ElementProxy(Guid guid, string name) :
        this(guid, NotCreatedInRevitId, name) { }

    protected ElementProxy(Guid guid, int revitId, string name)
    {
        Guid = guid;
        RevitId = revitId;
        Name = name;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType() &&
               Equals((ElementProxy) obj);
    }

    protected bool Equals(ElementProxy other) => Guid.Equals(other.Guid);
    public override int GetHashCode() => Guid.GetHashCode();
    public override string ToString() => $"[{Guid} / {RevitId}] <{GetType().Name}> {Name}";
}