namespace BIMdance.Revit.Model.Common;

public interface IPropertyPrototype<in T> where T : class
{
    void PullProperties(T prototype);
}