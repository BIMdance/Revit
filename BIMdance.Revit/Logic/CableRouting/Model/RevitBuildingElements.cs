namespace BIMdance.Revit.Logic.CableRouting.Model;

public class RevitBuildingElements
{
    private readonly Dictionary<ElementId, List<Room>> _levelsRooms;

    public RevitBuildingElements(Document document)
    {
        _levelsRooms = new Dictionary<ElementId, List<Room>>();
        LevelsCurvesRooms = new Dictionary<ElementId, Dictionary<SegmentProxy, List<Room>>>();
        LevelsOrderByDescending = new FilteredElementCollector(document)
            .OfCategory(BuiltInCategory.OST_Levels)
            .WhereElementIsNotElementType()
            .OfType<Level>()
            .OrderByDescending(level => level.Elevation)
            .ToList();

        var rooms = new FilteredElementCollector(document)
            .OfCategory(BuiltInCategory.OST_Rooms)
            .OfType<Room>()
            .Where(room => room.Location != null)
            .ToList();
        
        BindRoomsToLevels(rooms);
    }
    
    public Dictionary<ElementId, Dictionary<SegmentProxy, List<Room>>> LevelsCurvesRooms { get; }
    public List<Level> LevelsOrderByDescending { get; }

    private void BindRoomsToLevels(IReadOnlyCollection<Room> rooms)
    {
        foreach (var level in LevelsOrderByDescending)
        {
            var roomsOfLevel = rooms.Where(n => n.LevelId == level.Id).ToList();

            _levelsRooms.Add(level.Id, roomsOfLevel);

            BindCurvesToRooms(level, rooms);
        }
    }

    private void BindCurvesToRooms(Level level, IEnumerable<Room> rooms)
    {
        var curvesRoomsOfLevel = new Dictionary<SegmentProxy, List<Room>>();

        foreach (var room in rooms)
        {
            var boundaries = room.GetBoundarySegments(new SpatialElementBoundaryOptions());

            foreach (var boundarySegments in boundaries)
            {
                foreach (var boundarySegment in boundarySegments)
                {
                    var curve = boundarySegment.GetCurve();
                    var segment = RevitMapper.Map<Curve, SegmentProxy>(curve);

                    if (curvesRoomsOfLevel.ContainsKey(segment))
                        curvesRoomsOfLevel[segment].Add(room);
                    else
                        curvesRoomsOfLevel.Add(segment, new List<Room> { room });
                }
            }
        }

        curvesRoomsOfLevel = curvesRoomsOfLevel
            .OrderBy(n => Math.Min(n.Key.Point1.X, n.Key.Point2.X))
            .ThenByDescending(n => Math.Min(n.Key.Point1.Y, n.Key.Point2.Y))
            .ToDictionary(n => n.Key, n => n.Value);

        LevelsCurvesRooms.Add(level.Id, curvesRoomsOfLevel);
    }

    public List<Room> GetRoomsOfLevel(ElementId levelId) => _levelsRooms.TryGetValue(levelId, out var rooms)
        ? rooms
        : new List<Room>();
}