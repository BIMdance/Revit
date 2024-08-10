namespace BIMdance.Revit.Utils.Revit.Parameters.Shared;

public class SharedParameterDefinition : BaseSharedParameterDefinition
{
    public SharedParameterDefinition(
        Guid guid,
        string name,
        ParameterTypeProxy parameterType,
        ParameterGroupProxy parameterGroup = ParameterGroupProxy.INVALID,
        bool isInstance = false,
        bool isVisible = true,
        bool isUserModifiable = true,
        string description = null) : base(guid, parameterGroup, isInstance)
    {
        Name = name;
        ParameterType = parameterType;
        Description = description;
        IsUserModifiable = isUserModifiable;
        IsVisible = isVisible;
    }

    public ParameterTypeProxy ParameterType { get; }
    public string Name { get; }
    public string Description { get; }
    public bool IsUserModifiable { get; }
    public bool IsVisible { get; }
}