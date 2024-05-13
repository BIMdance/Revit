namespace BIMdance.Revit.Utils;

internal class RevitIdEqualityComparer<T> : IEqualityComparer<T> where T : ElementProxy
{
    public bool Equals(T x, T y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        return x.GetType() == y.GetType() && x.RevitId.Equals(y.RevitId);
    }

    public int GetHashCode(T obj) => (int)obj.RevitId;
}