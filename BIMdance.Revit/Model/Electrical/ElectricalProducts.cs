namespace BIMdance.Revit.Model.Electrical;

public class ElectricalProducts : ElementProxy, IPropertyPrototype<ElectricalProducts>
{
    public ElectricalProducts()
    {
        Products = new List<SelectorProduct>();
    }

    public void PullProperties(ElectricalProducts prototype)
    {
        this.MainSwitch = prototype.MainSwitch;
        this.Products = prototype.Products.ToList();
    }

    public SelectorProduct MainSwitch { get; set; }
    public List<SelectorProduct> Products { get; set; }
}

public class CircuitProducts : ElectricalProducts, IPropertyPrototype<CircuitProducts>
{
    private readonly ElectricalSystemProxy _electricalSystem;

    public CircuitProducts() { }

    public CircuitProducts(ElectricalSystemProxy electricalSystem)
    {
        _electricalSystem = electricalSystem;
    }

    public Cable Cable { get; private set; }
    public int CablesCount { get; private set; } = 1;
    public int EstimateCablesCount { get; private set; } = 1;

    public void PullProperties(CircuitProducts prototype)
    {
        base.PullProperties(prototype);

        this.Cable = !string.IsNullOrWhiteSpace(prototype.Cable?.Id) ? prototype.Cable : null;
        this.CablesCount = prototype.CablesCount;
        this.EstimateCablesCount = prototype.EstimateCablesCount;
    }

    public void SetCable(Cable cable)
    {
        Cable = cable;
        _electricalSystem?.CircuitPowerParameters?.UpdatePermissibleCurrent();
    }

    public void SetCablesCount(int cablesCount)
    {
        CablesCount = cablesCount;

        if (Cable == null)
            return;

        var phasesNumber = _electricalSystem.PowerParameters.PhasesNumber;
        var conductorsCount = Cable.Conductors.Sum(n => n.Count);

        EstimateCablesCount = conductorsCount == 1 && CablesCount > (int)phasesNumber
            ? 1
            : CablesCount;
    }
}

public class SwitchBoardUnitProducts : ElectricalProducts
{
    public List<SelectorProduct> Enclosures { get; set; } = new();
    public SelectorProduct ReserveSwitch { get; set; }
    public SelectorProduct SectionSwitch { get; set; }
    public SelectorProduct EnergyMeter { get; set; }
}