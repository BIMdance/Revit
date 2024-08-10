namespace BIMdance.Revit.Model.Geometry;

public class Line3D
{
    public Line3D(Point3D point1, Point3D point2)
    {
        Point1 = point1;
        Point2 = point2;
    }
    public Point3D Point1 { get; set; }
    public Point3D Point2 { get; set; }

    public double DistanceToPoint(Point3D point) => point.DistanceTo(this);
}