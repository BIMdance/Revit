namespace BIMdance.Revit.Model.RevitProxy;

public class RoomProxy : ElementProxy
{
    public string Number { get; set; }
    public long LevelId { get; set; }
    public LevelProxy Level { get; set; }
    public List<List<Line3D>> Segments { get; set; }

    public bool IsPointInRoom(Point3D point)
    {
        var intersectionCount = 0;

        foreach (var areaSegments in Segments)
        {
            Line3D previousSegment = null;

            for (var index = 0; index < areaSegments.Count; index++)
            {
                var segment = areaSegments[index];
                var distanceToSegment = point.DistanceTo(segment);

                if (distanceToSegment < 0.01)
                    return true;

                switch (IsIntersectRightRay(point, segment))
                {
                    case Intersection.Cross:
                        intersectionCount++;
                        break;
                    
                    case Intersection.Touch when previousSegment != null:
                        if (IsIntersectSegmentPair(point, segment, previousSegment))
                            intersectionCount++;
                        previousSegment = null;
                        break;

                    case Intersection.Touch:
                        previousSegment = segment;
                        break;

                    case Intersection.LayOn when index == 0:
                        previousSegment = areaSegments.Last();
                        break;
                }
            }
        }

        return intersectionCount % 2 == 1;
    }

    /// <summary>
    /// Check intersection of the segment and the ray that started from rayStartPoint and directed to the right.
    /// </summary>
    /// <param name="rayStartPoint">The start of the ray directed to the right.</param>
    /// <param name="segment">The verified segment.</param>
    /// <returns></returns>
    private static Intersection IsIntersectRightRay(Point3D rayStartPoint, Line3D segment)
    {
        if (rayStartPoint.X > segment.Point1.X && rayStartPoint.X > segment.Point2.X ||
            rayStartPoint.Y > segment.Point1.Y && rayStartPoint.Y > segment.Point2.Y ||
            rayStartPoint.Y < segment.Point1.Y && rayStartPoint.Y < segment.Point2.Y)
        {
            return Intersection.No;
        }

        if (rayStartPoint.X < segment.Point1.X && rayStartPoint.X < segment.Point2.X &&
            (rayStartPoint.Y < segment.Point1.Y && rayStartPoint.Y > segment.Point2.Y ||
             rayStartPoint.Y > segment.Point1.Y && rayStartPoint.Y < segment.Point2.Y))
        {
            return Intersection.Cross;
        }

        if ((rayStartPoint.X > segment.Point1.X && rayStartPoint.X < segment.Point2.X ||
             rayStartPoint.X < segment.Point1.X && rayStartPoint.X > segment.Point2.X) &&
            (rayStartPoint.Y > segment.Point1.Y && rayStartPoint.Y < segment.Point2.Y ||
             rayStartPoint.Y < segment.Point1.Y && rayStartPoint.Y > segment.Point2.Y))
        {
            return rayStartPoint.X < rayStartPoint.ProjectToLine(segment.Point1, segment.Point2).X
                ? Intersection.Cross
                : Intersection.No;
        }

        var point1OnRay = rayStartPoint.X < segment.Point1.X && Math.Abs(rayStartPoint.Y - segment.Point1.Y) < 1e-6; 
        var point2OnRay = rayStartPoint.X < segment.Point2.X && Math.Abs(rayStartPoint.Y - segment.Point2.Y) < 1e-6;

        if (point1OnRay && point2OnRay)
        {
            return Intersection.LayOn;
        }

        if (point1OnRay || point2OnRay)
        {
            return Intersection.Touch;
        }

        return Intersection.No;
    }

    /*
     * true:
     *   segment1 \
     *         *---*--- point.Y
     *     segment2 \
     * 
     * false:
     *   segment1 \  / segment2
     *         *---*--- point.Y
     */
    private static bool IsIntersectSegmentPair(Point3D point, Line3D segment1, Line3D segment2)
    {
        var segment1MaxY = Math.Max(segment1.Point1.Y, segment1.Point2.Y);
        var segment1MinY = Math.Min(segment1.Point1.Y, segment1.Point2.Y);
        var segment2MaxY = Math.Max(segment2.Point1.Y, segment2.Point2.Y);
        var segment2MinY = Math.Min(segment2.Point1.Y, segment2.Point2.Y);

        return segment1MaxY > point.Y && segment2MinY < point.Y ||
               segment2MaxY > point.Y && segment1MinY < point.Y;
    }

    private enum Intersection
    {
        No,
        Cross,
        Touch,
        LayOn
    }
}