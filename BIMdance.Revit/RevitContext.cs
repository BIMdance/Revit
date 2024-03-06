// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace BIMdance.Revit;

public delegate void DocumentChanged(Document oldDocument, Document newDocument);

public class RevitContext
{
    private Document _document;
    public event DocumentChanged OnDocumentChanged;
    
    public RevitContext(UIControlledApplication uiControlledApplication)
    {
        UIControlledApplication = uiControlledApplication;
        IsRevitStarted = true;
        Version = UIControlledApplication.ControlledApplication.VersionNumber.FirstInt();
    }
        
    public bool IsRevitStarted { get; }
    public UIControlledApplication UIControlledApplication { get; }
    public UIApplication UIApplication { get; set; }
    public UIDocument UIDocument => UIApplication.ActiveUIDocument;
    public Selection Selection => UIDocument.Selection;

    public Document Document
    {
        get => _document;
        set
        {
            if (Equals(_document, value)) return;
            var oldDocument = _document;
            _document = value;
            OnDocumentChanged?.Invoke(oldDocument, newDocument: value);
        }
    }

    public string AssemblyPath { get; set; }
    public int Version { get; }
}