namespace BIMdance.Revit.Utils.Async.Utils;

public readonly struct UnlockKey : IDisposable
{
    private AsyncLocker Locker { get; }

    internal UnlockKey(AsyncLocker locker)
    {
        Locker = locker;
    }

    public void Dispose()
    {
        Locker?.Release();
    }
}