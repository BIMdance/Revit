namespace BIMdance.Revit.Utils.Revit.Ribbon.Definitions;

public class RibbonCheckBoxDefinition : IRibbonDefinition
{
    public string Text { get; set; }
    public RibbonCheckedBinding RibbonCheckedBinding { get; set; }
}