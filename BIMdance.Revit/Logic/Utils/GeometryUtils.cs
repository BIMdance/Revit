using Vector = BIMdance.Revit.Model.Geometry.Vector;

namespace BIMdance.Revit.Logic.Utils;

internal static class GeometryUtils
{
    internal static double DistanceTo(this Point3D point1, Point3D point2) =>
        Math.Sqrt(point1.RatingDistanceTo(point2));

    internal static double DistanceTo(this Point3D point, Line3D segment) =>
        point.DistanceToSegment(segment.Point1, segment.Point2);
    
    internal static double DistanceToSegment(this Point3D point, Point3D segmentPoint1, Point3D segmentPoint2)
    {
        var projectPoint = point.ProjectToLine(segmentPoint1, segmentPoint2);
        var lengthLine = segmentPoint1.RatingDistanceTo(segmentPoint2);
        var segment1 = segmentPoint1.RatingDistanceTo(projectPoint);
        var segment2 = segmentPoint2.RatingDistanceTo(projectPoint);

        return segment1 + segment2 > lengthLine + 0.0001
            ? Math.Min(point.DistanceTo(segmentPoint1), point.DistanceTo(segmentPoint2))
            : point.DistanceTo(projectPoint);
    }
    
    internal static double DistanceToOnPlane(this Point3D point1, Point3D point2, Normal normal)
    {
        var thisProject = point1.OnPlane(normal);
        var pointProject = point2.OnPlane(normal);

        return thisProject.DistanceTo(pointProject);
    }
    
    internal static Point3D OnPlane(this Point3D point, Normal normal)
    {
        return normal switch
        {
            Normal.X => new Point3D(0, point.Y, point.Z),
            Normal.Y => new Point3D(point.X, 0, point.Z),
            Normal.Z => new Point3D(point.X, point.Y, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(normal), normal, null)
        };
    }
    
    internal static double RatingDistanceToSegment(this Point3D point, Point3D linePoint1, Point3D linePoint2)
    {
        var projectPoint = ProjectToLine(point, linePoint1, linePoint2);
        var lengthLine = RatingDistanceTo(linePoint1, linePoint2);
        var segment1 = RatingDistanceTo(linePoint1, projectPoint);
        var segment2 = RatingDistanceTo(linePoint2, projectPoint);

        return segment1 + segment2 > lengthLine
            ? Math.Min(RatingDistanceTo(point, linePoint1), RatingDistanceTo(point, linePoint2))
            : RatingDistanceTo(point, projectPoint);
    }
    
    internal static Point3D ProjectToLine(this Point3D point, Point3D linePoint1, Point3D linePoint2)
    {
        var s1 = linePoint1 - linePoint2;
        return linePoint2 - (linePoint2 - point) * s1 / (s1 ^ 2) * s1;
    }
    
    internal static Point3D ProjectToSegment(this Point3D point, Point3D segmentPoint1, Point3D segmentPoint2)
    {
        var projectPoint = point.ProjectToLine(segmentPoint1, segmentPoint2);
        var lengthLine = segmentPoint1.RatingDistanceTo(segmentPoint2);
        var segment1 = segmentPoint1.RatingDistanceTo(projectPoint);
        var segment2 = segmentPoint2.RatingDistanceTo(projectPoint);

        return segment1 + segment2 < lengthLine
            ? projectPoint
            : segment1 < segment2 ? segmentPoint1 : segmentPoint2;
    }
    
    internal static double RatingDistanceTo(this Point3D point1, Point3D point2) =>
        Math.Pow(point1.X - point2.X, 2) +
        Math.Pow(point1.Y - point2.Y, 2) +
        Math.Pow(point1.Z - point2.Z, 2);

    internal static bool IsIntersect(this Line3D segment1, Line3D segment2)
    {
        return IsCross(segment1, segment2) && IsCross(segment2, segment1);
    }

    private static bool IsCross(Line3D segment1, Line3D segment2)
    {
        var vector = new Vector(segment1.Point1, segment1.Point2);
        var vector1 = new Vector(segment1.Point1, segment2.Point1);
        var vector2 = new Vector(segment1.Point1, segment2.Point2);
        var product1 = vector * vector1;
        var product2 = vector * vector2;

        return Math.Sign(product1.Z) != Math.Sign(product2.Z);
    }
}