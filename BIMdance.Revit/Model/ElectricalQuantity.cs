// ReSharper disable InconsistentNaming

namespace BIMdance.Revit.Model;

public class ElectricalQuantity
{
    public static ElectricalQuantity Create(double value, string unit = null) => new(value, unit, ac: null);
    public static ElectricalQuantity CreateAC(double value, string unit = null) => new(value, unit, ac: true);
    public static ElectricalQuantity CreateDC(double value, string unit = null) => new(value, unit, ac: false);
    
    public ElectricalQuantity(double value, string unit = null, bool? ac = true)
    {
        Value = value;
        Unit = unit;
        AC = ac;
    }

    public double Value { get; }
    public string Unit { get; }
    public bool? AC { get; }
    public bool? DC => !AC ;

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType() && Equals((ElectricalQuantity)obj);
    }
    private bool Equals(ElectricalQuantity other) => AC == other.AC && Value.IsEqualTo(other.Value) && Unit == other.Unit;
    public override int GetHashCode()
    {
        unchecked
        {
            return (Value.GetHashCode() * 397) ^ AC.GetHashCode();
        }
    }

    public override string ToString() => !string.IsNullOrWhiteSpace(Unit)
        ? $"{Value} {Unit}{(AC is true ? " AC" : DC is true ? " DC" : string.Empty)}"
        : $"{Value}";
}