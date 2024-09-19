namespace BIMdance.Revit.Logic.Comparers;

public class IntComparer : IComparer<int>
{
    public int Compare(int x, int y) => x > y ? 1 : x < y ? -1 : 0;
}