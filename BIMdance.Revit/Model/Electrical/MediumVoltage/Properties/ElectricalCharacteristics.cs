namespace BIMdance.Revit.Model.Electrical.MediumVoltage.Properties;

public class ElectricalCharacteristics : IPrototype<ElectricalCharacteristics>
{
    public static readonly double DefaultOperatingVoltage = ElectricalConstants.Voltage.V_230;
    public int BusbarCurrent { get; set; }
    public int Frequency { get; set; }
    public SwitchGearVoltage SwitchGearVoltage { get; set; }
    public int DynamicCurrent { get; set; }
    public int RatedCurrent { get; set; }
    public CurrentTimePoint ThermalCurrent { get; set; }
    public ElectricalQuantity OperatingVoltage { get; set; }
    public InsulationType? ArcInsulationType { get; set; }
    public InsulationType? CubicleInsulation { get; set; }

    public ElectricalCharacteristics Clone() => new()
    {
        ArcInsulationType = this.ArcInsulationType,
        BusbarCurrent = this.BusbarCurrent,
        CubicleInsulation = this.CubicleInsulation,
        SwitchGearVoltage = this.SwitchGearVoltage,
        DynamicCurrent = this.DynamicCurrent,
        Frequency = this.Frequency,
        OperatingVoltage = this.OperatingVoltage,
        RatedCurrent = this.RatedCurrent,
        ThermalCurrent = this.ThermalCurrent,
    };
}