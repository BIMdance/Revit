namespace BIMdance.Revit.Logic.CableRouting.Model;

public class SegmentProxy
{
    public SegmentProxy(XYZProxy point1, XYZProxy point2)
    {
        Point1 = point1;
        Point2 = point2;
    }
    public XYZProxy Point1 { get; set; }
    public XYZProxy Point2 { get; set; }

    public double DistanceToPoint(XYZProxy point) => point.DistanceTo(this);
}