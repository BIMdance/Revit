using RibbonItem = Autodesk.Revit.UI.RibbonItem;
using RibbonPanel = Autodesk.Revit.UI.RibbonPanel;

namespace BIMdance.Revit.Ribbon;

public class RibbonVisibleUtils
{
    private readonly UIControlledApplication _application;

    public RibbonVisibleUtils(
        UIControlledApplication application)
    {
        _application = application;
    }
        
    public void SetVisibleCondition(RibbonTab ribbonPanel, RibbonVisible ribbonVisible, Func<bool> checkFunc = null)
    {
        if (ribbonVisible == RibbonVisible.All)
            return;
        
        _application.ViewActivated += (_, e) =>
        {
            try
            {
                ribbonPanel.IsVisible = CheckVisible(e.Document, ribbonVisible) && (checkFunc?.Invoke() ?? true);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        };
    }
        
    public void SetVisibleCondition(RibbonPanel ribbonPanel, RibbonVisible ribbonVisible, Func<bool> checkFunc = null)
    {
        if (ribbonVisible == RibbonVisible.All)
            return;
        
        _application.ViewActivated += (_, e) =>
        {
            try
            {
                ribbonPanel.Visible = CheckVisible(e.Document, ribbonVisible) && (checkFunc?.Invoke() ?? true);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        };
    }

    public void SetVisibleCondition(RibbonItem ribbonItem, RibbonVisible ribbonVisible, Func<bool> checkFunc = null)
    {
        if (ribbonVisible == RibbonVisible.All)
            return;
        
        _application.ViewActivated += (_, e) =>
        {
            try
            {
                ribbonItem.Visible = CheckVisible(e.Document, ribbonVisible) && (checkFunc?.Invoke() ?? true);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        };
    }

    public void SetVisibleCondition(RibbonItem ribbonItem, Func<View, bool> checkFunc) =>
        _application.ViewActivated += (_, e) =>
        {
            try
            {
                ribbonItem.Visible = checkFunc(e.CurrentActiveView);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
                
            }
        };

    public void SetVisibleCondition(RibbonItem ribbonItem, ViewType checkViewType, Func<View, bool> checkFunc = null) =>
        _application.ViewActivated += (_, e) =>
        {
            try
            {
                ribbonItem.Visible = checkViewType == e.CurrentActiveView.ViewType && (checkFunc?.Invoke(e.CurrentActiveView) ?? true);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        };

    private static bool CheckVisible(Document document, RibbonVisible ribbonVisible) =>
        ribbonVisible switch
        {
            RibbonVisible.Family => document.IsFamilyDocument,
            RibbonVisible.Project => !document.IsFamilyDocument,
            RibbonVisible.All => true,
            _ => throw new ArgumentOutOfRangeException(nameof(ribbonVisible), ribbonVisible, null)
        };
}