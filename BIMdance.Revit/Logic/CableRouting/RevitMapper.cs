﻿namespace BIMdance.Revit.Logic.CableRouting;

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
        AddCreate<Connector, ConnectorProxy>(CreateConnectorProxy);
        AddCreate<Curve, SegmentProxy>(CreateSegmentProxy);
        AddCreate<ElectricalSystem, ElectricalSystemProxy>(CreateElectricalSystemProxy);
        AddCreate<FamilyInstance, CableTrayConduitFittingProxy>(CreateCableTrayConduitFittingProxy);
        AddCreate<FamilyInstance, ElectricalElementProxy>(CreateElectricalElementProxy);
        AddCreate<Level, LevelProxy>(CreateLevelProxy);
        AddCreate<Line, SegmentProxy>(CreateSegmentProxy);
        AddCreate<Room, RoomProxy>(CreateRoomProxy);
        AddCreate<XYZ, XYZProxy>(CreateXYZProxy);
        AddCreate<XYZProxy, XYZ>(CreateXYZ);
        
        // AddModify<CableTrayProxy, CableTray>((proxy, revit) => (CableTray)ModifyCableTrayConduitBase(proxy, revit));
        // AddModify<ConduitProxy, Conduit>((proxy, revit) => (Conduit)ModifyCableTrayConduitBase(proxy, revit));
        // AddModify<CableTrayProxy, CableTrayConduitBase>(ModifyCableTrayConduitBase);
        // AddModify<ConduitProxy, CableTrayConduitBase>(ModifyCableTrayConduitBase);
        AddModify<CableTrayConduitBaseProxy, CableTrayConduitBase>(ModifyCableTrayConduitBase);
        AddModify<ElectricalSystemProxy, ElectricalSystem>(ModifyElectricalSystem);
        
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
        Point1 = Map<XYZ, XYZProxy>((cableTray.Location as LocationCurve)?.Curve.GetEndPoint(0)),
        Point2 = Map<XYZ, XYZProxy>((cableTray.Location as LocationCurve)?.Curve.GetEndPoint(1)),
        ServiceType = cableTray.get_Parameter(BuiltInParameter.RBS_CTC_SERVICE_TYPE).AsString(),
    };

    private static ConduitProxy CreateConduitProxy(Conduit conduit) => new()
    {
        RevitId = conduit.Id.GetValue(),
        Point1 = Map<XYZ, XYZProxy>((conduit.Location as LocationCurve)?.Curve.GetEndPoint(0)),
        Point2 = Map<XYZ, XYZProxy>((conduit.Location as LocationCurve)?.Curve.GetEndPoint(1)),
        ServiceType = conduit.get_Parameter(BuiltInParameter.RBS_CTC_SERVICE_TYPE).AsString(),
    };

    private static ConnectorProxy CreateConnectorProxy(Connector connector) =>
        new(connector.Id, point: Map<XYZ, XYZProxy>(connector.Origin));
    
    private static CableTrayConduitFittingProxy CreateCableTrayConduitFittingProxy(FamilyInstance familyInstance) => new()
    {
        RevitId = familyInstance.Id.GetValue(),
    };
    
    private static ElectricalElementProxy CreateElectricalElementProxy(FamilyInstance familyInstance)
    {
        var electricalElementProxy = new ElectricalElementProxy
        {
            Name = familyInstance.Name,
            RevitId = familyInstance.Id.GetValue(),
            CableReserve = AsDouble(familyInstance, SharedParameters.Cable_Reserve, 0),
            LocationPoint = Map<XYZ, XYZProxy>((familyInstance.Location as LocationPoint)?.Point),
            ServiceType = AsString(familyInstance, SharedParameters.ServiceType, string.Empty),
        };

        SetLevelAndRoomId(familyInstance, electricalElementProxy);
        SetTraceBinding(familyInstance, electricalElementProxy);
        
        return electricalElementProxy;
    }

    private static ElectricalSystemProxy CreateElectricalSystemProxy(ElectricalSystem electricalSystem) => new()
    {
        Name = electricalSystem.Name,
        RevitId = electricalSystem.Id.GetValue(),
        CableDesignation = AsString(electricalSystem, SharedParameters.Cable_Designation, string.Empty),
        CableDiameter = AsDouble(electricalSystem, SharedParameters.Cable_Diameter, 0d),
        CablesCount = AsInteger(electricalSystem, SharedParameters.Cables_Count, 1),
        CircuitDesignation = AsString(electricalSystem, SharedParameters.Circuit_Designation, string.Empty),
        Elements = electricalSystem.Elements.OfType<FamilyInstance>().Select(Map<FamilyInstance, ElectricalElementProxy>).ToList(),
        Topology = (ConnectionTopology)AsInteger(electricalSystem, SharedParameters.Topology, 0),
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

    private static SegmentProxy CreateSegmentProxy(Curve curve) =>
        new(Map<XYZ, XYZProxy>(curve.GetEndPoint(0)), Map<XYZ, XYZProxy>(curve.GetEndPoint(1)));
    
    private static XYZProxy CreateXYZProxy(XYZ xyz) =>
        xyz is not null ? new XYZProxy(xyz.X, xyz.Y, xyz.Z) : new XYZProxy();

    private static XYZ CreateXYZ(XYZProxy xyz) =>
        new(xyz.X, xyz.Y, xyz.Z);

    private static ElectricalSystem ModifyElectricalSystem(ElectricalSystemProxy electricalSystemProxy, ElectricalSystem electricalSystem)
    {
        electricalSystem.get_Parameter(SharedParameters.Cables_Count)?.Set(electricalSystemProxy.CablesCount);
        electricalSystem.get_Parameter(SharedParameters.Cable_Length)?.Set(electricalSystemProxy.CableLength.MillimetersToInternal());
        electricalSystem.get_Parameter(SharedParameters.Cable_Length_InCableTray)?.Set(electricalSystemProxy.CableLengthInCableTray.MillimetersToInternal());
        electricalSystem.get_Parameter(SharedParameters.Cable_Length_OutsideCableTray)?.Set(electricalSystemProxy.CableLengthOutsideCableTray.MillimetersToInternal());
        electricalSystem.get_Parameter(SharedParameters.Cable_LengthMax)?.Set(electricalSystemProxy.CableLengthMax.MillimetersToInternal());
        electricalSystem.get_Parameter(SharedParameters.Topology)?.Set((int)electricalSystemProxy.Topology);
        return electricalSystem;
    }

    private static CableTrayConduitBase ModifyCableTrayConduitBase(CableTrayConduitBaseProxy proxy, CableTrayConduitBase revit)
    {
        revit.get_Parameter(SharedParameters.ElectricalSystemIds)?.Set($"#{string.Join("#", proxy.ElectricalSystems.Select(x => x.RevitId))}#");
        revit.get_Parameter(SharedParameters.CableTrace)?.Set(proxy.CableTrace);
           
        // if (proxy is CableTrayProxy cableTray)
        //     revit.get_Parameter(SharedParameters.CableTray_Filling)?.Set(cableTray.Filling);

        return revit;
    }

    private static void SetLevelAndRoomId(FamilyInstance src, ElectricalElementProxy dest)
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

    private static void SetTraceBinding(FamilyInstance src, ElectricalElementProxy dest)
    {
        var traceBindingParameter = src?.get_Parameter(SharedParameters.CableTrayConduitIds);
        
        if (traceBindingParameter is null || !traceBindingParameter.HasValue)
            return;
        
        var traceBindingIdAsString = traceBindingParameter.AsString().Trim('#').Split('#').FirstOrDefault();
        
        if (string.IsNullOrWhiteSpace(traceBindingIdAsString) ||
            !int.TryParse(traceBindingIdAsString, out var traceBindingId))
            return;

        dest.SetTraceBinding(new ConduitProxy { RevitId = traceBindingId });
    }
    
    private static List<List<SegmentProxy>> GetSegments(SpatialElement spatialElement)
    {
        var segments = new List<List<SegmentProxy>>();
        var boundarySegments = spatialElement.GetBoundarySegments(new SpatialElementBoundaryOptions());

        foreach (var area in boundarySegments)
        {
            var areaSegments = new List<SegmentProxy>();
            segments.Add(areaSegments);
            
            foreach (var boundarySegment in area)
            {
                var tessellate = boundarySegment.GetCurve().Tessellate();
                
                for (var i = 1; i < tessellate.Count; i++)
                {
                    var xyz1 = tessellate[i - 1];
                    var xyz2 = tessellate[i];
                    var segment = new SegmentProxy(new XYZProxy(xyz1.X, xyz1.Y, xyz1.Z), new XYZProxy(xyz2.X, xyz2.Y, xyz2.Z));
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
