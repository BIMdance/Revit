using System.Diagnostics.CodeAnalysis;

namespace BIMdance.Revit.Model.Electrical.Products;

public class Conductor
{
    public Conductor() { }

    public Conductor(double sectionAsSquareMillimeters)
    {
        Section = sectionAsSquareMillimeters;
        Id = Section.ToString(CultureInfo.InvariantCulture);
    }

    public string Id { get; set; }
    public double Section { get; set; }
    public override string ToString() => Id;

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType() && Equals((Conductor)obj);
    }

    protected bool Equals(Conductor other) => Id == other.Id;

    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode() => Id != null ? Id.GetHashCode() : 0;
}