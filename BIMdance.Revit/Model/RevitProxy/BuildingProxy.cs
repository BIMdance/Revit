namespace BIMdance.Revit.Model.RevitProxy;

public class BuildingProxy
{
    public BuildingProxy() : this(levels: Array.Empty<LevelProxy>(), rooms: new List<RoomProxy>()) { }

    public BuildingProxy(IEnumerable<LevelProxy> levels, List<RoomProxy> rooms)
    {
        Levels = levels.OrderBy(x => x.Elevation).ToList();
        Rooms = rooms;
        OutRooms = rooms.FirstOrDefault(x => x.RevitId == -1);

        if (OutRooms != null)
            return;
        
        OutRooms = new RoomProxy { RevitId = -1 };
        rooms.Insert(0, OutRooms);
    }

    public List<LevelProxy> Levels { get; }
    public RoomProxy OutRooms { get; }
    public List<RoomProxy> Rooms { get; }
    
    public LevelProxy GetLevel(int revitId)
    {
        return revitId > 0 ? Levels.FirstOrDefault(n => n.RevitId == revitId) : null;
    }

    public LevelProxy GetLevel(Point3D point)
    {
        foreach (var level in Levels)
        {
            if (level.Elevation <= point.Z)
                return level;
        }

        return Levels.LastOrDefault();
    }

    public List<LevelProxy> GetLevels(Point3D point1, Point3D point2)
    {
        var levels = new List<LevelProxy>();
        var level0 = GetLevel(point1);
        var level1 = GetLevel(point2);

        if (level0?.RevitId == level1?.RevitId)
        {
            levels.Add(level0);
        }
        else
        {
            levels.Add(level0);
            levels.Add(level1);

            var elevation0 = level0?.Elevation ?? double.NaN;
            var elevation1 = level1?.Elevation ?? double.NaN;
            var minElevation = Math.Min(elevation0, elevation1);
            var maxElevation = Math.Max(elevation0, elevation1);

            levels.AddRange(Levels.Where(n => n.Elevation > minElevation && n.Elevation < maxElevation));
        }

        return levels;
    }
    
    public RoomProxy GetRoom(int revitId) => Rooms.FirstOrDefault(n => n.RevitId == revitId);

    public RoomProxy GetRoom(ElectricalElementProxy electricalElement)
    {
        if (electricalElement.RoomId > 0)
        {
            var room = Rooms.FirstOrDefault(x => x.RevitId == electricalElement.RoomId);

            if (room != null)
                return room;
        }

        return GetRoom(electricalElement.Level, electricalElement.LocationPoint);
    }
    
    public RoomProxy GetRoom(LevelProxy level, Point3D point)
    {
        return level?.Rooms.FirstOrDefault(room => room.IsPointInRoom(point)) ?? OutRooms;
    }

    public IEnumerable<RoomProxy> GetRooms(CableTrayConduitBaseProxy cableTrayConduit)
    {
        return cableTrayConduit.Orientation switch
        {
            CableTrayConduitOrientation.Horizontal => GetRoomsHorizontal(cableTrayConduit),
            CableTrayConduitOrientation.Vertical => GetRoomsVertical(cableTrayConduit),
            CableTrayConduitOrientation.Inclined => new List<RoomProxy>(),
            _ => new List<RoomProxy>()
        };
    }

    private List<RoomProxy> GetRoomsHorizontal(CableTrayConduitBaseProxy cableTrayConduit)
    {
        var rooms = new List<RoomProxy>();
        var level = cableTrayConduit.Levels.FirstOrDefault();
        var room1 = GetRoom(level, cableTrayConduit.Point1);
        var room2 = GetRoom(level, cableTrayConduit.Point2);

        if (Equals(room1, room2))
        {
            rooms.Add(room1);
        }
        else
        {
            rooms.Add(room1);
            rooms.Add(room2);
        }
        
        if (level == null)
            return rooms;
        
        var segment = new Line3D(cableTrayConduit.Point1, cableTrayConduit.Point2);

        rooms.AddRange(level.Rooms.Where(room =>
            !Equals(room, room1) &&
            !Equals(room, room2) &&
            room.Segments.SelectMany(x => x).Any(roomSegment => roomSegment.IsIntersect(segment))));

        return rooms;
    }

    private List<RoomProxy> GetRoomsVertical(CableTrayConduitBaseProxy cableTrayConduit)
    {
        var rooms = new List<RoomProxy>(cableTrayConduit.Levels.Count);
        
        rooms.AddRange(
            from level in cableTrayConduit.Levels
            let point = new Point3D(cableTrayConduit.Point1.X, cableTrayConduit.Point1.Y, level.Elevation)
            select GetRoom(level, point));

        return rooms;
    }
}