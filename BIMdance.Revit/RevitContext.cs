// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace BIMdance.Revit;

public delegate void DocumentChanged(Document oldDocument, Document newDocument);

public class RevitContext
{
    private readonly ServiceScope _serviceScope;
    private Document _document;
    public event DocumentChanged OnDocumentChanged;
    
    public RevitContext(ServiceScope serviceScope, UIControlledApplication uiControlledApplication)
    {
        _serviceScope = serviceScope;
        UIControlledApplication = uiControlledApplication;
        UIControlledApplication.ViewActivated += OnViewActivated;
        Version = UIControlledApplication.ControlledApplication.VersionNumber.FirstInt();
    }
        
    public UIControlledApplication UIControlledApplication { get; }
    public UIApplication UIApplication { get; set; }
    public UIDocument UIDocument => UIApplication.ActiveUIDocument;
    public Selection Selection => UIDocument.Selection;
    public int Version { get; }

    public Document Document
    {
        get => _document;
        set
        {
            var oldDocument = _document?.IsValidObject ?? false ? _document : null;
            
            if (Equals(oldDocument, value))
                return;
            
            if (value is not null)
            {
                value.DocumentClosing -= OnDocumentClosing;
                value.DocumentClosing += OnDocumentClosing;
            }
            
            _document = value;
            OnDocumentChanged?.Invoke(oldDocument, newDocument: value);
        }
    }
    
    private void OnDocumentClosing(object sender, DocumentClosingEventArgs e)
    {
        try { _serviceScope.Remove(e.Document); }
        catch (Exception exception) { Logger.Error(exception); }
    }
    
    private void OnViewActivated(object sender, ViewActivatedEventArgs e)
    {
        try { if (e.CurrentActiveView is not null && e.Document is not null) Document = e.Document; }
        catch (Exception exception) { Logger.Error(exception); }
    }
}