namespace BIMdance.Revit.Utils;

public static class EnumerableExtension
{
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection) =>
        collection == null ? new ObservableCollection<T>() : new ObservableCollection<T>(collection);

    public static string JoinToString<T>(this IEnumerable<T> enumerable, string separator = " ") =>
        $"{(separator.StartsWith("\n") ? separator : string.Empty)}{string.Join(separator, enumerable)}";

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        var array = enumerable?.ToArray();
        
        if (array is null)
            return null;

        foreach (var item in array)
            action.Invoke(item);

        return array;
    }

    public static bool IsEmpty<T>(this IEnumerable<T> enumerable) =>
        (enumerable?.Any() ?? false) == false;
}