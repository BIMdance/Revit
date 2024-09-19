using System.Diagnostics.CodeAnalysis;

namespace BIMdance.Revit.Model.Electrical.Products;

public class ConductorCount
{
    public ConductorCount() { }

    public ConductorCount(int count, double section)
    {
        Count = count;
        Section = section;
        Id = $"{Count}×{Section}";
    }
        
    public string Id { get; set; }
    public int Count { get; set; }
    public double Section { get; set; }
    public override string ToString() => Id;
        
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType() && Equals((ConductorCount)obj);
    }

    protected bool Equals(ConductorCount other) => Id == other.Id;

    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode() => Id != null ? Id.GetHashCode() : 0;
}