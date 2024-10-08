namespace BIMdance.Revit.Logic.Converters;

public abstract class ProxyConverter<T> where T : class
{
    protected Document Document { get; }
    protected ProxyConverter(Document document) => Document = document;
    public abstract List<T> Convert();
}