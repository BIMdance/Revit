namespace BIMdance.Revit.Logic;

public class ElementComparer<T> : IEqualityComparer<T> where T : Element
{
    public bool Equals(T x, T y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (ReferenceEquals(x, null) ||
            ReferenceEquals(y, null))
            return false;

        return x.Id.GetValue() == y.Id.GetValue();
    }

    public int GetHashCode(T element) => (int)element.Id.GetValue();
}