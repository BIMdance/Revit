namespace BIMdance.Revit.Logic.Parameters.Shared;

public class ProjectSharedParameter
{
    public ProjectSharedParameter(
        BaseSharedParameterDefinition baseSharedParameterDefinition,
        BuiltInCategory category) :
        this(baseSharedParameterDefinition, new List<BuiltInCategory> { category }) { }
    
    public ProjectSharedParameter(
        BaseSharedParameterDefinition baseSharedParameterDefinition,
        IReadOnlyCollection<BuiltInCategory> categories)
    {
        if (categories.IsEmpty())
            throw new ArgumentException($"{nameof(categories)} must not be empty.");
            
        BaseSharedParameterDefinition = baseSharedParameterDefinition;
        IsInstance = baseSharedParameterDefinition.IsInstance;
        Categories = categories;
    }

    public ProjectSharedParameter(
        BaseSharedParameterDefinition baseSharedParameterDefinition,
        bool isInstance,
        BuiltInCategory category) :
        this(baseSharedParameterDefinition, isInstance, new List<BuiltInCategory> { category }) { }
    
    public ProjectSharedParameter(
        BaseSharedParameterDefinition baseSharedParameterDefinition,
        bool isInstance,
        IReadOnlyCollection<BuiltInCategory> categories)
    {
        if (categories.IsEmpty())
            throw new ArgumentException($"{nameof(categories)} must not be empty.");

        BaseSharedParameterDefinition = baseSharedParameterDefinition;
        IsInstance = isInstance;
        Categories = categories;
    }

    public BaseSharedParameterDefinition BaseSharedParameterDefinition { get; }
    public bool IsInstance { get; }
    public IReadOnlyCollection<BuiltInCategory> Categories { get; }
    public Guid Guid => BaseSharedParameterDefinition.Guid;

    public CategorySet GetCategorySet(Document document)
    {
        var categorySet = document.Application.Create.NewCategorySet();

        foreach (var builtInCategory in Categories)
            categorySet.Insert(document.Settings.Categories.get_Item(builtInCategory));

        return categorySet;
    }

    public override string ToString() => $"{BaseSharedParameterDefinition}{Categories.JoinToString()}";
}