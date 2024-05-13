namespace BIMdance.Revit.Logic.CableRouting.Model;

public class Vector
{
    public Vector(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector(XYZProxy point1, XYZProxy point2)
    {
        X = point2.X - point1.X;
        Y = point2.Y - point1.Y;
        Z = point2.Z - point1.Z;
    }
    
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    
    public static Vector operator *(Vector vector1, Vector vector2)
    {
        return new Vector(
            x: vector1.Y * vector2.Z - vector1.Z * vector2.Y,
            y: vector1.Z * vector2.X - vector1.X * vector2.Z,
            z: vector1.X * vector2.Y - vector1.Y * vector2.X);
    }
}