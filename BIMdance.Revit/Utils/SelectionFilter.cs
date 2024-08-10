namespace BIMdance.Revit.Utils;

public class SelectionFilter<T1>: ISelectionFilter
    where T1 : Element
{
    public bool AllowElement(Element element) => element is T1;
    public bool AllowReference(Reference reference, XYZ position) => true;
}

public class SelectionFilter<T1, T2> : ISelectionFilter
    where T1 : Element
    where T2 : Element
{
    public bool AllowElement(Element element) => element is T1 or T2;
    public bool AllowReference(Reference reference, XYZ position) => true;
}

public class SelectionFilter<T1, T2, T3> : ISelectionFilter
    where T1 : Element
    where T2 : Element
    where T3 : Element
{
    public bool AllowElement(Element element) => element is T1 or T2 or T3;
    public bool AllowReference(Reference reference, XYZ position) => true;
}

public class SelectionFilter<T1, T2, T3, T4> : ISelectionFilter
    where T1 : Element
    where T2 : Element
    where T3 : Element
    where T4 : Element
{
    public bool AllowElement(Element element) => element is T1 or T2 or T3 or T4;
    public bool AllowReference(Reference reference, XYZ position) => true;
}