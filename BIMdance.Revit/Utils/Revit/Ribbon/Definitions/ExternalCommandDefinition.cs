namespace BIMdance.Revit.Utils.Revit.Ribbon.Definitions;

public abstract class ExternalCommandDefinition : IExternalCommand
{
    private readonly string _commandName;
    private readonly string _currentSessionGuid;
    private readonly string _version;
        
    protected readonly IServiceProvider ServiceProvider;
    protected readonly RevitContext RevitContext;
    protected Document Document;
    protected ExternalCommandData ExternalCommandData;
    protected ElementSet Elements;
    protected string Message;
    
    protected abstract Result Execute();
        
    public CommandDefinition Definition { get; }

    protected ExternalCommandDefinition(
        string caption, Bitmap largeImage = null, Bitmap image = null,
        string longDescription = null, string toolTipText = null, Bitmap toolTipImage = null) :
        this(Locator.ServiceProvider, caption, largeImage, image, longDescription, toolTipText, toolTipImage) { }
    
    protected ExternalCommandDefinition(
        IServiceProvider serviceProvider,
        string caption, Bitmap largeImage = null, Bitmap image = null,
        string longDescription = null, string toolTipText = null, Bitmap toolTipImage = null)
    {
        ServiceProvider = serviceProvider;
        RevitContext = ServiceProvider?.Get<RevitContext>();
        var appInfo = ServiceProvider?.Get<AppInfo>();
        _currentSessionGuid = appInfo?.CurrentSessionGuid.ToString();
        _version = appInfo?.Version;
        _commandName = GetType().Name;
        
        Definition = new CommandDefinition
        {
            Name = _commandName,
            Caption = caption,
            LargeImage = largeImage,
            Image = image,
            LongDescription = longDescription,
            ToolTipText = toolTipText,
            ToolTipImage = toolTipImage,
        };
    }
        
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        try
        {
            ExternalCommandData = commandData;
            Message = message;
            Elements = elements;
            Document = ExternalCommandData.Application.ActiveUIDocument.Document;
            RevitContext.Document = Document;
            RevitContext.UIApplication = commandData.Application;

            Logger.Info(
                $"Command: {_commandName}\n\t" +
                $"Application version: {_version}, " +
                $"Revit version: {RevitContext.Version}, " +
                $"Session: {_currentSessionGuid}\n\t" +
                $"Document: {Document.Title}");
                
            return Execute();
        }
        catch (OperationCanceledException)
        {
            return Result.Cancelled;
        }
        catch (Autodesk.Revit.Exceptions.OperationCanceledException)
        {
            return Result.Cancelled;
        }
        catch (Exception exception)
        {
            Logger.Error(exception);
            return Result.Failed;
        }
        finally
        {
            try
            {
                
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }
    }
}