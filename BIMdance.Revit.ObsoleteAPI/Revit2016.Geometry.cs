namespace BIMdance.Revit.ObsoleteAPI;

public static partial class Revit2016
{
    public static class Geometry
    {
        public static Plane CreatePlaneByNormalAndOrigin(
            XYZ normal,
            XYZ origin) =>
            new(normal, origin);
    }
}