namespace BIMdance.Revit;

public static class Logger
{
    public static event EventHandler<Exception> OnException;
    public static event EventHandler<string> OnError;
    public static event EventHandler<string> OnDebug;
    public static event EventHandler<string> OnInfo;
    public static event EventHandler<string> OnWarn;
    
    public static void Error(Exception exception)
    {
        Console.WriteLine(exception);
        try { OnException?.Invoke(null, exception); }
        catch (Exception e) { Console.WriteLine(e); }
    }

    public static void Error() => Error(string.Empty);
    public static void Error(string message, MethodBase method) => Error($"{message}\n\t{method.DeclaringType?.FullName}.{method.Name}");
    public static void Error(string message)
    {
        Console.WriteLine($@"ERROR: {message}");
        try { OnError?.Invoke(null, message); }
        catch (Exception e) { Console.WriteLine(e); }
    }

    public static void Debug() => Debug(string.Empty);
    public static void Debug(string message, MethodBase method) => Debug($"{message}\n\t{method.DeclaringType?.FullName}.{method.Name}");
    public static void Debug(string message)
    {
        Console.WriteLine($@"DEBUG: {message}");
        try { OnDebug?.Invoke(null, message); }
        catch (Exception e) { Console.WriteLine(e); }
    }
    
    public static void Info() => Info(string.Empty);
    public static void Info(string message, MethodBase method) => Info($"{message}\n\t{method.DeclaringType?.FullName}.{method.Name}");
    public static void Info(string message)
    {
        Console.WriteLine($@"INFO: {message}");
        try { OnInfo?.Invoke(null, message); }
        catch (Exception e) { Console.WriteLine(e); }
    }
    
    public static void Warn() => Warn(string.Empty);
    public static void Warn(string message, MethodBase method) => Warn($"{message}\n\t{method.DeclaringType?.FullName}.{method.Name}");
    public static void Warn(string message)
    {
        Console.WriteLine($@"WARN: {message}");
        try { OnWarn?.Invoke(null, message); }
        catch (Exception e) { Console.WriteLine(e); }
    }
}