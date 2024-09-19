namespace BIMdance.Revit.Model.RevitProxy;

public sealed class DemandFactorValueProxy : IPrototype<DemandFactorValueProxy>
{
    private const double MinValidDemandFactorInRevit = 0.0001;
    private double _factor;
    private double _minRange;
    private double _maxRange;
        
    public DemandFactorValueProxy(double factor = 1, double minRange = 0, double maxRange = double.PositiveInfinity)
    {
        Factor = factor;

        // не изменять порядок присвоения значений
        MaxRange = maxRange;
        MinRange = minRange;
    }

    public DemandFactorValueProxy Clone() => new(Factor, MinRange, MaxRange);

    public double Factor
    {
        get => _factor;
        set => _factor = value < MinValidDemandFactorInRevit
            ? MinValidDemandFactorInRevit
            : value;
    }

    public double MinRange
    {
        get => _minRange;
        set => _minRange = value >= _maxRange
            ? _maxRange - 1
            : value < 0 ? 0 : value;
    }

    public double MaxRange
    {
        get => _maxRange;
        set => _maxRange = value <= _minRange
            ? _minRange + 1
            : value;
    }

    public void ConvertRangeFromInternal(DemandFactorRuleProxy demandFactorRule)
    {
        switch (demandFactorRule)
        {
            case DemandFactorRuleProxy.LoadTable:
            case DemandFactorRuleProxy.LoadTablePerPortion:
                MinRange = MinRange.Convert(Converting.VoltAmperesFromInternal);
                MaxRange = MaxRange.Convert(Converting.VoltAmperesFromInternal);
                return;
        }
    }
    
    public void GetInternalRange(DemandFactorRuleProxy demandFactorRule, out double minRange, out double maxRange)
    {
        switch (demandFactorRule)
        {
            case DemandFactorRuleProxy.LoadTable:
            case DemandFactorRuleProxy.LoadTablePerPortion:
                minRange = MinRange.Convert(Converting.VoltAmperesToInternal);
                maxRange = MaxRange.Convert(Converting.VoltAmperesToInternal);
                return;
            
            case DemandFactorRuleProxy.Constant:
            case DemandFactorRuleProxy.QuantityTable:
            case DemandFactorRuleProxy.QuantityTablePerPortion:
                minRange = MinRange;
                maxRange = MaxRange;
                return;

            default:
                throw new ArgumentOutOfRangeException(nameof(demandFactorRule), demandFactorRule, null);
        }
    }
}