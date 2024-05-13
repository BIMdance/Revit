namespace BIMdance.Revit.Logic.CableRouting.Model;

public class XYZProxy
{
    public XYZProxy() { }

    public XYZProxy(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public XYZProxy(XYZProxy prototype)
    {
        X = prototype.X;
        Y = prototype.Y;
        Z = prototype.Z;
    }
    
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public override bool Equals(object obj)
    {
        if (obj is not XYZProxy other)
            return false;

        return this.X.Equals(other.X) &&
               this.Y.Equals(other.Y) &&
               this.Z.Equals(other.Z);
    }

    public override string ToString()
    {
        return $"({X:0.######}, {Y:0.######}, {Z:0.######})";
    }
    
    public static XYZProxy operator +(XYZProxy point1, XYZProxy point2) =>
        new XYZProxy(
            point1.X + point2.X,
            point1.Y + point2.Y,
            point1.Z + point2.Z);

    public static XYZProxy operator -(XYZProxy point1, XYZProxy point2) =>
        new XYZProxy(
            point1.X - point2.X,
            point1.Y - point2.Y,
            point1.Z - point2.Z);

    public static XYZProxy operator *(XYZProxy point1, double multiplier) =>
        new XYZProxy(
            point1.X * multiplier,
            point1.Y * multiplier,
            point1.Z * multiplier);

    public static XYZProxy operator *(double multiplier, XYZProxy point1) =>
        point1 * multiplier;

    public static double operator *(XYZProxy point1, XYZProxy point2) =>
        point1.X * point2.X +
        point1.Y * point2.Y +
        point1.Z * point2.Z;

    public static double operator ^(XYZProxy point1, double power) =>
        Math.Pow(point1.X, power) +
        Math.Pow(point1.Y, power) +
        Math.Pow(point1.Z, power);

    public double SumAbsXYZ() => Math.Abs(this.X) + Math.Abs(this.Y) + Math.Abs(this.Z);
}