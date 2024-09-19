namespace BIMdance.Revit.Logic.Comparers;

public class DoubleComparer : IComparer<double>
{
    public int Compare(double x, double y) => x.IsEqualTo(y) ? 0 : x > y ? 1 : -1;
}