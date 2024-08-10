namespace BIMdance.Revit.Model.Geometry;

public class Point3D
{
    public Point3D() { }

    public Point3D(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Point3D(Point3D prototype)
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
        if (obj is not Point3D other)
            return false;

        return this.X.Equals(other.X) &&
               this.Y.Equals(other.Y) &&
               this.Z.Equals(other.Z);
    }

    public override string ToString()
    {
        return $"({X:0.######}, {Y:0.######}, {Z:0.######})";
    }
    
    public static Point3D operator +(Point3D point1, Point3D point2) =>
        new Point3D(
            point1.X + point2.X,
            point1.Y + point2.Y,
            point1.Z + point2.Z);

    public static Point3D operator -(Point3D point1, Point3D point2) =>
        new Point3D(
            point1.X - point2.X,
            point1.Y - point2.Y,
            point1.Z - point2.Z);

    public static Point3D operator *(Point3D point1, double multiplier) =>
        new Point3D(
            point1.X * multiplier,
            point1.Y * multiplier,
            point1.Z * multiplier);

    public static Point3D operator *(double multiplier, Point3D point1) =>
        point1 * multiplier;

    public static double operator *(Point3D point1, Point3D point2) =>
        point1.X * point2.X +
        point1.Y * point2.Y +
        point1.Z * point2.Z;

    public static double operator ^(Point3D point1, double power) =>
        Math.Pow(point1.X, power) +
        Math.Pow(point1.Y, power) +
        Math.Pow(point1.Z, power);

    public double SumAbsXYZ() => Math.Abs(this.X) + Math.Abs(this.Y) + Math.Abs(this.Z);
}