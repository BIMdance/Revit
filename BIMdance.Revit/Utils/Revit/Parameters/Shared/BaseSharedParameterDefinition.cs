// ReSharper disable LocalizableElement

namespace BIMdance.Revit.Utils.Revit.Parameters.Shared;

public class BaseSharedParameterDefinition
{
    public BaseSharedParameterDefinition(
        Guid guid,
        ParameterGroupProxy parameterGroup = ParameterGroupProxy.INVALID,
        bool isInstance = false)
    {
        Guid = guid;
        ParameterGroup = parameterGroup;
        IsInstance = isInstance;
    }
        
    public Guid Guid { get; }
    public ParameterGroupProxy ParameterGroup { get; }
    public bool IsInstance { get; }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() &&
               Equals((BaseSharedParameterDefinition)obj);
    }

    private bool Equals(BaseSharedParameterDefinition other) => Guid.Equals(other.Guid);
    public override int GetHashCode() => Guid.GetHashCode();
    public override string ToString() => $"[{Guid}]";
}