namespace BIMdance.Revit.Utils.Common;

internal static class CollectionExtension
{
    internal static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
    {
        if (target == null || source == null)
            return;

        foreach (var element in source)
            target.Add(element);
    }

    public static ICollection<T> Inverse<T>(this ICollection<T> items)
    {
        var result = new List<T>();

        for (var i = items.Count - 1; i >= 0; i--)
            result.Add(items.ElementAt(i));

        return result;
    }

    public static bool IsFirst<T>(this IList<T> collection, T item) => collection.IndexOf(item) == 0;
    public static bool IsLast<T>(this IList<T> collection, T item) => collection.IndexOf(item) == collection.Count - 1;
    public static bool IsFirst<T>(this T[] collection, T item) => Array.IndexOf(collection, item) == 0;
    public static bool IsLast<T>(this T[] collection, T item) => Array.IndexOf(collection, item) == collection.Length - 1;

    public static List<T> Clone<T>(this IEnumerable<T> items) where T : class
    {
        return
            typeof(IPrototype<T>).IsAssignableFrom(typeof(T)) ? items.Select(x => (x as IPrototype<T>)?.Clone()).ToList() :
            typeof(ICloneable).IsAssignableFrom(typeof(T)) ? items.Select(x => (x as ICloneable)?.Clone() as T).ToList() :
            items.ToList();
    }
    
    public static ICollection<T> ReplaceAll<T>(this ICollection<T> items, IEnumerable<T> newItems)
    {
        items.Clear();
        items.AddRange(newItems);
        return items;
    }
}