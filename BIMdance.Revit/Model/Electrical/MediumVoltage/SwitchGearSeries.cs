using BIMdance.Revit.Model.Electrical.MediumVoltage.Properties;

namespace BIMdance.Revit.Model.Electrical.MediumVoltage;

public class SwitchGearSeries : Product
{
    public SwitchGearSeries() { }
    public SwitchGearSeries(Guid guid, string name) : base(guid, name) { }
    
    /// <summary>
    /// If true, enclosures of switchgear can include multiple functions, for example RM6 series.
    /// </summary>
    public bool IsBlockSwitchGear { get; set; }

    public SwitchGearFunction[] SwitchGearFunctions { get; set; } = Array.Empty<SwitchGearFunction>();
    public SwitchGearUnit[] SwitchGearUnits { get; set; } = Array.Empty<SwitchGearUnit>();
    public List<GasExhaust> GasExhausts { get; } = new();
    public List<InternalArcClassification> InternalArcClassifications { get; } = new();
    public List<NeutralSystem> NeutralSystems { get; } = new();
    public List<ProtectionIndex> ExternalFaceProtectionIndexes { get; } = new();
    public List<ProtectionIndex> BetweenCompartmentsProtectionIndexes { get; } = new();
    public List<ProtectionIndex> HighVoltagePartsProtectionIndexes { get; } = new();
    public ElectricalCharacteristicsSource ElectricalCharacteristicsSource { get; } = new();
    public List<SwitchMountType> SwitchMountTypes { get; } = new();
    public List<double> DynamicShortCurrentRange { get; } = new();
    public List<CurrentTimePoint> ThermalDurabilityRange { get; } = new();
    public List<CurrentTimePoint> InternalArcResistance { get; } = new();
    public bool DoubleBuses { get; internal set; } // Сдвоенные шины
    public List<int> EarthquakeResistancePoints { get;  } = new(); //{9} if count ==0 - not resist

    public SwitchGearSeries InitializeElectricalCharacteristics()
    {
        foreach (var switchGearFunction in SwitchGearFunctions)
            ElectricalCharacteristicsSource.Merge(switchGearFunction.ElectricalCharacteristicsSource);

        return this;
    }
    
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType() && Equals((SwitchGearSeries)obj);
    }
    
    private bool Equals(SwitchGearSeries other) => Guid.Equals(other.Guid);

    public override int GetHashCode() => Guid.GetHashCode();
}