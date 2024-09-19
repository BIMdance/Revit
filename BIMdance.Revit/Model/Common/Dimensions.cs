namespace BIMdance.Revit.Model.Common;

public class Dimensions
{
    public static Dimensions CreateFromMillimeters(double heightMillimeters, double widthMillimeters, double depthMillimeters) =>
        new(heightMillimeters, widthMillimeters, depthMillimeters);
    public static Dimensions CreateFromMeters(double heightMillimeters, double widthMillimeters, double depthMillimeters) =>
        new(heightMillimeters / 1000, widthMillimeters / 1000, depthMillimeters / 1000);

    public Dimensions() { }
        
    public Dimensions(double height, double width, double depth)
    {
        Height = height;
        Width = width;
        Depth = depth;
    }

    public double Height { get; }
    public double Width { get; }
    public double Depth { get; }
        
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is Dimensions otherDimension && Equals(otherDimension);
    }
    private bool Equals(Dimensions other) => Equals(Height, other.Height) && Equals(Width, other.Width) && Equals(Depth, other.Depth);
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Height.GetHashCode();
            hashCode = (hashCode * 397) ^ Width.GetHashCode();
            hashCode = (hashCode * 397) ^ Depth.GetHashCode();
            return hashCode;
        }
    }
        
    public override string ToString() => $"{Height} × {Width} × {Depth}";
}