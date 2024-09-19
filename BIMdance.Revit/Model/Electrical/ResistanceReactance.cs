namespace BIMdance.Revit.Model.Electrical;

public class ResistanceReactance
{
    public double InternalR0 { get; set; }
    public double InternalR1 { get; set; }
    public double InternalX0 { get; set; }
    public double InternalX1 { get; set; }

    public double TotalR0 { get; set; }
    public double TotalR1 { get; set; }
    public double TotalX0 { get; set; }
    public double TotalX1 { get; set; }

    public override string ToString() =>
        $"R1 = {InternalR1.Round(4)} / {TotalR1.Round(4)}; " +
        $"R0 = {InternalR0.Round(4)} / {TotalR0.Round(4)}; " +
        $"X1 = {InternalX1.Round(4)} / {TotalX1.Round(4)}; " +
        $"X0 = {InternalX0.Round(4)} / {TotalX0.Round(4)}; ";
}