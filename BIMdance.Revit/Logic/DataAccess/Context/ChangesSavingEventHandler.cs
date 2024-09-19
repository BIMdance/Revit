namespace BIMdance.Revit.Logic.DataAccess.Context;

public delegate void ChangesSavingEventHandler(object sender, ChangesSavingArgs e);
    
public class ChangesSavingArgs : EventArgs
{
    public ChangesSavingArgs(string transactionName) => TransactionName = transactionName;
    public string TransactionName { get; }
}