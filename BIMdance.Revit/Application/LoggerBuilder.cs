namespace BIMdance.Revit.Application;

public class LoggerBuilder
{
    private readonly IServiceProvider _serviceProvider;
    public LoggerBuilder() { }
    public LoggerBuilder(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public LoggerBuilder OnException(Action<Exception> errorAction)
    {
        Logger.OnException += (_, exception) => errorAction(exception);
        return this;
    }

    public LoggerBuilder OnException(Action<IServiceProvider, Exception> errorAction)
    {
        Logger.OnException += (_, exception) => errorAction(_serviceProvider, exception);
        return this;
    }
    
    public LoggerBuilder OnError(Action<string> errorAction)
    {
        Logger.OnError += (_, message) => errorAction(message);
        return this;
    }
    
    public LoggerBuilder OnError(Action<IServiceProvider, string> errorAction)
    {
        Logger.OnError += (_, message) => errorAction(_serviceProvider, message);
        return this;
    }
    
    public LoggerBuilder OnDebug(Action<string> debugAction)
    {
        Logger.OnDebug += (_, message) => debugAction(message);
        return this;
    }
    
    public LoggerBuilder OnDebug(Action<IServiceProvider, string> debugAction)
    {
        Logger.OnDebug += (_, message) => debugAction(_serviceProvider, message);
        return this;
    }
    
    public LoggerBuilder OnInfo(Action<string> infoAction)
    {
        Logger.OnInfo += (_, message) => infoAction(message);
        return this;
    }
    
    public LoggerBuilder OnInfo(Action<IServiceProvider, string> infoAction)
    {
        Logger.OnInfo += (_, message) => infoAction(_serviceProvider, message);
        return this;
    }
    
    public LoggerBuilder OnWarn(Action<string> warnAction)
    {
        Logger.OnWarn += (_, message) => warnAction(message);
        return this;
    }
    
    public LoggerBuilder OnWarn(Action<IServiceProvider, string> warnAction)
    {
        Logger.OnWarn += (_, message) => warnAction(_serviceProvider, message);
        return this;
    }
}