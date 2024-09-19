using BIMdance.Revit.Model.Electrical.MediumVoltage.Properties;
using BIMdance.Revit.Model.Electrical.Products.SwitchGears;

namespace BIMdance.Revit.Model.Electrical.MediumVoltage;

public class SwitchGearFunction : EquipmentUnit, IPrototype<SwitchGearFunction>
{
    public SwitchGearFunction(Guid switchGearSeriesGuid, Guid guid, Product product) :
        base(product.Name)
    {
        SwitchGearSeriesGuid = switchGearSeriesGuid;
        Guid = guid;
        Product = product;
        LeftConnector = new LeftConnector<EquipmentUnit>();
        RightConnector = new RightConnector<EquipmentUnit>();
    }

    public Guid Guid { get; }
    public Guid SwitchGearSeriesGuid { get; }
    public SwitchGearUnit SwitchGearUnit { get; set; }
    public Product Product { get; set; }
    public ElectricalCharacteristics ElectricalCharacteristics { get; set; } = new();
    public ElectricalCharacteristicsSource ElectricalCharacteristicsSource { get; } = new();
    public SwitchMountType SwitchMountType { get; set; }
    public SwitchGearOptions AvailableOptions { get; set; }
    public SwitchGearOptions SelectedOptions { get; set; } = new();
    public SwitchGearComponentCollections CompatibleComponents { get; set; }
    public SwitchGearComponents Components { get; set; } = new();
    public bool IsOnlyInternalConnections { get; set; }
    public SwitchGearFunction Clone()
    {
        return new SwitchGearFunction(SwitchGearSeriesGuid, Guid, Product)
        {
            AvailableOptions = this.AvailableOptions,
            CompatibleComponents = this.CompatibleComponents,
            Components = this.Components.Clone(),
            Functions = this.Functions,
            IsOnlyInternalConnections = this.IsOnlyInternalConnections,
            SwitchMountType = this.SwitchMountType,
            SelectedOptions = this.SelectedOptions.Clone(),
        };
    }
}