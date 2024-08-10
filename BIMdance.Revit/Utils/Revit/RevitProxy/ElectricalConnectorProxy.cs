namespace BIMdance.Revit.Utils.Revit.RevitProxy;

public class ElectricalConnectorProxy
{
    // public ElectricalSystemType ElectricalSystemType { get; set; }
    // public PowerFactorStateType PowerFactorStateType { get; set; }
    public int NumberOfPoles { get; set; }
    public string ConnectionDescription { get; set; }
    public double ApparentLoad { get; set; }
    public double ApparentLoad1 { get; set; }
    public double ApparentLoad2 { get; set; }
    public double ApparentLoad3 { get; set; }
    public double PowerFactor { get; set; }
    public double Voltage { get; set; }
    public bool Unity { get; set; }
}