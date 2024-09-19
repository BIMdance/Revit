namespace BIMdance.Revit.Model.RevitProxy;

public sealed class LoadClassificationProxy :
    ElementProxy,
    IPrototype<LoadClassificationProxy>,
    IPropertyPrototype<LoadClassificationProxy>
{
    private SelectAndConfigSetting<ElectricalSystemProxy> _circuitSelectAndConfigSetting;
    private SelectAndConfigSetting<SwitchBoardUnit> _panelSelectAndConfigSetting;
    
    public LoadClassificationProxy() { }

    public LoadClassificationProxy(int revitId, string name, DemandFactorProxy demandFactor = null) :
        base(revitId, name) => DemandFactor = demandFactor;

    public LoadClassificationProxy(LoadClassificationProxy prototype) => PullProperties(prototype);
    public LoadClassificationProxy Clone() => new(this);
    public void PullProperties(LoadClassificationProxy prototype)
    {
        this.Name = prototype.Name;
        this.RevitId = prototype.RevitId;
        this.DemandFactor = prototype.DemandFactor;
        this._circuitSelectAndConfigSetting = prototype._circuitSelectAndConfigSetting?.Clone();
        this._panelSelectAndConfigSetting = prototype._panelSelectAndConfigSetting?.Clone();
    }

    public DemandFactorProxy DemandFactor { get; set; }
    public SelectAndConfigSetting<ElectricalSystemProxy> CircuitSelectAndConfigSetting => _circuitSelectAndConfigSetting ??= SelectAndConfigSetting<ElectricalSystemProxy>.Create();
    public SelectAndConfigSetting<SwitchBoardUnit> PanelSelectAndConfigSetting => _panelSelectAndConfigSetting ??= SelectAndConfigSetting<SwitchBoardUnit>.Create();
    public string ServiceType { get; set; }

    public override string ToString() => $"[{RevitId}] {Name} - {nameof(DemandFactor)}: {DemandFactor}";
}