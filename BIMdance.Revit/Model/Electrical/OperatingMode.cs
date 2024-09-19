namespace BIMdance.Revit.Model.Electrical;

public class OperatingMode : ElementProxy, IPrototype<OperatingMode>, IPropertyPrototype<OperatingMode>
{
    public const int DefaultOperatingModeId = -1;
    public static OperatingMode CreateDefault() => new(DefaultOperatingModeId, ModelLocalization.Normal);
    public OperatingMode() { }
    internal OperatingMode(int revitId, string name) : base(revitId, name) { }
    private OperatingMode(OperatingMode prototype) : this() => this.PullProperties(prototype);
    public OperatingMode Clone() => new(this);
    public void PullProperties(OperatingMode prototype)
    {
        this.RevitId = prototype.RevitId;
        this.Name = prototype.Name;
        this.DisabledElements = prototype.DisabledElements.ToHashSet();
    }
    
    public Dictionary<ElementProxy, bool> ElementSwitches { get; set; } = new();
    public HashSet<int> DisabledElements { get; private set; } = new();
    public override string ToString() => $"[{RevitId}] <{GetType().Name}> {Name}";
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType() && Equals((OperatingMode)obj);
    }
    private bool Equals(OperatingMode other) => RevitId.Equals(other.RevitId);
    public override int GetHashCode() => RevitId.GetHashCode();
}