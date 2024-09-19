namespace BIMdance.Revit.Model.Electrical.Factories;

public class ElectricalFactory
{
    public ElectricalFactory(ElectricalContext electricalContext)
    {
        ElectricalContext = electricalContext;
        ElectricalContext.ElectricalFactory = this;
        ElectricalGenerator   = new ElectricalGeneratorFactory(ElectricalContext);
        ElectricalElement     = new ElectricalElementFactory(ElectricalContext);
        ElectricalNetwork     = new ElectricalNetworkFactory(ElectricalContext);
        ElectricalSystem      = new ElectricalSystemFactory(ElectricalContext);
        ElectricalSystemGroup = new ElectricalSystemGroupFactory(ElectricalContext);
        OperatingMode         = new OperatingModeFactory(ElectricalContext);
        SwitchBoard           = new SwitchBoardFactory(ElectricalContext, ElectricalSystem);
        Transformer           = new TransformerFactory(ElectricalContext);
    }

    public ElectricalContext ElectricalContext { get; }
    public ElectricalGeneratorFactory ElectricalGenerator { get; }
    public ElectricalElementFactory ElectricalElement { get; }
    public ElectricalNetworkFactory ElectricalNetwork { get; }
    public ElectricalSystemFactory ElectricalSystem { get; }
    public ElectricalSystemGroupFactory ElectricalSystemGroup { get; }
    public OperatingModeFactory OperatingMode { get; }
    public SwitchBoardFactory SwitchBoard { get; }
    public TransformerFactory Transformer { get; }
}