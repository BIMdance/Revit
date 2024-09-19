namespace BIMdance.Revit.Model.Electrical.Products;

public class CableInsulation
{
    public bool FireResistant { get; set; }
    public bool LowSmoke { get; set; }
    public bool LowToxic { get; set; }
    public bool ZeroHalogen { get; set; }
    public override string ToString() => this.AllValuesToString();
}