namespace BIMdance.Revit.Model.Electrical.Products.Characteristics;

public class Poles
{
    public Poles() { }

    public Poles(string polesDescription, string protectedPolesDescription = null)
    {
        Total = polesDescription?.Split('+').Sum(n => n.FirstInt(defaultValue: 1)) ?? default;
        Protected = protectedPolesDescription?.FirstInt(defaultValue: Total) ?? Total;
    }

    public Poles(int poles, int protectedPoles = default)
    {
        Total = poles;
        Protected = protectedPoles != default ? protectedPoles : Total;
    }

    public int Total { get; set; }
    public int Protected { get; set; }
    public string SpecialNeutralPole { get; set; }
    public string FullName => $"{Total}P{Protected}d";
    public override string ToString()
    {
        return $"{Total}P{(Protected < Total ? $"{Protected}d" : string.Empty)}";
    }
}