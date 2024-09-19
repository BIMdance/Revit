namespace BIMdance.Revit.Model.Electrical.Transformers;

public class TransformerSeries
{
    public TransformerSeries() { }
        
    public TransformerSeries(
        Guid guid,
        string name,
        TransformerNoLoadLossesLevel noLoadLossesLevel,
        TransformerLoadLossesLevel loadLossesLevel)
    {
        Guid = guid;
        Name = name;
        NoLoadLossesLevel = noLoadLossesLevel;
        LoadLossesLevel = loadLossesLevel;
    }

    public Guid Guid { get; set; }
    public string Name { get; }
    public TransformerNoLoadLossesLevel NoLoadLossesLevel { get; }
    public TransformerLoadLossesLevel LoadLossesLevel { get; }
    public List<string> Countries { get; set; }
    public List<TransformerProduct> Products { get; set; }
    public IEnumerable<double> InsulationVoltages => Products.Select(n => n.InsulationVoltage).Distinct(new DoubleEqualityComparer());
    public IEnumerable<double> RatedPowers => Products.Select(n => n.RatedPower).Distinct(new DoubleEqualityComparer());
    public IEnumerable<double> PrimaryVoltages => Products.Select(n => n.PrimaryVoltage).Distinct(new DoubleEqualityComparer());
    public IEnumerable<double> SecondaryVoltages => Products.Select(n => n.SecondaryVoltage).Distinct(new DoubleEqualityComparer());
    public IEnumerable<Material> WindingMaterials { get; set; }
    public List<ProtectionIndex> ProtectionIndices { get; set; }
    public List<VectorGroup> VectorGroups { get; set; }
    public List<CoolingMode> CoolingModes { get; set; }
    public List<TransformerProperty> Properties { get; set; }

    public TransformerProduct GetProduct(
        double ratedPower,
        double primaryVoltage,
        double secondaryVoltage,
        ProtectionIndex protectionIndex)
    {
        var product = Products.FirstOrDefault(n =>
            n.RatedPower.IsEqualTo(ratedPower) &&
            n.PrimaryVoltage.IsEqualTo(primaryVoltage) &&
            n.SecondaryVoltage.IsEqualTo(secondaryVoltage) &&
            n.ProtectionIndices.Contains(protectionIndex));

        if (product != null)
            product.TransformerSeries = this;

        return product;
    }
        
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType() && Equals((TransformerSeries)obj);
    }
    private bool Equals(TransformerSeries other) => Guid.Equals(other.Guid);
    public override int GetHashCode() => Guid.GetHashCode();
}

// public class TransformerSeries : Manufactured
// {
//     public TransformerSeries(string name) : base(name: name) { }
//
//     [HelpKeyword(nameof(Transformer.RatedPower))]
//     public List<PowerApparent> RatedPowers { get; set; }
//
//     public List<Voltage> InsulationVoltages { get; set; }
//
//     [HelpKeyword(nameof(Transformer.PrimaryVoltage))]
//     public List<Voltage> PrimaryVoltages { get; set; }
//
//     [HelpKeyword(nameof(Transformer.SecondaryVoltage))]
//     public List<Voltage> SecondaryVoltages { get; set; }
//
//     [HelpKeyword(nameof(Transformer.WindingMaterial))]
//     public List<Material> WindingMaterials { get; set; }
//
//     [HelpKeyword(nameof(Transformer.ProtectionIndex))]
//     public List<ProtectionIndex> ProtectionIndices { get; set; }
//     
//     [HelpKeyword(nameof(Transformer.VectorGroup))]
//     public List<VectorGroup> VectorGroups { get; set; }
//
//     [HelpKeyword(nameof(Transformer.CoolingMode))]
//     public List<CoolingMode> CoolingModes { get; set; }
//
//     [HelpKeyword(nameof(Transformer.Properties))]
//     public List<TransformerProperty> Properties { get; set; }
// }