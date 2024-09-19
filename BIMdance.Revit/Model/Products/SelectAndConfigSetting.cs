namespace BIMdance.Revit.Model.Products;

public class SelectAndConfigSetting
{
    public bool BreakNeutralCore { get; set; }
    public EquipmentSelectionMode EquipmentSelectionMode { get; set; }
    public bool NeutralCore { get; set; }
    public CableSeries CableSeries { get; set; }
    public double MinCoreSection { get; set; }
    public SelectorProductSettings SwitchSettings { get; set; }
    public SelectorProductSettings ContactorSettings { get; set; }
    public SelectorProductSettings EnergyMeterSettings { get; set; }
    public SelectorProductSettings OtherProductSettings { get; set; }
}

public class SelectAndConfigSetting<TElectrical> :
    SelectAndConfigSetting,
    IPrototype<SelectAndConfigSetting<TElectrical>>,
    IPropertyPrototype<SelectAndConfigSetting<TElectrical>>
    where TElectrical : ElectricalBase
{
    public static SelectAndConfigSetting<TElectrical> Create() =>
        Locator.Get<SelectAndConfigSetting<TElectrical>>();

    public SelectAndConfigSetting()
    {
        ContactorSettings = new SelectorProductSettings();
        EnergyMeterSettings = new SelectorProductSettings();
        SwitchSettings = new SelectorProductSettings();
        OtherProductSettings = new SelectorProductSettings();
    }

    public List<SelectorProductSettings> AllSetting
    {
        get
        {
            var allProperties = GetType().GetProperties();

            return allProperties
                .Where(n => n.PropertyType == typeof(SelectorProductSettings))
                .Select(n => n.GetValue(this) as SelectorProductSettings)
                .ToList();
        }
    }

    public SelectAndConfigSetting<TElectrical> Clone()
    {
        var clone = new SelectAndConfigSetting<TElectrical>();

        clone.PullProperties(this);

        return clone;
    }

    public void PullProperties(SelectAndConfigSetting<TElectrical> prototype)
    {
        if (prototype == null)
            return;

        BreakNeutralCore = prototype.BreakNeutralCore;
        CableSeries = prototype.CableSeries;
        EquipmentSelectionMode = prototype.EquipmentSelectionMode;
        MinCoreSection = prototype.MinCoreSection;
        NeutralCore = prototype.NeutralCore;

        ContactorSettings = prototype.ContactorSettings.Clone();
        EnergyMeterSettings = prototype.EnergyMeterSettings.Clone();
        SwitchSettings = prototype.SwitchSettings.Clone();
        OtherProductSettings = prototype.OtherProductSettings.Clone();
    }

    public List<SelectorProductSetting> GetAllSelectorSettings()
    {
        return SwitchSettings
            .Union(ContactorSettings)
            .Union(EnergyMeterSettings)
            .Union(OtherProductSettings)
            .ToList();
    }
}

public class SelectorProductSettings : List<SelectorProductSetting>, IPrototype<SelectorProductSettings>
{
    private bool _isCurrentChanging;

    public SelectorProductSettings()
    {

    }

    public SelectorProductSettings(IEnumerable<string> selectorIds)
    {
        foreach (var selectorId in selectorIds)
        {
            Add(new SelectorProductSetting(
                selectorId,
                new ProductRange(new List<BaseProduct>()),
                selectorMinCurrent: 0,
                selectorMaxCurrent: 0));
        }
    }

    private SelectorProductSettings(SelectorProductSettings prototype)
    {
        this.AddRange(prototype.Select(n => n.Clone()));
    }

    public SelectorProductSettings Clone()
    {
        return new SelectorProductSettings(this);
    }

    public new void Add(SelectorProductSetting added)
    {
        var previous = this.LastOrDefault(n => n.MaxCurrent < added.MaxCurrent);
        var next = this.FirstOrDefault(n => n.MaxCurrent > added.MaxCurrent);

        if (next?.MinCurrent == 0)
        {
            added.SetMinCurrent(0);
            next.SetMinCurrent(added.MaxCurrent);
        }
        else if (next != null && next.MinCurrent < added.MaxCurrent)
        {
            added.SetMinCurrent(previous?.MaxCurrent ?? 0);
            next.SetMinCurrent(added.MaxCurrent);
        }
        else
        {
            added.SetMinCurrent(previous?.MaxCurrent ?? 0);
            added.SetMaxCurrent(next?.MinCurrent ?? added.MaxCurrent);
        }

        var index = previous != null ? IndexOf(previous) + 1 : 0;
        Insert(index, added);
    }

    public new void Remove(SelectorProductSetting item)
    {
        GetNeighborSwitches(item, out var previous, out var next);

        next?.SetMinCurrent(previous?.MaxCurrent ?? 0);

        base.Remove(item);
    }

    public void SetMaxCurrent(SelectorProductSetting setting, double maxCurrent)
    {
        if (_isCurrentChanging)
            return;

        try
        {
            _isCurrentChanging = true;

            var nextSwitch = GetNextSwitch(setting);

            if (nextSwitch != null && maxCurrent >= nextSwitch.MaxCurrent)
            {
                maxCurrent = nextSwitch.MinCurrent;
            }

            setting.SetMaxCurrent(maxCurrent);

            nextSwitch?.SetMinCurrent(maxCurrent);
        }
        finally
        {
            _isCurrentChanging = false;
        }
    }

    public void SetMinCurrent(SelectorProductSetting setting, double minCurrent)
    {
        if (_isCurrentChanging)
            return;

        try
        {
            _isCurrentChanging = true;

            var previousSwitch = GetPreviousSwitch(setting);

            if (previousSwitch == null)
            {
                minCurrent = 0;
            }
            else if (minCurrent <= previousSwitch.MinCurrent)
            {
                minCurrent = previousSwitch.MaxCurrent;
            }
            else if (minCurrent > previousSwitch.SelectorMaxCurrent)
            {
                minCurrent = previousSwitch.SelectorMaxCurrent;
            }

            setting.SetMinCurrent(minCurrent);

            previousSwitch?.SetMaxCurrent(minCurrent);
        }
        finally
        {
            _isCurrentChanging = false;
        }
    }

    public double GetNewMinCurrentValue(SelectorProductSetting productSetting)
    {
        var previousProductSetting = GetPreviousSwitch(productSetting);

        if (previousProductSetting == null)
        {
            return 0;
        }

        if (productSetting.SelectorMinCurrent < previousProductSetting.MaxCurrent
            && previousProductSetting.MaxCurrent < productSetting.SelectorMaxCurrent)
        {
            return previousProductSetting.MaxCurrent;
        }

        return productSetting.SelectorMinCurrent;
    }

    private void GetNeighborSwitches(
        SelectorProductSetting setting,
        out SelectorProductSetting previous,
        out SelectorProductSetting next)
    {
        previous = GetPreviousSwitch(setting);
        next = GetNextSwitch(setting);
    }

    private SelectorProductSetting GetPreviousSwitch(SelectorProductSetting item)
    {
        var index = IndexOf(item);
        return index > 0 ? this[index - 1] : null;
    }

    private SelectorProductSetting GetNextSwitch(SelectorProductSetting item)
    {
        var index = IndexOf(item);
        return index > -1 && index < Count - 1 ? this[index + 1] : null;
    }
}