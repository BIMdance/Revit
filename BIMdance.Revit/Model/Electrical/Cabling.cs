namespace BIMdance.Revit.Model.Electrical;

public class Cabling : IPrototype<Cabling>, IPropertyPrototype<Cabling>
{
    public CablingEnvironment CablingEnvironment { get; set; }
    public double CableLengthInCableTray { get; set; }
    public double CableLengthOutsideCableTray { get; set; }
    public string CablingDescription { get; set; }

    public Cabling() { }
    private Cabling(Cabling prototype) => this.PullProperties(prototype);
    public Cabling Clone() => new(prototype: this);
    public void PullProperties(Cabling prototype)
    {
        this.CablingEnvironment = prototype.CablingEnvironment;
        this.CableLengthInCableTray = prototype.CableLengthInCableTray;
        this.CableLengthOutsideCableTray = prototype.CableLengthOutsideCableTray;
        this.CablingDescription = prototype.CablingDescription;
    }
}