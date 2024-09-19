namespace BIMdance.Revit.Logic.Comparers;

public class RevitProxyComparer : IEqualityComparer<ElementProxy>
{
    public bool Equals(ElementProxy x, ElementProxy y)
    {

        if (ReferenceEquals(x, y)) return true;

        if (x == null || y == null)
            return false;

        return x.RevitId == y.RevitId;
    }

    public int GetHashCode(ElementProxy element) => (int)element.RevitId;
}

public class RevitProxyComparer<T> : IEqualityComparer<T> where T : ElementProxy
{
    public bool Equals(T x, T y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x == null || y == null)
            return false;

        return x.RevitId == y.RevitId;
    }

    public int GetHashCode(T element) => (int)element.RevitId;
}