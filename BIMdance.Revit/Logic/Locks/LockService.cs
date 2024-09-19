namespace BIMdance.Revit.Logic.Locks;

public class LockService : IDisposable
{
    private readonly Lock[] _locks;
    private static readonly List<int> LockKeys = new();
        
    public static LockService ToLock(params Lock[] locks) => new(locks);

    public static void ToUnlock(params Lock[] locks)
    {
        foreach (var @lock in locks)
            LockKeys.Remove((int) @lock);
    }

    public static void UnlockEverywhere(Lock @lock) =>
        LockKeys.RemoveAll(n => n.Equals((int)@lock));

    public static bool IsLocked(params Lock[] locks) =>
        locks.Any(n => LockKeys.Contains((int)n));

    public static void SetLock(Lock @lock, bool isLocked)
    {
        if (isLocked)
            AddLock(@lock);
        else
            UnlockEverywhere(@lock);
    }

    public LockService(Lock[] locks)
    {
        _locks = locks;
        AddLock(locks);
    }

    private static void AddLock(params Lock[] locks) =>
        LockKeys.AddRange(locks.Select(n => (int)n));

    public void Dispose() => ToUnlock(_locks);
}