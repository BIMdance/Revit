namespace BIMdance.Revit.Logic.DataAccess.Context;

public interface ISet
{
    void Reset();
}

public interface ISet<TEntity> : ISet, IEnumerable<TEntity>
    where TEntity : class
{
    TEntity Add(TEntity entity);
    TEntity Find(object key);
    TEntity Create();
    TEntity Remove(TEntity entity);
    TEntity Update(TEntity entity);
    IQueryable<TEntity> Query();
}