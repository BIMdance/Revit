namespace BIMdance.Revit.Utils.Revit;

public static class ReferenceUtils
{
    public static int GetIntegerValue(this FamilyInstanceReferenceTypeProxy familyInstanceReferenceType) => familyInstanceReferenceType switch
    {
        FamilyInstanceReferenceTypeProxy.NotAReference   => 12,
        FamilyInstanceReferenceTypeProxy.StrongReference => 13,
        FamilyInstanceReferenceTypeProxy.WeakReference   => 14,
        _ => (int)familyInstanceReferenceType
    };
}