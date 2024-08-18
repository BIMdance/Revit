namespace BIMdance.Revit.Logic.Mapping;

public static class RevitMapper
{
    public static TTarget Map<TSource, TTarget>(TSource source) =>
        Dictionary.TryGetValue((typeof(TSource), typeof(TTarget)), out var value) &&
        value is Func<TSource, TTarget> func
            ? func.Invoke(source)
            : default;
    
    public static TTarget Map<TSource, TTarget>(TSource source, TTarget target) =>
        Dictionary.TryGetValue((typeof(TSource), typeof(TTarget)), out var value) &&
        value is Func<TSource, TTarget, TTarget> func
            ? func.Invoke(source, target)
            : default;

    private static Dictionary<(Type, Type), object> _dictionary;
    private static Dictionary<(Type, Type), object> Dictionary => _dictionary ??= Initialize();

    private static Dictionary<(Type, Type), object> Initialize()
    {
        var dictionary = new Dictionary<(Type, Type), object>();
        
        AddCreate<CableTray, CableTrayProxy>(CreateCableTrayProxy);
        AddCreate<Conduit, ConduitProxy>(CreateConduitProxy);
        AddCreate<Connector, TraceConnectorProxy>(CreateConnectorProxy);
        AddCreate<Curve, Line3D>(CreateSegmentProxy);
        AddCreate<ElectricalSystem, TraceElectricalSystemProxy>(CreateElectricalSystemProxy);
        AddCreate<FamilyInstance, CableTrayConduitFittingProxy>(CreateCableTrayConduitFittingProxy);
        AddCreate<FamilyInstance, TraceElectricalElementProxy>(CreateElectricalElementProxy);
        AddCreate<Level, LevelProxy>(CreateLevelProxy);
        AddCreate<Line, Line3D>(CreateSegmentProxy);
        AddCreate<Room, RoomProxy>(CreateRoomProxy);
        AddCreate<XYZ, Point3D>(CreateXYZProxy);
        AddCreate<Point3D, XYZ>(CreateXYZ);
        
        // AddModify<CableTrayProxy, CableTray>((proxy, revit) => (CableTray)ModifyCableTrayConduitBase(proxy, revit));
        // AddModify<ConduitProxy, Conduit>((proxy, revit) => (Conduit)ModifyCableTrayConduitBase(proxy, revit));
        // AddModify<CableTrayProxy, CableTrayConduitBase>(ModifyCableTrayConduitBase);
        // AddModify<ConduitProxy, CableTrayConduitBase>(ModifyCableTrayConduitBase);
        AddModify<CableTrayConduitBaseProxy, CableTrayConduitBase>(ModifyCableTrayConduitBase);
        AddModify<TraceElectricalSystemProxy, ElectricalSystem>(ModifyElectricalSystem);
        
        return dictionary;
        
        void AddCreate<TSource, TTarget>(Func<TSource, TTarget> func)
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            if (dictionary.ContainsKey((sourceType, targetType)))
                dictionary[(sourceType, targetType)] = func;
            else
                dictionary.Add((sourceType, targetType), func);
        }
        
        void AddModify<TSource, TTarget>(Func<TSource, TTarget, TTarget> func)
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            if (dictionary.ContainsKey((sourceType, targetType)))
                dictionary[(sourceType, targetType)] = func;
            else
                dictionary.Add((sourceType, targetType), func);
        }
    }

    private static CableTrayProxy CreateCableTrayProxy(CableTray cableTray) => new()
    {
        RevitId = cableTray.Id.GetValue(),
        Point1 = Map<XYZ, Point3D>((cableTray.Location as LocationCurve)?.Curve.GetEndPoint(0)),
        Point2 = Map<XYZ, Point3D>((cableTray.Location as LocationCurve)?.Curve.GetEndPoint(1)),
        ServiceType = cableTray.get_Parameter(BuiltInParameter.RBS_CTC_SERVICE_TYPE).AsString(),
    };

    private static ConduitProxy CreateConduitProxy(Conduit conduit) => new()
    {
        RevitId = conduit.Id.GetValue(),
        Point1 = Map<XYZ, Point3D>((conduit.Location as LocationCurve)?.Curve.GetEndPoint(0)),
        Point2 = Map<XYZ, Point3D>((conduit.Location as LocationCurve)?.Curve.GetEndPoint(1)),
        ServiceType = conduit.get_Parameter(BuiltInParameter.RBS_CTC_SERVICE_TYPE).AsString(),
    };

    private static TraceConnectorProxy CreateConnectorProxy(Connector connector) =>
        new(connector.Id, point: Map<XYZ, Point3D>(connector.Origin));
    
    private static CableTrayConduitFittingProxy CreateCableTrayConduitFittingProxy(FamilyInstance familyInstance) => new()
    {
        RevitId = familyInstance.Id.GetValue(),
    };
    
    private static TraceElectricalElementProxy CreateElectricalElementProxy(FamilyInstance familyInstance)
    {
        var electricalElementProxy = new TraceElectricalElementProxy
        {
            Name = familyInstance.Name,
            RevitId = familyInstance.Id.GetValue(),
            CableReserve = AsDouble(familyInstance, Constants.SharedParameters.Cable.Reserve, 0),
            LocationPoint = Map<XYZ, Point3D>((familyInstance.Location as LocationPoint)?.Point),
            ServiceType = AsString(familyInstance, Constants.SharedParameters.ServiceType, string.Empty),
        };

        SetLevelAndRoomId(familyInstance, electricalElementProxy);
        SetTraceBinding(familyInstance, electricalElementProxy);
        
        return electricalElementProxy;
    }

    private static TraceElectricalSystemProxy CreateElectricalSystemProxy(ElectricalSystem electricalSystem) => new()
    {
        Name = electricalSystem.Name,
        RevitId = electricalSystem.Id.GetValue(),
        CableDesignation = AsString(electricalSystem, Constants.SharedParameters.Cable.Designation, string.Empty),
        CableDiameter = AsDouble(electricalSystem, Constants.SharedParameters.Cable.Diameter, 0d),
        CablesCount = AsInteger(electricalSystem, Constants.SharedParameters.Cable.Count, 1),
        CircuitDesignation = AsString(electricalSystem, Constants.SharedParameters.Circuit.Designation, string.Empty),
        Elements = electricalSystem.Elements.OfType<FamilyInstance>().Select(Map<FamilyInstance, TraceElectricalElementProxy>).ToList(),
        Topology = (ConnectionTopology)AsInteger(electricalSystem, Constants.SharedParameters.Circuit.Topology, 0),
    };

    private static LevelProxy CreateLevelProxy(Level level) => new()
    {
        Name = level.Name,
        RevitId = level.Id.GetValue(),
        Elevation = level.Elevation,
    };

    private static RoomProxy CreateRoomProxy(Room room) => new()
    {
        Name = room.Name,
        RevitId = room.Id.GetValue(),
        LevelId = room.LevelId.GetValue(),
        Segments = GetSegments(room),
    };

    private static Line3D CreateSegmentProxy(Curve curve) =>
        new(Map<XYZ, Point3D>(curve.GetEndPoint(0)), Map<XYZ, Point3D>(curve.GetEndPoint(1)));
    
    private static Point3D CreateXYZProxy(XYZ xyz) =>
        xyz is not null ? new Point3D(xyz.X, xyz.Y, xyz.Z) : new Point3D();

    private static XYZ CreateXYZ(Point3D xyz) =>
        new(xyz.X, xyz.Y, xyz.Z);

    private static ElectricalSystem ModifyElectricalSystem(TraceElectricalSystemProxy electricalSystemProxy, ElectricalSystem electricalSystem)
    {
        electricalSystem.get_Parameter(Constants.SharedParameters.Cable.Count)?.Set(electricalSystemProxy.CablesCount);
        electricalSystem.get_Parameter(Constants.SharedParameters.Cable.Length)?.Set(electricalSystemProxy.CableLength.MillimetersToInternal());
        electricalSystem.get_Parameter(Constants.SharedParameters.Cable.Length_InCableTray)?.Set(electricalSystemProxy.CableLengthInCableTray.MillimetersToInternal());
        electricalSystem.get_Parameter(Constants.SharedParameters.Cable.Length_OutsideCableTray)?.Set(electricalSystemProxy.CableLengthOutsideCableTray.MillimetersToInternal());
        electricalSystem.get_Parameter(Constants.SharedParameters.Cable.Length_Max)?.Set(electricalSystemProxy.CableLengthMax.MillimetersToInternal());
        electricalSystem.get_Parameter(Constants.SharedParameters.Circuit.Topology)?.Set((int)electricalSystemProxy.Topology);
        return electricalSystem;
    }

    private static CableTrayConduitBase ModifyCableTrayConduitBase(CableTrayConduitBaseProxy proxy, CableTrayConduitBase revit)
    {
        revit.get_Parameter(Constants.SharedParameters.ElectricalSystemIds)?.Set($"#{string.Join("#", proxy.ElectricalSystems.Select(x => x.RevitId))}#");
        revit.get_Parameter(Constants.SharedParameters.Cable.Trace)?.Set(proxy.CableTrace);
           
        // if (proxy is CableTrayProxy cableTray)
        //     revit.get_Parameter(SharedParameters.CableTray_Filling)?.Set(cableTray.Filling);

        return revit;
    }

    private static void SetLevelAndRoomId(FamilyInstance src, TraceElectricalElementProxy dest)
    {
        var levelId = src.GetLevelId();

        dest.LevelId = levelId.GetValue();
        dest.RoomId = src.Room?.Id.GetValue() ?? -1;
        
        if (dest.RoomId > 0 || dest.LevelId == -1 || src.Location is not LocationPoint locationPoint)
            return;

        var document = src.Document;
        var level = (Level)document.GetElement(levelId);
        var point = locationPoint.Point;
        var projectPoint = new XYZ(point.X, point.Y, level.Elevation + 0.01);
        var roomAtPoint = document.GetRoomAtPoint(projectPoint);

        dest.RoomId = roomAtPoint?.Id.GetValue() ?? -1;
    }

    private static void SetTraceBinding(FamilyInstance src, TraceElectricalElementProxy dest)
    {
        var traceBindingParameter = src?.get_Parameter(Constants.SharedParameters.CableTrayConduitIds);
        
        if (traceBindingParameter is null || !traceBindingParameter.HasValue)
            return;
        
        var traceBindingIdAsString = traceBindingParameter.AsString().Trim('#').Split('#').FirstOrDefault();
        
        if (string.IsNullOrWhiteSpace(traceBindingIdAsString) ||
            !int.TryParse(traceBindingIdAsString, out var traceBindingId))
            return;

        dest.SetTraceBinding(new ConduitProxy { RevitId = traceBindingId });
    }
    
    private static List<List<Line3D>> GetSegments(SpatialElement spatialElement)
    {
        var segments = new List<List<Line3D>>();
        var boundarySegments = spatialElement.GetBoundarySegments(new SpatialElementBoundaryOptions());

        foreach (var area in boundarySegments)
        {
            var areaSegments = new List<Line3D>();
            segments.Add(areaSegments);
            
            foreach (var boundarySegment in area)
            {
                var tessellate = boundarySegment.GetCurve().Tessellate();
                
                for (var i = 1; i < tessellate.Count; i++)
                {
                    var xyz1 = tessellate[i - 1];
                    var xyz2 = tessellate[i];
                    var segment = new Line3D(new Point3D(xyz1.X, xyz1.Y, xyz1.Z), new Point3D(xyz2.X, xyz2.Y, xyz2.Z));
                    areaSegments.Add(segment);
                }
            }
        }

        return segments;
    }
    
    private static double AsDouble(Element element, Guid guid, double defaultValue) => element.get_Parameter(guid)?.AsDouble() ?? defaultValue;
    private static int AsInteger(Element element, Guid guid, int defaultValue) => element.get_Parameter(guid)?.AsInteger() ?? defaultValue;
    private static string AsString(Element element, Guid guid, string defaultValue) => element.get_Parameter(guid)?.AsString() ?? defaultValue;
}
