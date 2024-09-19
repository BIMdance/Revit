namespace BIMdance.Revit.Model.Electrical.Transformers;

public class TransformerProduct : IPrototype<TransformerProduct>, IPropertyPrototype<TransformerProduct>
{
    private double _ratedPower;
    private double _primaryVoltage;
    private double _secondaryVoltage;
        
    public TransformerProduct() { }

    public TransformerProduct(
        double power, double primaryVoltage, double secondaryVoltage, double insulationVoltage,
        double noLoadLosses, double loadLosses75, double loadLosses120, double impedanceVoltage,
        List<ProtectionIndex> protectionIndices, double length, double width, double height, double weight,
        double acousticPower, double acousticPressure) 
    {
        RatedPower = power;
        PrimaryVoltage = primaryVoltage;
        SecondaryVoltage = secondaryVoltage;
        InsulationVoltage = insulationVoltage;
        NoLoadLosses = noLoadLosses;
        LoadLosses75 = loadLosses75;
        LoadLosses120 = loadLosses120;
        ImpedanceVoltage = impedanceVoltage;
        ProtectionIndices = protectionIndices;
        Product = new Product
        {
            Dimensions = Dimensions.CreateFromMillimeters(height, length, width),
            Weight = weight,
        };
        Accessories = new List<Product>();
        AcousticPower = acousticPower;
        AcousticPressure = acousticPressure;

        CalculateResistance();
    }
        
    private TransformerProduct(TransformerProduct prototype) => PullProperties(prototype);

    public void PullProperties(TransformerProduct prototype)
    {
        this.PullInstanceProperties(prototype);
        this.PullProductProperties(prototype);
    }

    public void PullInstanceProperties(TransformerProduct prototype)
    {
        this.Accessories = prototype.Accessories?.ToList() ?? new List<Product>();
        this.WindingMaterial = prototype.WindingMaterial;
        this.CoolingMode = prototype.CoolingMode;
        this.ProtectionIndex = prototype.ProtectionIndex;
        this.VectorGroup = prototype.VectorGroup;
        this.Properties = prototype.Properties;
    }

    public void PullProductProperties(TransformerProduct prototype)
    {
        this.TransformerSeries = prototype.TransformerSeries;
        this.Product = prototype.Product;
        this.ProtectionIndices = prototype.ProtectionIndices;
            
        this.RatedPower = prototype.RatedPower;
        this.InsulationVoltage = prototype.InsulationVoltage;
        this.PrimaryVoltage = prototype.PrimaryVoltage;
        this.SecondaryVoltage = prototype.SecondaryVoltage;
            
        this.NoLoadLosses = prototype.NoLoadLosses;
        this.LoadLosses75 = prototype.LoadLosses75;
        this.LoadLosses120 = prototype.LoadLosses120;
        this.ImpedanceVoltage = prototype.ImpedanceVoltage;
            
        this.Resistance = prototype.Resistance;
        this.InductiveReactance = prototype.InductiveReactance;
            
        this.AcousticPower = prototype.AcousticPower;
        this.AcousticPressure = prototype.AcousticPressure;
    }
        
    public TransformerProduct Clone() => new(prototype: this);

    public TransformerSeries TransformerSeries { get; set; }
    public Product Product { get; set; }
    public List<Product> Accessories { get; set; }
    public Material WindingMaterial { get; set; }
    public CoolingMode CoolingMode { get; set; }
    public ProtectionIndex ProtectionIndex { get; set; }
    public List<ProtectionIndex> ProtectionIndices { get; set; }
    public VectorGroup VectorGroup { get; set; }
    public List<TransformerProperty> Properties { get; set; } = new();
    public double AcousticPower { get; set; }
    public double AcousticPressure { get; set; }

    public double RatedPower
    {
        get => _ratedPower;
        set
        {
            _ratedPower = value;
                
            PrimaryCurrent = RatedPower / PrimaryVoltage / MathConstants.Sqrt3;
            SecondaryCurrent = RatedPower / SecondaryVoltage / MathConstants.Sqrt3;
        }
    }

    public double InsulationVoltage { get; set; }

    public double PrimaryVoltage
    {
        get => _primaryVoltage;
        set
        {
            _primaryVoltage = value;
            PrimaryCurrent = RatedPower / PrimaryVoltage / MathConstants.Sqrt3;
        }
    }

    public double SecondaryVoltage
    {
        get => _secondaryVoltage;
        set
        {
            _secondaryVoltage = value;
            SecondaryCurrent = RatedPower / SecondaryVoltage / MathConstants.Sqrt3;
        }
    }

    public double PrimaryCurrent { get; set; }
    public double SecondaryCurrent { get; set; }

    public double NoLoadLosses { get; set; }
    public double LoadLosses75 { get; set; }
    public double LoadLosses120 { get; set; }
    public double ImpedanceVoltage { get; set; }

    public double Resistance { get; set; }
    public double InductiveReactance { get; set; }

    public void CalculateResistance()
    {
        if (LoadLosses120.IsEqualToZero() ||
            ImpedanceVoltage.IsEqualToZero() ||
            RatedPower.IsEqualToZero() ||
            SecondaryVoltage.IsEqualToZero())
            return;

        Resistance = 
            LoadLosses120
            * Math.Pow(SecondaryVoltage, 2)
            / Math.Pow(RatedPower, 2);

        InductiveReactance = 
            Math.Sqrt(Math.Pow(ImpedanceVoltage, 2) - Math.Pow(100 * LoadLosses120 / RatedPower, 2))
            * Math.Pow(SecondaryVoltage, 2)
            / RatedPower
            * 1e-2;

        PrimaryCurrent = RatedPower / PrimaryVoltage / MathConstants.Sqrt3;
        SecondaryCurrent = RatedPower / SecondaryVoltage / MathConstants.Sqrt3;
    }

    public void Reset()
    {
        this.TransformerSeries = default;
        this.Product = default;
        this.Accessories = new List<Product>();
        this.WindingMaterial = default;
        this.CoolingMode = default;
        this.ProtectionIndex = default;
        this.VectorGroup = default;
        this.Properties = new List<TransformerProperty>();
            
        this.RatedPower = default;
        this.InsulationVoltage = default;
        this.PrimaryVoltage = default;
        this.SecondaryVoltage = default;

        this.NoLoadLosses = default;
        this.LoadLosses75 = default;
        this.LoadLosses120 = default;
        this.ImpedanceVoltage = default;

        this.Resistance = default;
        this.InductiveReactance = default;

        this.AcousticPower = default;
        this.AcousticPressure = default;
    }
}