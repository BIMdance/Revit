namespace BIMdance.Revit.Logic.DataAccess;

public interface IRepositoryInitializer<T>
{
    public List<T> Initialize();
}