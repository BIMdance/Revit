namespace BIMdance.Revit.Model.Context;

public class TransactionGroupProvider
{
    public Action<Action, string> TransactionGroupAction { get; set; }
}