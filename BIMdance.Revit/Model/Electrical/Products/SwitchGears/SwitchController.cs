namespace BIMdance.Revit.Model.Electrical.Products.SwitchGears;

public class SwitchController : Manufactured, ICompatibleComponents<Manufactured>, IPrototype<SwitchController>
{
    public SwitchController(Guid guid) : base(guid)
    {
        RelayFunctions = new UniqueCollection<RelayFunction>();
        CompatibleComponents = new UniqueCollection<Manufactured>();
    }

    public UniqueCollection<RelayFunction> RelayFunctions { get; protected set; }

    /// <summary>
    /// Set value only via CoLa.BimEd.MV.Model.Services.CubicleService class.
    /// </summary>
    public CurrentTransformer CurrentTransformer { get; set; }

    /// <summary>
    /// Set value only via CoLa.BimEd.MV.Model.Services.CubicleService class.
    /// </summary>
    public CurrentTransformerZeroSequence CurrentTransformerZeroSequence { get; internal set; }

    /// <summary>
    /// Set value only via CoLa.BimEd.MV.Model.Services.CubicleService class.
    /// </summary>
    public MeasuringVoltageTransformer VoltageTransformer { get; set; }

    public UniqueCollection<Manufactured> CompatibleComponents { get; protected set; }
    public Manufactured GetDefault(Type type = null) =>
        CompatibleComponents.FirstOrDefault(n => n.GetType() == type);
    public bool IsCompatible(Manufactured component) =>
        CompatibleComponents.Contains(component);
    public CurrentTransformer GetDefaultCurrentTransformer() => GetDefault(typeof(CurrentTransformer)) as CurrentTransformer;
    public CurrentTransformerZeroSequence GetDefaultCurrentTransformerZeroSequence() => GetDefault(typeof(CurrentTransformerZeroSequence)) as CurrentTransformerZeroSequence;
    public MeasuringVoltageTransformer GetDefaultVoltageTransformer() => GetDefault(typeof(MeasuringVoltageTransformer)) as MeasuringVoltageTransformer;
        
    public SwitchController Clone() => (SwitchController) this.MemberwiseClone();
}