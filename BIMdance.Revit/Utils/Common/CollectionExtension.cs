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
}