namespace BIMdance.Revit.Logic.DataAccess.Context;

public interface IUnitOfWork : IDisposable
{
    void SaveChanges();
    void SaveChanges(string transactionName);
}