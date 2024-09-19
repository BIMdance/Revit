namespace BIMdance.Revit.Model.Common;

public class Range<T>
{
    private readonly IComparer<T> _comparer;

    public Range(T value, IComparer<T> comparer = null) :
        this(value, value ,comparer) { }
    
    public Range(T minValue, T maxValue, IComparer<T> comparer = null)
    {
        _comparer = comparer ?? minValue switch
        {
            int => new IntComparer() as IComparer<T>,
            double => new DoubleComparer() as IComparer<T>,
            _ => null
        };

        MinValue = minValue;
        MaxValue = maxValue;
    }

    public T MinValue { get; }
    public T MaxValue { get; }
    public bool Contains(T value, IComparer<T> comparer = null)
    {
        comparer ??= _comparer;
        return comparer != null
            ? comparer.Compare(MinValue, value) <= 0 &&
              comparer.Compare(value, MaxValue) <= 0
            : throw new NullReferenceException(nameof(comparer));
    }

    public bool Contains(Range<T> otherRange, IComparer<T> comparer = null)
    {
        comparer ??= _comparer;
        return comparer != null
            ? comparer.Compare(MinValue, otherRange.MinValue) <= 0 &&
              comparer.Compare(otherRange.MaxValue, MaxValue) <= 0
            : throw new NullReferenceException(nameof(comparer));
    }

    public override string ToString() => $"{MinValue}...{MaxValue}";
}