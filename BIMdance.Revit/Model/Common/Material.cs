namespace BIMdance.Revit.Model.Common;

public class Material : IPropertyPrototype<Material>
{
    public const string Al = "Al";
    public const string Cu = "Cu";

    public Material() { }
    public Material(string id) => Id = id;

    public string Id { get; set; }
    public int Density { get; set; }
    public double Resistivity { get; set; }
    public double TemperatureCoefficient { get; set; }

    public void PullProperties(Material prototype)
    {
        this.Id = prototype.Id;
        this.Density = prototype.Density;
        this.Resistivity = prototype.Resistivity;
        this.TemperatureCoefficient = prototype.TemperatureCoefficient;
    }

    public override string ToString() => Id;
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType() && Equals((Material)obj);
    }
    protected bool Equals(Material other) => Id == other.Id;
    
    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => Id?.GetHashCode() ?? 0;
}