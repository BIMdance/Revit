namespace BIMdance.Revit.Model.Electrical;

public class ElectricalProduct : Product
{
    public ElectricalProduct(BaseProduct baseProduct) : base(baseProduct) { }
    public double BreakingCapacity { get; set; }
    public double RatedCurrent { get; set; }
    public override string ToString()
    {
        var characteristics = new[]
        {
            RatedCurrent.ToStringInvariant(),
            BreakingCapacity.ToStringInvariant(),
        };

        return $"{base.ToString()} {characteristics.Where(n => n != null).JoinToString(", ")}";
    }
}