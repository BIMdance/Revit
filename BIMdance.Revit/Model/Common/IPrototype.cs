namespace BIMdance.Revit.Model.Common;

public interface IPrototype<out T>
{
    T Clone();
}