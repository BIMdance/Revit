namespace BIMdance.Revit.Model.RevitProxy;

public sealed class DemandFactorProxy : ElementProxy, IPrototype<DemandFactorProxy>, IPropertyPrototype<DemandFactorProxy>
{
    public DemandFactorProxy() => Values = new List<DemandFactorValueProxy> { new() };

    public DemandFactorProxy(
        int revitId,
        string name,
        DemandFactorRuleProxy ruleType = DemandFactorRuleProxy.Constant,
        params DemandFactorValueProxy[] values) :
        base(revitId, name)
    {
        AdditionalLoad = 0;
        RuleType = ruleType;
        Values = values.Any() ? values.ToList() : new List<DemandFactorValueProxy> { new() };
    }

    public DemandFactorProxy(
        int revitId,
        string name,
        double constantValue) :
        this(revitId, name, DemandFactorRuleProxy.Constant, new DemandFactorValueProxy(constantValue))
    { }

    private DemandFactorProxy(DemandFactorProxy prototype) => PullProperties(prototype);
    public DemandFactorProxy Clone() => new(this);
    public void PullProperties(DemandFactorProxy prototype)
    {
        this.Name = prototype.Name;
        this.RevitId = prototype.RevitId;
        this.AdditionalLoad = prototype.AdditionalLoad;
        this.IncludeAdditionalLoad = prototype.IncludeAdditionalLoad;
        this.RuleType = prototype.RuleType;
        this.Values = prototype.Values.Select(x => x.Clone()).ToList();
    }
    public double AdditionalLoad { get; set; }
    public bool IncludeAdditionalLoad { get; set; }
    public DemandFactorRuleProxy RuleType { get; set; }
    public List<DemandFactorValueProxy> Values { get; set; }
    
    public double GetValue(List<ElectricalElementProxy> elements)
    {
        try
        {
            return RuleType switch
            {
                DemandFactorRuleProxy.Constant => Values.ElementAt(0).Factor,
                DemandFactorRuleProxy.LoadTable => GetValueLoadTable(elements),
                DemandFactorRuleProxy.LoadTablePerPortion => GetValueLoadTable(elements),
                DemandFactorRuleProxy.QuantityTable => GetValueQuantityTable(elements),
                DemandFactorRuleProxy.QuantityTablePerPortion => GetValueQuantityTable(elements),
                _ => 1
            };
        }
        catch (Exception exception)
        {
            Logger.Error(exception);
            return 1;
        }
    }

    #region GetValue

    private double GetValueQuantityTable(ICollection<ElectricalElementProxy> elements)
    {
        var count = elements.Count;
        var demandFactorValue = Values.FirstOrDefault(n => n.MinRange < count && n.MaxRange >= count);

        if (demandFactorValue == null)
            return 1;

        var index = Values.IndexOf(demandFactorValue);

        if (index == 0 || index == Values.Count - 1)
            return demandFactorValue.Factor;
            
        var demandFactorValuePrevious = Values.ElementAt(index - 1);

        return (count - demandFactorValue.MinRange)
               * (demandFactorValue.Factor - demandFactorValuePrevious.Factor)
               / (demandFactorValue.MaxRange - demandFactorValue.MinRange)
               + demandFactorValuePrevious.Factor;
            
    }

    private double GetValueLoadTable(IEnumerable<ElectricalElementProxy> elements)
    {
        var load = elements.Sum(n => n.PowerParameters.OwnApparentLoad * n.PowerParameters.OwnPowerFactor);
        var demandFactorValue = Values.FirstOrDefault(n => n.MinRange < load && n.MaxRange >= load);

        if (demandFactorValue == null)
            return 1;

        var index = Values.IndexOf(demandFactorValue);

        if (index == 0 || index == Values.Count - 1)
            return demandFactorValue.Factor;
            
        var demandFactorValuePrevious = Values.ElementAt(index - 1);

        return (load - demandFactorValue.MinRange)
               * (demandFactorValue.Factor - demandFactorValuePrevious.Factor)
               / (demandFactorValue.MaxRange - demandFactorValue.MinRange)
               + demandFactorValuePrevious.Factor;
    }

    #endregion

    public override string ToString() => $"[{RevitId}] {Name} - {nameof(RuleType)}: {RuleType}";

    public override bool Equals(object obj)
    {
        if (obj is not DemandFactorProxy electricalDemandFactor)
            return false;

        return this.RevitId == electricalDemandFactor.RevitId;
    }

    public override int GetHashCode() => RevitId.GetHashCode();
}