namespace BIMdance.Revit;

public static class Logger
{
    public static event EventHandler<Exception> OnError;
    public static event EventHandler<string> OnDebug;
    public static event EventHandler<string> OnInfo;
    public static event EventHandler<string> OnWarn;
    
    public static void Error(Exception exception)
    {
        Console.WriteLine(exception);
        OnError?.Invoke(null, exception);
    }
    
    public static void Debug(string message)
    {
        Console.WriteLine(message);
        OnDebug?.Invoke(null, message);
    }
    
    public static void Info(string message)
    {
        Console.WriteLine(message);
        OnInfo?.Invoke(null, message);
    }
    
    public static void Warn(string message)
    {
        Console.WriteLine(message);
        OnWarn?.Invoke(null, message);
    }
}