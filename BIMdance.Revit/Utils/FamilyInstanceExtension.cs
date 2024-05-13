namespace BIMdance.Revit.Utils;

internal static class FamilyInstanceExtension
{
    internal static ElementId GetLevelId(this FamilyInstance familyInstance)
    {
        // var document = familyInstance.Document;

        if (familyInstance.LevelId.GetValue() != -1)
            return familyInstance.LevelId;
        else if (familyInstance.Room != null)
            return familyInstance.Room.LevelId;
        else if (familyInstance.Space != null)
            return familyInstance.Space.LevelId;
        else if (familyInstance.Host != null && familyInstance.Host.LevelId.GetValue() != -1)
            return familyInstance.Host.LevelId;
        else
            return ElementId.InvalidElementId;
        
        // else
        // {
        //     var pointZ = familyInstance.GetLocationZ();
        //     var levels = new FilteredElementCollector(document)
        //         .OfCategory(BuiltInCategory.OST_Levels)
        //         .WhereElementIsNotElementType()
        //         .OfType<Level>()
        //         .OrderByDescending(n => n.Elevation)
        //         .ToList();
        //
        //     foreach (var l in levels.Where(l => l.Elevation < pointZ))
        //     {
        //         level = l;
        //         break;
        //     }
        //
        //     level ??= levels.LastOrDefault();
        // }
        //
        // return level;
    }

    private static double GetLocationZ(this Element familyInstance)
    {
        return familyInstance.Location switch
        {
            LocationPoint locationPoint => locationPoint.Point.Z,
            LocationCurve locationCurve => locationCurve.Curve.GetEndPoint(0).Z,
            _ => throw new ArgumentOutOfRangeException(nameof(familyInstance.Location), familyInstance.Location,
                new ArgumentOutOfRangeException().Message)
        };
    }
    
    internal static Room GetRoom(this FamilyInstance familyInstance, Level level, List<Room> roomsOfLevel)
    {
        if (familyInstance.Room != null)
            return familyInstance.Room;
        
        if (familyInstance.Location is not LocationPoint locationPoint)
            return null;
            
        var point = locationPoint.Point;
        var projectPoint = new XYZ(point.X, point.Y, level.Elevation);
        var roomAtPoint = familyInstance.Document.GetRoomAtPoint(projectPoint);

        if (roomAtPoint != null)
            return roomAtPoint;

        var projectFloorPoint = new XYZ(point.X, point.Y, level.Elevation);

        return roomsOfLevel.FirstOrDefault(n => IsPointInSpatialElement(n, projectFloorPoint, 0.1));
    }

    private static bool IsPointInSpatialElement(SpatialElement spatialElement, XYZ projectFloorPoint, double tolerance)
    {
        var line = Line.CreateBound(projectFloorPoint, new XYZ(projectFloorPoint.X + 1000000, projectFloorPoint.Y, projectFloorPoint.Z));
        var intersectionCount = 0;
        var listListBoundaries = spatialElement.GetBoundarySegments(new SpatialElementBoundaryOptions());

        foreach (var boundarySegments in listListBoundaries)
        {
            foreach (var boundarySegment in boundarySegments)
            {
                var curve = boundarySegment.GetCurve();
                var projectResult = curve.Project(projectFloorPoint);

                if (projectResult.Distance < tolerance)
                    return true;

                var intersectResult = line.Intersect(curve);

                if (intersectResult == SetComparisonResult.Overlap)
                    intersectionCount++;
            }
        }

        return intersectionCount % 2 == 1;
    }
}