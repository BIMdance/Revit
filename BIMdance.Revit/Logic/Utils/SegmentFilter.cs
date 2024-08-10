namespace BIMdance.Revit.Logic.Utils;

public static class SegmentFilter
{
    public static bool FilterLeftUpDown(Line3D segment, double boundaryLeft, double boundaryUp, double boundaryDown)
    {
        var point0 = segment.Point1;
        var point1 = segment.Point2;

        return (point0.X >= boundaryLeft || point1.X >= boundaryLeft) &&
               (point0.Y >= boundaryDown || point1.Y >= boundaryDown) &&
               (point0.Y <= boundaryUp || point1.Y <= boundaryUp);
    }

    public static bool FilterRight(Line3D segment, double boundaryRight)
    {
        var point0 = segment.Point1;
        var point1 = segment.Point2;

        return (point0.X <= boundaryRight || point1.X <= boundaryRight);
    }

    public static bool FilterLeftUpDown(Line3D segment, Point3D point)
    {
        var point0 = segment.Point1;
        var point1 = segment.Point2;

        return (point0.X >= point.X || point1.X >= point.X) &&
               (point0.Y >= point.Y && point1.Y <= point.Y) ||
               (point0.Y <= point.Y && point1.Y >= point.Y);
    }
}