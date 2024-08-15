namespace BIMdance.Revit.Utils.Revit;

public static class LevelUtils
{
    public static Level GetActiveLevel(Document document, long lastLevelId = 0)
    {
        var activeView = document.ActiveView;

        if (activeView.GenLevel is not null)
            return activeView.GenLevel;

        var allLevels = document.GetElementsOfClassIsNotElementType<Level>();
        
        if (activeView is View3D { IsSectionBoxActive: true } view3D &&
            view3D.GetDependentElements(null).FirstOrDefault(x =>
                document.GetElement(x) is { } element &&
                element.Category.Id.GetValue() == (int)BuiltInCategory.OST_SectionBox) is { } sectionBoxId &&
            document.GetElement(sectionBoxId) is { } sectionBox)
        {
            var boundingBox = sectionBox.get_BoundingBox(activeView);
            var allLevelsOnView = allLevels.Where(x => x.Elevation > boundingBox.Min.Z && x.Elevation < boundingBox.Max.Z).ToList();
            
            if (allLevelsOnView.Any())
                allLevels = allLevelsOnView;
        }

        if (allLevels.IsEmpty())
            return null;

        if (lastLevelId > 0 && allLevels.FirstOrDefault(x => x.Id.GetValue() == lastLevelId) is { } lastLevel)
            return lastLevel;
        
        if (activeView.SketchPlane is not null)
        {
            var nameSketchPlane = activeView.SketchPlane.Name;
            var level = allLevels.FirstOrDefault(n => n.Name == nameSketchPlane);
            if (level is not null) return level;
        }

        return allLevels.First();
    }
}