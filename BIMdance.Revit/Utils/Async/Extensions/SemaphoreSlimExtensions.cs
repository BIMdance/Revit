namespace BIMdance.Revit.Utils.Async.Extensions;

public static class SemaphoreSlimExtensions
{
    public static Task AsyncWait(this SemaphoreSlim semaphore)
    {
        return semaphore.WaitAsync();
    }
}