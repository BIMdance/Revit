namespace BIMdance.Revit.Model.Electrical.MediumVoltage.Properties;

public class ElectricalCharacteristicsSource
{
    public HashSet<SwitchGearVoltage> CubicleVoltages { get; } = new();
    public HashSet<int> Frequencies { get; } = new();
    public BindingValues<int, SwitchGearVoltage> BusbarCurrentVoltageBinding { get; } = new();
    public BindingValues<int, SwitchGearVoltage> RatedCurrentVoltageBinding { get; } = new();
    public BindingValues<SwitchGearVoltage, CurrentTimePoint> ThermalCurrentVoltageBinding { get; } = new();
    public BindingValues<int, CurrentTimePoint> ThermalCurrentBusbarCurrentBinding { get; } = new();
    public BindingValues<int, CurrentTimePoint> ThermalCurrentDynamicCurrentBinding { get; } = new();
    public HashSet<ElectricalQuantity> OperatingVoltages { get; } = new();
    public HashSet<InsulationType> ArcInsulations { get; } = new();
    public HashSet<InsulationType> CubicleInsulations { get; } = new();

    public void Merge(ElectricalCharacteristicsSource other)
    {
        RatedCurrentVoltageBinding.Merge(other.RatedCurrentVoltageBinding);
        BusbarCurrentVoltageBinding.Merge(other.BusbarCurrentVoltageBinding);
        ThermalCurrentVoltageBinding.Merge(other.ThermalCurrentVoltageBinding);
        ThermalCurrentBusbarCurrentBinding.Merge(other.ThermalCurrentBusbarCurrentBinding);
        ThermalCurrentDynamicCurrentBinding.Merge(other.ThermalCurrentDynamicCurrentBinding);
        CubicleVoltages.UnionWith(other.CubicleVoltages);
        OperatingVoltages.UnionWith(other.OperatingVoltages);
        Frequencies.UnionWith(other.Frequencies);
        ArcInsulations.UnionWith(other.ArcInsulations);
        CubicleInsulations.UnionWith(other.CubicleInsulations);
    }

    public bool Any() =>
        BusbarCurrentVoltageBinding.Any() ||
        CubicleVoltages.Any() ||
        Frequencies.Any() ||
        RatedCurrentVoltageBinding.Any() ||
        ThermalCurrentVoltageBinding.Any() ||
        ThermalCurrentBusbarCurrentBinding.Any() ||
        ThermalCurrentDynamicCurrentBinding.Any() ||
        ArcInsulations.Any() ||
        CubicleInsulations.Any();
}