

// ReSharper disable VirtualMemberCallInConstructor

namespace BIMdance.Revit.Model.Electrical.Base;

public abstract class ElectricalSource :
    ElectricalEquipmentProxy,
    IPrototype<ElectricalSource>,
    IPropertyPrototype<ElectricalSource>
{
    protected ElectricalSource()
    {
        SetBaseConnector(new ConnectorProxy(this, 1, ElectricalSystemTypeProxy.UndefinedSystemType));
    }
        
    protected ElectricalSource(
        int revitId,
        string name,
        ElectricalSystemTypeProxy electricalSystemType) : this()
    {
        RevitId = revitId;
        Name = name;
        SetBaseConnector(new ConnectorProxy(this, 1, electricalSystemType));
    }
        
    protected ElectricalSource(
        int revitId,
        string name,
        DistributionSystemProxy distributionSystem) : this()
    {
        RevitId = revitId;
        Name = name;
        DistributionSystem = distributionSystem;
        SetBaseConnector(new ConnectorProxy(this, 1, distributionSystem));
    }
        
    public void PullProperties(ElectricalSource prototype)
    {
        this.Name = prototype.Name;
        this.DistributionSystem = prototype.DistributionSystem;
        this.BaseConnector.PowerParameters = prototype.BaseConnector.PowerParameters;
    }

    public ElectricalSource Clone()
    {
        return this.MemberwiseClone() as ElectricalSource;
    }
}