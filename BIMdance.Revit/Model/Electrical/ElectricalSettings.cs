namespace BIMdance.Revit.Model.Electrical;

public class ElectricalSettings
{
    public ElectricalSettings()
    {
        CableReservePercent = 0;
        CircuitBreakerDeviation = 0.2;
        RebalanceLoadDeviation = 0.15;
        DesignTemperature = 65;
        VoltageDrop = 0.08;
    }

    public long CurrentOperatingMode { get; set; }
    public double CableReservePercent { get; set; }
    public double CircuitBreakerDeviation { get; set; }
    public double RebalanceLoadDeviation { get; set; }
    public double DesignTemperature { get; set; }
    public double VoltageDrop { get; set; }
}