namespace BIMdance.Revit.Async.Utils;

internal static class TaskUtils
{
    public static Task<T> FromResult<T>(T value)
    {
        return Task.FromResult(value);
    }
}