namespace BIMdance.Revit.Model.RevitProxy;

public class ElectricalElementProxy : ElectricalBase
{
    public ElectricalElementProxy() :
        this(NotCreatedInRevitId, null) { }

    public ElectricalElementProxy(int revitId, string name, BuiltInCategoryProxy category) :
        this(revitId, name) => Category = category;

    public ElectricalElementProxy(
        int revitId, string name,
        PhasesNumber phasesNumber, double lineToGroundVoltage) :
        this(revitId, name)
    {
        Category = BuiltInCategoryProxy.OST_ElectricalFixtures;
        SetBaseConnector(new ConnectorProxy(this, 1, phasesNumber, lineToGroundVoltage));
    }

    public ElectricalElementProxy(int revitId, string name) : base(revitId, name) { }

    public BuiltInCategoryProxy Category { get; set; }
    public string ServiceType { get; set; }
    public ConnectorProxy Connector { get; set; }
    public Point3D Origin => Connector?.Origin;
    public int LevelId { get; set; }
    public int RoomId { get; set; }
    public LevelProxy Level { get; set; }
    public RoomProxy Room { get; set; }
    public bool InRoom => (Room?.RevitId ?? -1) > 0;
    
    /// <summary>
    /// <see cref="ElectricalElementProxy"> must not have children</see>
    /// </summary>
    /// <param name="electrical"></param>
    public override void Add(ElectricalBase electrical) => ThrowChildrenException();

    /// <summary>
    /// <see cref="ElectricalElementProxy"> must not have children</see>
    /// </summary>
    /// <param name="electricals"></param>
    public override void AddRange(IEnumerable<ElectricalBase> electricals) => ThrowChildrenException();

    /// <summary>
    /// <see cref="ElectricalElementProxy"> must not have children</see>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="electrical"></param>
    public override void Insert(int index, ElectricalBase electrical) => ThrowChildrenException();

    /// <summary>
    /// <see cref="ElectricalElementProxy"/> must not have children
    /// </summary>
    /// <param name="electrical"></param>
    public override void Remove(ElectricalBase electrical) => ThrowChildrenException();

    /// <summary>
    /// <see cref="ElectricalElementProxy"/> must not have children
    /// </summary>
    /// <param name="electrical1"></param>
    /// <param name="electrical2"></param>
    public override void Move(ElectricalBase electrical1, ElectricalBase electrical2) => ThrowChildrenException();

    private static void ThrowChildrenException() => throw new NotImplementedException(
        $"{nameof(ElectricalElementProxy)} must not have children");

    #region Factory

    public static ElectricalElementProxy Create(
        string name,
        BuiltInCategoryProxy category = BuiltInCategoryProxy.INVALID,
        ElectricalSystemTypeProxy systemType = ElectricalSystemTypeProxy.UndefinedSystemType,
        Point3D point = null) =>
        Create(NotCreatedInRevitId, name, category, systemType, point);

    public static ElectricalElementProxy Create(
        int revitId, string name,
        BuiltInCategoryProxy category = BuiltInCategoryProxy.INVALID,
        ElectricalSystemTypeProxy systemType = ElectricalSystemTypeProxy.UndefinedSystemType,
        Point3D point = null)
    {
        var element = new ElectricalElementProxy(revitId, name)
        {
            Category = category,
        };

        var connector = new ConnectorProxy(element, 1, systemType, point);
        
        element.SetBaseConnector(connector);
        // element.Connectors.Add(connector);

        return element;
    }

    public static ElectricalElementProxy Create(
        string name,
        BuiltInCategoryProxy category,
        params ConnectorProxy[] connectors) =>
        Create(NotCreatedInRevitId, name, category, connectors);

    public static ElectricalElementProxy Create(
        int revitId, string name,
        BuiltInCategoryProxy category,
        params ConnectorProxy[] connectors)
    {
        var element = new ElectricalElementProxy(revitId, name)
        {
            Category = category,
        };

        foreach (var connector in connectors)
        {
            element.SetBaseConnector(connector);
            // connector.Owner = element;
            // element.Connectors.Add(connector);
        }
            
        return element;
    }
        
    public static ElectricalElementProxy Create(
        string name,
        BuiltInCategoryProxy category,
        LoadClassificationProxy loadClassification,
        PhasesNumber phasesNumber, double lineToGroundVoltage,
        double apparentLoad = 0, double powerFactor = 1,
        Point3D point = null) =>
        Create(
            NotCreatedInRevitId, name, category, 
            loadClassification, phasesNumber, lineToGroundVoltage,
            apparentLoad, powerFactor, point);

    public static ElectricalElementProxy Create(
        int revitId, string name,
        BuiltInCategoryProxy category,
        LoadClassificationProxy loadClassification,
        PhasesNumber phasesNumber, double lineToGroundVoltage,
        double apparentLoad = 0, double powerFactor = 1,
        Point3D point = null)
    {
        var element = new ElectricalElementProxy(revitId, name)
        {
            Category = category,
        };

        var phaseApparentLoad = phasesNumber != PhasesNumber.Undefined ? apparentLoad / (int)phasesNumber : 0;
            
        var connector = new ConnectorProxy(element, 1, phasesNumber, lineToGroundVoltage, point)
        {
            PowerParameters =
            {
                LoadClassification = loadClassification,
                OwnApparentLoad = apparentLoad,
                OwnApparentLoad1 = phasesNumber == PhasesNumber.Three ? phaseApparentLoad : default,
                OwnApparentLoad2 = phasesNumber == PhasesNumber.Three ? phaseApparentLoad : default,
                OwnApparentLoad3 = phasesNumber == PhasesNumber.Three ? phaseApparentLoad : default,
                OwnPowerFactor = powerFactor,
            },
            SystemType = ElectricalSystemTypeProxy.PowerCircuit,
        };

        element.SetBaseConnector(connector);
        // element.Connectors.Add(connector);

        return element;
    }

    public static ElectricalElementProxy Create1Phase(
        string name,
        BuiltInCategoryProxy category,
        LoadClassificationProxy loadClassification,
        double lineToGroundVoltage,
        double apparentLoad = 0, double powerFactor = 1,
        Point3D point = null) =>
        Create1Phase(NotCreatedInRevitId, name, category, loadClassification, lineToGroundVoltage, apparentLoad, powerFactor, point);

    public static ElectricalElementProxy Create1Phase(
        int revitId, string name,
        BuiltInCategoryProxy category,
        LoadClassificationProxy loadClassification,
        double lineToGroundVoltage,
        double apparentLoad = 0, double powerFactor = 1,
        Point3D point = null) =>
        Create(revitId, name, category, loadClassification, PhasesNumber.One, lineToGroundVoltage, apparentLoad, powerFactor, point);

    public static ElectricalElementProxy Create2Phases(
        string name,
        BuiltInCategoryProxy category,
        LoadClassificationProxy loadClassification,
        double lineToGroundVoltage,
        double apparentLoad = 0, double powerFactor = 1,
        Point3D point = null) =>
        Create2Phases(NotCreatedInRevitId, name, category, loadClassification, lineToGroundVoltage, apparentLoad, powerFactor, point);

    public static ElectricalElementProxy Create2Phases(
        int revitId, string name,
        BuiltInCategoryProxy category,
        LoadClassificationProxy loadClassification,
        double lineToGroundVoltage,
        double apparentLoad = 0, double powerFactor = 1,
        Point3D point = null) =>
        Create(revitId, name, category, loadClassification, PhasesNumber.Two, lineToGroundVoltage, apparentLoad, powerFactor, point);

    public static ElectricalElementProxy Create3Phases(
        string name,
        BuiltInCategoryProxy category,
        LoadClassificationProxy loadClassification,
        double lineToGroundVoltage,
        double apparentLoad = 0, double powerFactor = 1,
        Point3D point = null) =>
        Create3Phases(NotCreatedInRevitId, name, category, loadClassification, lineToGroundVoltage, apparentLoad, powerFactor, point);

    public static ElectricalElementProxy Create3Phases(
        int revitId, string name,
        BuiltInCategoryProxy category,
        LoadClassificationProxy loadClassification,
        double lineToGroundVoltage,
        double apparentLoad = 0, double powerFactor = 1,
        Point3D point = null) =>
        Create(revitId, name, category, loadClassification, PhasesNumber.Three, lineToGroundVoltage, apparentLoad, powerFactor, point);

    #endregion
}