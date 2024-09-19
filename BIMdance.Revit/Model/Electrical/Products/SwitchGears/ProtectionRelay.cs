using BIMdance.Revit.Model.Electrical.MediumVoltage;

namespace BIMdance.Revit.Model.Electrical.Products.SwitchGears;

public class ProtectionRelay : Manufactured, ICompatibleComponents<Manufactured>, IPrototype<ProtectionRelay>
{
    public ProtectionRelay(Guid guid) : base(guid)
    {
        Functions = new UniqueCollection<Function>();
        RelayFunctions = new UniqueCollection<RelayFunction>();
        CompatibleComponents = new UniqueCollection<Manufactured>();
    }

    public UniqueCollection<Function> Functions { get; private set; }
    public UniqueCollection<RelayFunction> RelayFunctions { get; private set; }
    public double Voltage { get; private set; }
        
    /// <summary>
    /// Set value only via CoLa.BimEd.MV.Model.Services.CubicleService class.
    /// </summary>
    public CurrentTransformer CurrentTransformer { get; set; }
        
    public UniqueCollection<Manufactured> CompatibleComponents { get; private set; }
    public Manufactured GetDefault(Type type = null) =>
        CompatibleComponents.FirstOrDefault(n => n.GetType() == type) as CurrentTransformer;
    public bool IsCompatible(Manufactured component) =>
        CompatibleComponents.Contains(component);

    public ProtectionRelay Clone() => (ProtectionRelay) this.MemberwiseClone();
}