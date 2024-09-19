namespace BIMdance.Revit.Model.Products;

public class SelectorProductSetting : IPrototype<SelectorProductSetting>
{
    public string SelectorId { get; set; }
    public double SelectorMinCurrent { get; }
    public double SelectorMaxCurrent { get; }
    public double MinCurrent { get; private set; }
    public double MaxCurrent { get; private set; }
    public ProductRange ProductRange { get; set; }
    public string SerializedConfiguration { get; set; }
    public List<string> AccessoryReferences { get; set; }
    public bool IsActualAccessories { get; set; }

    public SelectorProductSetting() { }

    public SelectorProductSetting(string selectorId, ProductRange productRange, double selectorMinCurrent, double selectorMaxCurrent)
    {
        SelectorId = selectorId;
        ProductRange = productRange.Clone();
        SelectorMinCurrent = selectorMinCurrent;
        SelectorMaxCurrent = selectorMaxCurrent;
        MinCurrent = SelectorMinCurrent;
        MaxCurrent = SelectorMaxCurrent;
        SerializedConfiguration = "";
        AccessoryReferences = new List<string>();
    }

    private SelectorProductSetting(SelectorProductSetting prototype) : this(
        prototype.SelectorId, prototype.ProductRange, prototype.SelectorMinCurrent, prototype.SelectorMaxCurrent)
    {
        MinCurrent = prototype.MinCurrent;
        MaxCurrent = prototype.MaxCurrent;
    }

    public SelectorProductSetting Clone()
    {
        return new SelectorProductSetting(this);
    }

    public void SetMaxCurrent(double maxCurrent)
    {
        if (maxCurrent < MinCurrent)
            return;

        MaxCurrent = maxCurrent < SelectorMaxCurrent ? maxCurrent : SelectorMaxCurrent;
    }

    public void SetMinCurrent(double minCurrent)
    {
        if (minCurrent > MaxCurrent)
            return;

        MinCurrent = minCurrent > 0 ? minCurrent : 0;
    }
}