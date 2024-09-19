namespace BIMdance.Revit.Model.Electrical.Products;

public class Cable : IPrototype<Cable>, IPropertyPrototype<Cable>
{
    public Cable() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cableSeries"></param>
    /// <param name="conductorCount"></param>
    /// <param name="square">mm²</param>
    /// <param name="diameter">mm</param>
    /// <param name="weightPerMeter">kg</param>
    public Cable(CableSeries cableSeries,
        int conductorCount, double square,
        double diameter, double weightPerMeter) :
        this(cableSeries, conductorCount, square)
    {
        Diameter = diameter;
        WeightPerMeter = weightPerMeter;
    }
        
    public Cable(CableSeries cableSeries, int conductorCount, double squareMillimeters) :
        this(cableSeries, new ConductorCount(conductorCount, squareMillimeters)) { }

    public Cable(CableSeries cableSeries, params ConductorCount[] conductorCounts) :
        this()
    {
        cableSeries.Add(this);
        Conductors = conductorCounts.ToList();
        Id = string.Join(" ", new[] { CableSeries?.Id, GetSections() }.Where(n => string.IsNullOrEmpty(n) == false));
    }

    private Cable(Cable prototype)
    {
        PullProperties(prototype);
    }

    public void PullProperties(Cable prototype)
    {
        this.Id = prototype.Id;
        this.Description = prototype.Description;
        this.CableSeries = prototype.CableSeries;
        this.Conductors = prototype.Conductors.ToList();
        this.ConductorType = prototype.ConductorType;
        this.PermissibleCurrent = prototype.PermissibleCurrent;
        this.GroundPermissibleCurrent = prototype.GroundPermissibleCurrent;
        this.Diameter = prototype.Diameter;
        this.R1 = prototype.R1;
        this.X1 = prototype.X1;
        this.R0 = prototype.R0;
        this.X0 = prototype.X0;
        this.WeightPerMeter = prototype.WeightPerMeter;
    }

    public Cable Clone()
    {
        return new Cable(this);
    }

    public string Id { get; set; }
    public string Description { get; set; }
    public CableSeries CableSeries { get; set; }
    public List<ConductorCount> Conductors { get; set; } = new();
    public ConductorType ConductorType { get; set; }
    public double PermissibleCurrent { get; set; }
    public double GroundPermissibleCurrent { get; set; }
    public double Diameter { get; set; }
    public double R1 { get; set; }
    public double X1 { get; set; }
    public double R0 { get; set; }
    public double X0 { get; set; }
    public double WeightPerMeter { get; set; }

    public override string ToString() => $"<{GetType().Name}> {Id}";

    protected bool Equals(Cable other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Cable)obj);
    }

    public override int GetHashCode()
    {
        return Id?.GetHashCode() ?? default;
    }

    public string GetSections() => string.Join("+", Conductors.Where(n => n != null));
}