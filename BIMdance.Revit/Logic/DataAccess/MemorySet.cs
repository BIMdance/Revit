namespace BIMdance.Revit.Logic.DataAccess;

/// <summary>
/// Используется для работы с коллекцией элементов типа T без возможности сохранения её в файл.
/// </summary>
/// <typeparam name="T">Тип элементов коллекции</typeparam>
public abstract class MemorySet<T> : Set<T> where T : class
{
    protected MemorySet(IRepositoryInitializer<T> repositoryInitializer) :
        base(repositoryInitializer) => Load();
    public sealed override void Load() => ReplaceAll(RepositoryInitializer.Initialize());
    public override void Save() => Debug.WriteLine($"Repository<{typeof(T).Name}> is configured to store only in memory. It will not be written.", GetType().Namespace);
}