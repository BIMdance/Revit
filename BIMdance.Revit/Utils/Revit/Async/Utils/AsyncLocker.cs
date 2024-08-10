using BIMdance.Revit.Utils.Revit.Async.Extensions;

namespace BIMdance.Revit.Utils.Revit.Async.Utils;

public class AsyncLocker
{
    public AsyncLocker()
    {
        Semaphore = new SemaphoreSlim(1, 1);
    }
        
    private SemaphoreSlim Semaphore { get; }

    public Task<UnlockKey> LockAsync()
    {
        var waitTask = Semaphore.AsyncWait();
        return waitTask.IsCompleted
            ? TaskUtils.FromResult(new UnlockKey(this))
            : waitTask.ContinueWith(_ => new UnlockKey(this),
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }

    internal void Release()
    {
        Semaphore.Release();
    }
}