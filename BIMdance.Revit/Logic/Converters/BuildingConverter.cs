namespace BIMdance.Revit.Logic.Converters;

public class BuildingConverter
{
    private readonly Document _document;

    public BuildingConverter(Document document)
    {
        _document = document;
    }

    public BuildingProxy Convert()
    {
        var levels = new FilteredElementCollector(_document)
            .OfCategory(BuiltInCategory.OST_Levels)
            .WhereElementIsNotElementType()
            .OfType<Level>()
            .OrderByDescending(n => n.Elevation)
            .Select(RevitMapper.Map<Level, LevelProxy>)
            .ToList();
        
        var rooms = new FilteredElementCollector(_document)
            .OfCategory(BuiltInCategory.OST_Rooms)
            .WhereElementIsNotElementType()
            .OfType<Room>()
            .Where(room => room.Location != null)
            .Select(RevitMapper.Map<Room, RoomProxy>)
            .ToList();

        foreach (var room in rooms)
        {
            var level = levels.FirstOrDefault(level => level.RevitId == room.LevelId);
            
            if (level == null) continue;

            room.Level = level;
            level.Rooms.Add(room);
        }
        
        return new BuildingProxy(levels, rooms);
    }
}