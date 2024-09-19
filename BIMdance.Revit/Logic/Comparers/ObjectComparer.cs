namespace BIMdance.Revit.Logic.Comparers;

public class ObjectComparer : IEqualityComparer<object>
{
    public new bool Equals(object x, object y)
    {
        if (x == null && y == null)
            return true;

        if (x == null || y == null)
            return false;

        return ReferenceEquals(x, y) || x.Equals(y);
    }

    public int GetHashCode(object obj)
    {
        return obj?.GetHashCode() ?? -1;
    }
}

public class ObjectComparer<T> : IEqualityComparer<T>
{
    bool IEqualityComparer<T>.Equals(T x, T y)
    {
        if (x == null && y == null)
            return true;

        if (x == null || y == null)
            return false;

        return ReferenceEquals(x, y) || x.Equals(y);
    }

    public int GetHashCode(T obj)
    {
        return obj?.GetHashCode() ?? -1;
    }
}