namespace BIMdance.Revit.Model.RevitProxy;

public class VoltageTypeProxy : ElementProxy, IPrototype<VoltageTypeProxy>, IPropertyPrototype<VoltageTypeProxy>
{
    public VoltageTypeProxy() { }
    public VoltageTypeProxy(int revitId, string name) : base(revitId, name) { }
    public VoltageTypeProxy(int revitId, string name, double actualValue, double minValue, double maxValue) :
        base(revitId, name)
    {
        ActualValue = actualValue;
        MinValue = minValue;
        MaxValue = maxValue;
    }
    
    private VoltageTypeProxy(VoltageTypeProxy prototype) => PullProperties(prototype);
    public VoltageTypeProxy Clone() => new(this);
    public void PullProperties(VoltageTypeProxy prototype)
    {
        this.Name = prototype.Name;
        this.RevitId = prototype.RevitId;
        this.ActualValue = prototype.ActualValue;
        this.MinValue = prototype.MinValue;
        this.MaxValue = prototype.MaxValue;
    }

    public double ActualValue { get; set; }
    public double MaxValue { get; set; }
    public double MinValue { get; set; }

    public override string ToString() => $"{base.ToString()} {ActualValue} | {MinValue} | {MaxValue}";

    public static IEqualityComparer<VoltageTypeProxy> PropertiesEqualityComparer { get; } = new VoltageTypeProxyEqualityComparer();

    private sealed class VoltageTypeProxyEqualityComparer : IEqualityComparer<VoltageTypeProxy>
    {
        public bool Equals(VoltageTypeProxy x, VoltageTypeProxy y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
                
            return Equals(x.Name, y.Name) &&
                   Equals(x.ActualValue, y.ActualValue) &&
                   Equals(x.MaxValue, y.MaxValue) &&
                   Equals(x.MinValue, y.MinValue);
        }

        public int GetHashCode(VoltageTypeProxy obj)
        {
            unchecked
            {
                var hashCode = obj.ActualValue.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.MaxValue.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.MinValue.GetHashCode();
                return hashCode;
            }
        }
    }
}