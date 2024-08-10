using Transform = Autodesk.Revit.DB.Transform;

namespace BIMdance.Revit.Utils;

public static class RevitPickUtils
{
    public static bool Pick<T>(this UIDocument uiDocument, string statusPrompt, out T element)
        where T : Element
    {
        var reference = uiDocument.Selection.PickObject(
            ObjectType.Element,
            new SelectionFilter<T, RevitLinkInstance>(),
            statusPrompt);
        
        switch (uiDocument.Document.GetElement(reference))
        {
            case T referenceElement: element = referenceElement; return true;
            default: element = null; return false;
        }
    }
    
    public static bool Pick<T>(this UIDocument uiDocument, string statusPrompt, out T element, out Transform transform)
        where T : Element
    {
        transform = null;
        var reference = uiDocument.Selection.PickObject(
            ObjectType.Element,
            new SelectionFilter<T, RevitLinkInstance>(),
            statusPrompt);
        
        switch (uiDocument.Document.GetElement(reference))
        {
            case T referenceElement: element = referenceElement; return true;
            case RevitLinkInstance linkInstance: return uiDocument.PickInRevitLink(statusPrompt, linkInstance, out element, out transform); 
            default: element = null; return false;
        }
    }
    
    public static bool PickInRevitLink<T>(this UIDocument uiDocument, string statusPrompt, RevitLinkInstance linkInstance, out T element, out Transform transform)
        where T : Element
    {
        var reference = uiDocument.Selection.PickObject(ObjectType.LinkedElement, statusPrompt);
        element = linkInstance.GetLinkDocument().GetElement(reference.LinkedElementId) as T;
        transform = linkInstance.GetTotalTransform();
        return element is not null;
    }
    
    public static void While(Action action)
    {
        try { while (true) action(); }
        catch (Autodesk.Revit.Exceptions.OperationCanceledException) { }
    }

    public static async Task WhileAsync(Func<Task> task)
    {
        try
        {
            while (true)
            {
                var isCanceled = true;
                
                await RevitTask.RunAsync(async () =>
                {
                    await task();
                    isCanceled = false;
                });

                if (isCanceled)
                    throw new OperationCanceledException();
            }
        }
        catch (OperationCanceledException) { }
        catch (Autodesk.Revit.Exceptions.OperationCanceledException) { }
    }
}