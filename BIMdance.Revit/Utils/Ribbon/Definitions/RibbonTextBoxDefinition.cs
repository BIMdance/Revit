using Binding = System.Windows.Data.Binding;

namespace BIMdance.Revit.Utils.Ribbon.Definitions;

public class RibbonTextBoxDefinition : IRibbonDefinition
{
    public double Width { get; set; }
    public Binding Binding { get; set; }
    public List<RibbonTextBox> SubscribedTextBoxes = new();
}