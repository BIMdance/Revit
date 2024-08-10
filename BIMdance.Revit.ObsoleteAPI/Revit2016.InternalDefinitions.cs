namespace BIMdance.Revit.ObsoleteAPI;

public static partial class Revit2016
{
    public static class InternalDefinitions
    {
        public static bool Equals(
            InternalDefinition internalDefinition1,
            InternalDefinition internalDefinition2) =>
            internalDefinition1.Name == internalDefinition2.Name;
    }
}