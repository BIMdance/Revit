namespace BIMdance.Revit.Model.Electrical.Products;

public class CableSeries
{
    public CableSeries() { }
        
    public CableSeries(string id, Material conductorMaterial, CableInsulation cableInsulation) :
        this()
    {
        Id = id;
        ConductorMaterial = conductorMaterial;
        Insulation = cableInsulation;
        CableProperties = new List<CableProperty>();
    }

    public string Id { get; set; }
    public bool Armor { get; set; }
    public bool Shield { get; set; }
    public Material ConductorMaterial { get; set; }
    public CableInsulation Insulation { get; set; }
    public List<CableProperty> CableProperties { get; set; }
    public List<Cable> Cables { get; set; } = new();
    public IEnumerator<Cable> GetEnumerator() => Cables.GetEnumerator();
    
    public void Add(Cable item)
    {
        if (item == null)
            throw new NullReferenceException(nameof(item));

        if (!Contains(item))
            Cables.Add(item);
        
        item.CableSeries = this;
    }
    public void Clear() => Cables.Clear();
    public bool Contains(Cable item) => Cables.Contains(item);
    public void CopyTo(Cable[] array, int arrayIndex) => Cables.CopyTo(array, arrayIndex);
    public bool Remove(Cable item)
    {
        if (item == null)
            throw new NullReferenceException(nameof(item));

        item.CableSeries = null;
        return Cables.Remove(item);
    }
    public int Count => Cables.Count;
    public bool IsReadOnly => false;

    protected bool Equals(CableSeries other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CableSeries)obj);
    }

    public override int GetHashCode()
    {
        return Id?.GetHashCode() ?? 0;
    }

    public override string ToString()
    {
        return $"<{GetType().Name}> {Id}, {nameof(ConductorMaterial)}: {ConductorMaterial?.Id}, {nameof(Count)}: {Count}";
    }
}