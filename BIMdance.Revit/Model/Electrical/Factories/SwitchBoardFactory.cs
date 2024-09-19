namespace BIMdance.Revit.Model.Electrical.Factories;

public class SwitchBoardFactory : ElectricalFactoryBase<SwitchBoard>
{
    private readonly ElectricalSystemFactory _electricalSystemFactory;
    private readonly SwitchBoardUnitFactory _switchBoardUnitFactory;
    
    public SwitchBoardFactory(ElectricalContext electricalContext, ElectricalSystemFactory electricalSystemFactory) :
        base(electricalContext, electricalContext.SwitchBoards, ModelLocalization.PrefixSwitchboard)
    {
        _electricalSystemFactory = electricalSystemFactory;
        _switchBoardUnitFactory = new SwitchBoardUnitFactory(electricalContext);
    }

    public SwitchBoard Create(DistributionSystemProxy distributionSystem) =>
        Create(() => SwitchBoard(NewName(), distributionSystem));
    public SwitchBoard Create(string name, DistributionSystemProxy distributionSystem) =>
        Create(() => SwitchBoard(name, distributionSystem));
    
    public SwitchBoard CreateInContext(DistributionSystemProxy distributionSystem) =>
        CreateInContext(() => SwitchBoardInContext(NewName(), distributionSystem));
    public SwitchBoard CreateInContext(string name, DistributionSystemProxy distributionSystem) =>
        CreateInContext(() => SwitchBoardInContext(name, distributionSystem));
    
    private SwitchBoard SwitchBoardInContext(string name, DistributionSystemProxy distributionSystem)
    {
        var switchBoard = SwitchBoard(name, distributionSystem);
        ElectricalContext.SwitchBoardUnits.Add(switchBoard.FirstUnit);
        return switchBoard;
    }
    
    private SwitchBoard SwitchBoard(string name, DistributionSystemProxy distributionSystem)
    {
        var switchBoard = new SwitchBoard(NewId(), name);
        var firstUnit = distributionSystem is not null
            ? _switchBoardUnitFactory.Create(name, distributionSystem)
            : _switchBoardUnitFactory.Create(name, ElectricalSystemTypeProxy.PowerCircuit);
        switchBoard.AddUnit(firstUnit);
        return switchBoard;
    }
    
    public SwitchBoard Create(SwitchBoard sourceSwitchBoard, DistributionSystemProxy distributionSystem) =>
        Create(() => SwitchBoard(NewName(), sourceSwitchBoard.FirstUnit, distributionSystem));
    public SwitchBoard Create(SwitchBoardUnit sourceSwitchBoard, DistributionSystemProxy distributionSystem) =>
        Create(() => SwitchBoard(NewName(), sourceSwitchBoard, distributionSystem));
    public SwitchBoard Create(string name, SwitchBoard sourceSwitchBoard, DistributionSystemProxy distributionSystem) =>
        Create(() => SwitchBoard(name, sourceSwitchBoard.FirstUnit, distributionSystem));
    public SwitchBoard Create(string name, SwitchBoardUnit sourceSwitchBoard, DistributionSystemProxy distributionSystem) =>
        Create(() => SwitchBoard(name, sourceSwitchBoard, distributionSystem));
    
    public SwitchBoard CreateInContext(SwitchBoard sourceSwitchBoard, DistributionSystemProxy distributionSystem) =>
        CreateInContext(() => SwitchBoardInContext(NewName(), sourceSwitchBoard.FirstUnit, distributionSystem));
    public SwitchBoard CreateInContext(SwitchBoardUnit sourceSwitchBoard, DistributionSystemProxy distributionSystem) =>
        CreateInContext(() => SwitchBoardInContext(NewName(), sourceSwitchBoard, distributionSystem));
    public SwitchBoard CreateInContext(string name, SwitchBoard sourceSwitchBoard, DistributionSystemProxy distributionSystem) =>
        CreateInContext(() => SwitchBoardInContext(name, sourceSwitchBoard.FirstUnit, distributionSystem));
    public SwitchBoard CreateInContext(string name, SwitchBoardUnit sourceSwitchBoard, DistributionSystemProxy distributionSystem) =>
        CreateInContext(() => SwitchBoardInContext(name, sourceSwitchBoard, distributionSystem));
    
    private SwitchBoard SwitchBoardInContext(string name, SwitchBoardUnit sourceSwitchBoard, DistributionSystemProxy distributionSystem)
    {
        var switchBoard = SwitchBoard(name, sourceSwitchBoard, distributionSystem);
        ElectricalContext.SwitchBoardUnits.Add(switchBoard.FirstUnit);
        ElectricalContext.ElectricalSystems.Add((ElectricalSystemProxy)switchBoard.FirstUnit.BaseSource);
        return switchBoard;
    }

    private SwitchBoard SwitchBoard(string name, SwitchBoardUnit sourceSwitchBoard, DistributionSystemProxy distributionSystem)
    {
        var switchBoard = new SwitchBoard(NewId(), name);
        var firstUnit = _switchBoardUnitFactory.Create(name, distributionSystem);
        switchBoard.AddUnit(firstUnit);
        var electricalSystem = _electricalSystemFactory.Create(sourceSwitchBoard, distributionSystem, items: firstUnit);
        electricalSystem.ConnectTo(sourceSwitchBoard);
        return switchBoard;
    }

    public SwitchBoard Create(ElectricalSystemTypeProxy systemType = ElectricalSystemTypeProxy.UndefinedSystemType, BuiltInCategoryProxy category = BuiltInCategoryProxy.OST_ElectricalEquipment) =>
        Create(() => SwitchBoard(NewName(), systemType, category));
    public SwitchBoard Create(string name, ElectricalSystemTypeProxy systemType = ElectricalSystemTypeProxy.UndefinedSystemType, BuiltInCategoryProxy category = BuiltInCategoryProxy.OST_ElectricalEquipment) =>
        Create(() => SwitchBoard(name, systemType, category));

    public SwitchBoard CreateInContext(ElectricalSystemTypeProxy systemType = ElectricalSystemTypeProxy.UndefinedSystemType, BuiltInCategoryProxy category = BuiltInCategoryProxy.OST_ElectricalEquipment) =>
        CreateInContext(() => SwitchBoardInContext(NewName(), systemType, category));
    public SwitchBoard CreateInContext(string name, ElectricalSystemTypeProxy systemType = ElectricalSystemTypeProxy.UndefinedSystemType, BuiltInCategoryProxy category = BuiltInCategoryProxy.OST_ElectricalEquipment) =>
        CreateInContext(() => SwitchBoardInContext(name, systemType, category));

    private SwitchBoard SwitchBoardInContext(string name, ElectricalSystemTypeProxy systemType = ElectricalSystemTypeProxy.UndefinedSystemType, BuiltInCategoryProxy category = BuiltInCategoryProxy.OST_ElectricalEquipment)
    {
        var switchBoard = SwitchBoard(name, systemType, category);
        ElectricalContext.SwitchBoardUnits.Add(switchBoard.FirstUnit);
        return switchBoard;
    }

    private SwitchBoard SwitchBoard(string name, ElectricalSystemTypeProxy systemType, BuiltInCategoryProxy category)
    {
        var switchBoard = new SwitchBoard(NewId(), name) { Category = category };
        var firstUnit = _switchBoardUnitFactory.Create(name, systemType, category);

        if (systemType == ElectricalSystemTypeProxy.PowerCircuit)
            firstUnit.BaseConnector.CreatePowerParameters(PhasesNumber.Undefined, 0);

        switchBoard.AddUnit(firstUnit);

        return switchBoard;
    }

    public SwitchBoardUnit CreateUnit(SwitchBoard switchBoard, string unitName) =>
        switchBoard.AddUnit(CreateSwitchBoardUnit(switchBoard, unitName));
    public SwitchBoardUnit CreateUnit(SwitchBoard switchBoard, int index, string unitName) =>
        switchBoard.InsertUnit(index, CreateSwitchBoardUnit(switchBoard, unitName));
    private SwitchBoardUnit CreateSwitchBoardUnit(SwitchBoard switchBoard, string unitName) => switchBoard.DistributionSystem is not null
        ? _switchBoardUnitFactory.Create(unitName, switchBoard.DistributionSystem)
        : _switchBoardUnitFactory.Create(unitName, switchBoard.FirstUnit.SystemType, switchBoard.Category);

    public SwitchBoardUnit CreateUnitInContext(SwitchBoard switchBoard, string unitName) =>
        switchBoard.AddUnit(CreateSwitchBoardUnitInContext(switchBoard, unitName));
    public SwitchBoardUnit CreateUnitInContext(SwitchBoard switchBoard, int index, string unitName) =>
        switchBoard.InsertUnit(index, CreateSwitchBoardUnitInContext(switchBoard, unitName));
    private SwitchBoardUnit CreateSwitchBoardUnitInContext(SwitchBoard switchBoard, string unitName) => switchBoard.DistributionSystem is not null
        ? _switchBoardUnitFactory.CreateInContext(unitName, switchBoard.DistributionSystem)
        : _switchBoardUnitFactory.CreateInContext(unitName, switchBoard.FirstUnit.SystemType, switchBoard.Category);

    private class SwitchBoardUnitFactory : ElectricalFactoryBase<SwitchBoardUnit>
    {
        public SwitchBoardUnitFactory(ElectricalContext electricalContext) : base(electricalContext, electricalContext.SwitchBoardUnits) { }

        public SwitchBoardUnit Create(string name, ElectricalSystemTypeProxy systemType, BuiltInCategoryProxy category = BuiltInCategoryProxy.OST_ElectricalEquipment) => 
            Create(() => new SwitchBoardUnit(NewId(), name, systemType) { Category = category });
        
        public SwitchBoardUnit CreateInContext(string name, ElectricalSystemTypeProxy systemType, BuiltInCategoryProxy category = BuiltInCategoryProxy.OST_ElectricalEquipment) => 
            CreateInContext(() => new SwitchBoardUnit(NewId(), name, systemType) { Category = category });
        
        public SwitchBoardUnit Create(string name, DistributionSystemProxy distributionSystem) =>
            Create(() => new SwitchBoardUnit(NewId(), name, distributionSystem));
        
        public SwitchBoardUnit CreateInContext(string name, DistributionSystemProxy distributionSystem) =>
            CreateInContext(() => new SwitchBoardUnit(NewId(), name, distributionSystem));
        
        public SwitchBoardUnit Create(string name, PhasesNumber phasesNumber, double voltage) =>
            Create(() => new SwitchBoardUnit(NewId(), name, phasesNumber, voltage));
        
        public SwitchBoardUnit CreateInContext(string name, PhasesNumber phasesNumber, double voltage) =>
            CreateInContext(() => new SwitchBoardUnit(NewId(), name, phasesNumber, voltage));
    }
}

