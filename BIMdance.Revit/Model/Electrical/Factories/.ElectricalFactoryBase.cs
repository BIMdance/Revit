using BIMdance.Revit.Logic.DataAccess;

namespace BIMdance.Revit.Model.Electrical.Factories;

public abstract class ElectricalFactoryBase<TElement>
    where TElement : ElementProxy
{
    private readonly Set<TElement> _set;
    private readonly string _namePrefix;

    protected ElectricalFactoryBase(
        ElectricalContext electricalContext,
        Set<TElement> set,
        string namePrefix = null)
    {
        _set = set;
        _namePrefix = namePrefix;
        ElectricalContext = electricalContext;
    }

    protected ElectricalContext ElectricalContext { get; }

    protected TElement Create(Func<TElement> createFunc)
    {
        return createFunc();
    }

    protected TElement CreateInContext(Func<TElement> createFunc)
    {
        var electrical = createFunc();
        _set.Add(electrical);
        return electrical;
    }

    protected int NewId() => ElectricalContext.NewId();
    
    protected string NewName()
    {
        var existNames = _set?.Select(x => x.Name).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        return NameUtils.GetNextName($"{_namePrefix}1", existNames);
    }
    
    protected string GetName(BuiltInCategoryProxy category) => ModelNameUtils.GetName(category);
    protected string GetName(ElectricalSystemTypeProxy systemTypeProxy) => ModelNameUtils.GetName(systemTypeProxy);
}