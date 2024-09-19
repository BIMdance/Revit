namespace BIMdance.Revit.Model.Electrical;

public class ThreePhasesParameters
{
    public double Asymmetry { get; set; }

    public double ApparentLoad1 { get; set; }
    public double ApparentLoad2 { get; set; }
    public double ApparentLoad3 { get; set; }

    public double TrueLoad1 { get; set; }
    public double TrueLoad2 { get; set; }
    public double TrueLoad3 { get; set; }

    public double EstimateTrueLoad1 { get; set; }
    public double EstimateTrueLoad2 { get; set; }
    public double EstimateTrueLoad3 { get; set; }

    public double EstimateCurrent1 { get; set; }
    public double EstimateCurrent2 { get; set; }
    public double EstimateCurrent3 { get; set; }

    public Dictionary<LoadClassificationProxy, double> LoadClassificationTrueEstimate1 { get; } = new();
    public Dictionary<LoadClassificationProxy, double> LoadClassificationTrueEstimate2 { get; } = new();
    public Dictionary<LoadClassificationProxy, double> LoadClassificationTrueEstimate3 { get; } = new();
}