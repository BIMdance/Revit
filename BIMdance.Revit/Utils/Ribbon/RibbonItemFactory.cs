using AutodeskRibbonItem = Autodesk.Windows.RibbonItem;
using Binding = System.Windows.Data.Binding;
using RevitRibbonItem = Autodesk.Revit.UI.RibbonItem;
using RevitRibbonPanel = Autodesk.Revit.UI.RibbonPanel;

namespace BIMdance.Revit.Utils.Ribbon;

public class RibbonItemFactory
{
    private int _indexPulldownButton;
    private readonly string _assemblyPath;

    public RibbonItemFactory(string assemblyPath)
    {
        _assemblyPath = assemblyPath;
    }
        
    public AutodeskRibbonItem CreateRibbonItem(IRibbonDefinition definition)
    {
        return definition switch
        {
            RibbonCheckBoxDefinition ribbonCheckBoxDefinition => CreateRibbonCheckBoxItem(ribbonCheckBoxDefinition),
            RibbonComboDefinition ribbonComboDefinition => CreateRibbonComboItem(ribbonComboDefinition),
            RibbonLabelDefinition ribbonLabelDefinition => CreateRibbonLabelItem(ribbonLabelDefinition),
            RibbonTextBoxDefinition ribbonTextBoxDefinition => CreateRibbonTextBoxItem(ribbonTextBoxDefinition),
            EmptyRibbonDefinition _ => CreateEmptyItem(),
            _ => throw new ArgumentOutOfRangeException(nameof(definition), definition,
                new ArgumentOutOfRangeException().Message)
        };
    }

    public RibbonCheckBox CreateRibbonCheckBoxItem(RibbonCheckBoxDefinition ribbonCheckBoxDefinition)
    {
        return new RibbonCheckBox
        {
            Text = ribbonCheckBoxDefinition.Text,
            IsCheckedBinding = new Binding
            {
                Source = ribbonCheckBoxDefinition.RibbonCheckedBinding,
                Path = new PropertyPath(nameof(RibbonCheckedBinding.IsChecked)),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            },
        };
    }

    public RibbonCombo CreateRibbonComboItem(RibbonComboDefinition ribbonComboDefinition)
    {
        return new RibbonCombo
        {
            // TODO
        };
    }

    public RibbonLabel CreateRibbonLabelItem(RibbonLabelDefinition ribbonLabelDefinition)
    {
        return new RibbonLabel
        {
            // TODO
            Text = "  " + ribbonLabelDefinition.Text + " ",
            Height = 23
        };
    }

    public RibbonTextBox CreateRibbonTextBoxItem(RibbonTextBoxDefinition ribbonTextBoxDefinition)
    {
        var textBox = new RibbonTextBox
        {
            // TODO
            Width = ribbonTextBoxDefinition.Width,
            TextValueBinding = ribbonTextBoxDefinition.Binding,
        };

        ribbonTextBoxDefinition.SubscribedTextBoxes.Add(textBox);

        return textBox;
    }

    private AutodeskRibbonItem CreateEmptyItem()
    {
        return new RibbonLabel { Height = 23 };
    }
        
    public RevitRibbonItem CreatePushButton(
        RevitRibbonPanel ribbonPanel,
        ExternalCommandDefinition definitionExternalCommand)
    {
        var pushButtonData = CreatePushButtonData(definitionExternalCommand);
        return ribbonPanel.AddItem(pushButtonData);
    }

    public PushButtonData CreatePushButtonData(
        ExternalCommandDefinition definitionExternalCommand)
    {
        var definition = definitionExternalCommand.Definition;
        var name = $"pb_{definition.Name}";
        var text = definition.Caption;
        var className = definitionExternalCommand.GetType().FullName;

        var pushButtonData = new PushButtonData(name, text, _assemblyPath, className)
        {
            LongDescription = definition.LongDescription,
            ToolTip = definition.ToolTipText,
            Image = definition.Image?.ToBitmapSource(),
            LargeImage = definition.LargeImage?.ToBitmapSource(),
            ToolTipImage = definition.ToolTipImage?.ToBitmapSource()
        };

        return pushButtonData;
    }

    public PulldownButton CreatePulldownButton(
        RevitRibbonPanel ribbonPanel,
        ExternalCommandDefinition pulldownButtonDefinition)
    {
        var definition = pulldownButtonDefinition.Definition;
        var pulldownButtonData = new PulldownButtonData("PulldownButtonData" + _indexPulldownButton++, definition.Caption)
        {
            LongDescription = definition.LongDescription,
            ToolTip = definition.ToolTipText,
            Image = definition.Image?.ToBitmapSource(),
            LargeImage = definition.LargeImage?.ToBitmapSource(),
            ToolTipImage = definition.ToolTipImage?.ToBitmapSource(),
        };

        return ribbonPanel.AddItem(pulldownButtonData) as PulldownButton;
    }
}