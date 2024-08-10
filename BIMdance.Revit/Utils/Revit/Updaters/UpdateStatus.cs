namespace BIMdance.Revit.Utils.Revit.Updaters;

public class UpdateStatus
{
    public bool IsUpdating { get; set; }
}

// ReSharper disable once UnusedTypeParameter
public class UpdateStatus<TElement> : UpdateStatus
    where TElement : Element
{
    
}