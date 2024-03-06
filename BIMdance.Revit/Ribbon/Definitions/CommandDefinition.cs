using System.Resources;

namespace BIMdance.Revit.Ribbon.Definitions;

public class CommandDefinition
{
    public string Name { get; set; }
    public string Caption { get; set; }
    public string LongDescription { get; set; }
    public string ToolTipText { get; set; }
    public Bitmap Image { get; set; }
    public Bitmap LargeImage { get; set; }
    public Bitmap ToolTipImage { get; set; }

    public CommandDefinition() { }
    public CommandDefinition(Type type)
    {
        var resourceManager = new ResourceManager(type);
        Name = type.Name;
        Caption = resourceManager.GetString(nameof(Caption));
        LongDescription = resourceManager.GetString(nameof(LongDescription));
        ToolTipText = resourceManager.GetString(nameof(ToolTipText));
        Image = resourceManager.GetObject(nameof(Image)) as Bitmap;
        LargeImage = resourceManager.GetObject(nameof(LargeImage)) as Bitmap;
        ToolTipImage = null; // ResourceManager.GetObject(nameof(TooltipImage)) as Bitmap,
    }
}