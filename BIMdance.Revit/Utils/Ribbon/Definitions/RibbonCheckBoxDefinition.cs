using BIMdance.Revit.Utils.Ribbon.Bindings;

namespace BIMdance.Revit.Utils.Ribbon.Definitions;

public class RibbonCheckBoxDefinition : IRibbonDefinition
{
    public string Text { get; set; }
    public RibbonCheckedBinding RibbonCheckedBinding { get; set; }
}