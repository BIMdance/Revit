namespace BIMdance.Revit.Logic.DataAccess;

public abstract class ElementProvider
{
    public abstract void Load();
    public abstract void Save();
}

public abstract class ElementProvider<T> : ElementProvider
    where T : class
{
    public bool IsChanged { get; set; }
    public T Element { get; set; }
}