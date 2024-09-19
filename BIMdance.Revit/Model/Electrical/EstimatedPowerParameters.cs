namespace BIMdance.Revit.Model.Electrical;

public class EstimatedPowerParameters
{
    public EstimatedPowerParameters() { }
    
    public EstimatedPowerParameters(PhasesNumber phasesNumber, double lineToGroundVoltage)
    {
        LineToGroundVoltage = lineToGroundVoltage;
        ThreePhases = phasesNumber > PhasesNumber.One ? new ThreePhasesParameters() : null;
        ResistanceReactance = new ResistanceReactance();
    }

    public bool IsSupplied { get; set; }
    public ThreePhasesParameters ThreePhases { get; }
    public double Current { get; set; }
    public double ShortCurrent1 { get; set; }
    public double ShortCurrent3 { get; set; }
    public double SurgeShortCurrent3 { get; set; }
    public double TrueLoad { get; set; }
    public double EstimateTrueLoad { get; set; }
    public double ApparentLoad { get; set; }
    public double EstimateApparentLoad { get; set; }
    public double PowerFactor { get; set; } = 1;
    public ResistanceReactance ResistanceReactance { get; set; }
    public double LineToGroundVoltage { get; }
    public double VoltageDrop { get; set; }
    public double VoltageDropPercent => VoltageDrop / LineToGroundVoltage;
    public double DemandFactor { get; set; }
    public double AdditionalDemandFactor { get; set; } = 1;
    public double TotalDemandFactor { get; set; } = 1;
}