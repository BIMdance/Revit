namespace BIMdance.Revit.Logic.CableRouting;

public static class SegmentFilter
{
    public static bool FilterLeftUpDown(SegmentProxy segment, double boundaryLeft, double boundaryUp, double boundaryDown)
    {
        var point0 = segment.Point1;
        var point1 = segment.Point2;

        return (point0.X >= boundaryLeft || point1.X >= boundaryLeft) &&
               (point0.Y >= boundaryDown || point1.Y >= boundaryDown) &&
               (point0.Y <= boundaryUp || point1.Y <= boundaryUp);
    }

    public static bool FilterRight(SegmentProxy segment, double boundaryRight)
    {
        var point0 = segment.Point1;
        var point1 = segment.Point2;

        return (point0.X <= boundaryRight || point1.X <= boundaryRight);
    }

    public static bool FilterLeftUpDown(SegmentProxy segment, XYZProxy point)
    {
        var point0 = segment.Point1;
        var point1 = segment.Point2;

        return (point0.X >= point.X || point1.X >= point.X) &&
               (point0.Y >= point.Y && point1.Y <= point.Y) ||
               (point0.Y <= point.Y && point1.Y >= point.Y);
    }
}