using BIMdance.Revit.Utils.Ribbon.Definitions;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace BIMdance.Revit.Samples;

public class CommandSample : ExternalCommandDefinition
{
    public CommandSample() :
        base("External Command") { }

    protected override Result Execute()
    {
        TaskDialog.Show("", "Hello");
        return Result.Succeeded;
    }
}