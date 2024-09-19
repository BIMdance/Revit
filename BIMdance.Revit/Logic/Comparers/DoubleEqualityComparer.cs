namespace BIMdance.Revit.Logic.Comparers;

public class DoubleEqualityComparer : IEqualityComparer<double>
{
    private readonly double _tolerance;
    public DoubleEqualityComparer(double tolerance = 1e-6) => _tolerance = tolerance;
    public bool Equals(double x, double y) => System.Math.Abs(x - y) <= _tolerance;
    public int GetHashCode(double obj) => obj.GetHashCode();
}