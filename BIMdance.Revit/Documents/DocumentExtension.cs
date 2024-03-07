namespace BIMdance.Revit.Documents;

public static class DocumentExtension
{
    public static Family GetFamilyByName(this Document document, string familyName) =>
        !string.IsNullOrWhiteSpace(familyName)
            ? document.GetElementsOfClass<Family>().FirstOrDefault(x => x.Name == familyName)
            : null;

    public static Element GetElement(this Document document, ElementId elementId) => document.GetElement(elementId);
    public static T GetElementAs<T>(this Document document, Element element) => document.GetElement(element.Id) is T t ? t : default;
    public static T GetElementAs<T>(this Document document, ElementId elementId) => document.GetElement(elementId) is T t ? t : default;

    public static List<T> GetElementsOfCategory<T>(this Document document, BuiltInCategory builtInCategory)
    {
        using var collector = document.NewElementCollector();
        return collector.OfCategory(builtInCategory).OfType<T>().ToList();
    }
    
    public static List<T> GetElementsOfCategory<T>(this Document document, BuiltInCategory builtInCategory, ElementFilter elementFilter)
    {
        using var collector = document.NewElementCollector();
        return collector.OfCategory(builtInCategory).WherePasses(elementFilter).OfType<T>().ToList();
    }

    public static List<T> GetElementsOfCategoryIsNotElementType<T>(this Document document, BuiltInCategory builtInCategory)
    {
        using var collector = document.NewElementCollector();
        return collector.OfCategory(builtInCategory)
            .WhereElementIsNotElementType()
            .OfType<T>().ToList();
    }
    
    public static List<T> GetElementsOfClass<T>(this Document document)
    {
        using var collector = document.NewElementCollector();
        return collector.OfClass(typeof(T)).OfType<T>().ToList();
    }
    
    public static List<T> GetElementsOfClass<T>(this Document document, ElementFilter elementFilter)
    {
        using var collector = document.NewElementCollector();
        return collector.OfClass(typeof(T)).WherePasses(elementFilter).OfType<T>().ToList();
    }

    public static T GetElementOfClass<T>(this Document document, Func<T, bool> firstOrDefaultFunc)
        where T : Element
    {
        using var collector = NewElementCollector(document);
        return collector
            .OfClass(typeof(T))
            .OfType<T>()
            .FirstOrDefault(firstOrDefaultFunc);
    }
    
    public static List<T> GetElementsOfClassIsElementType<T>(this Document document)
    {
        using var collector = document.NewElementCollector();
        return collector.OfClass(typeof(T)).WhereElementIsElementType().OfType<T>().ToList();
    }
    
    public static List<T> GetElementsOfClassIsNotElementType<T>(this Document document)
    {
        using var collector = document.NewElementCollector();
        return collector.OfClass(typeof(T)).WhereElementIsNotElementType().OfType<T>().ToList();
    }

    public static ViewFamilyType Get3dViewFamilyType(this Document document) =>
        document.GetElementsOfClass<ViewFamilyType>().FirstOrDefault(x => x.ViewFamily == ViewFamily.ThreeDimensional);

    public static List<ElectricalSystem> GetElectricalSystems(
        this Document document, bool includeSpare = false) =>
        includeSpare
            ? document.GetElementsOfClass<ElectricalSystem>()
            : document.GetElementsOfCategory<ElectricalSystem>(BuiltInCategory.OST_ElectricalCircuit);

    public static Family GetFamily(this Document document, string familyName) =>
        !string.IsNullOrWhiteSpace(familyName)
            ? document.GetElementsOfClass<Family>().FirstOrDefault(x => x.Name == familyName)
            : null;

    public static FamilySymbol GetFamilySymbol(
        this Document document, string familyName, string symbolName) =>
        !string.IsNullOrWhiteSpace(familyName) && !string.IsNullOrWhiteSpace(symbolName)
            ? document.GetElementsOfClass<FamilySymbol>().FirstOrDefault(n => n.FamilyName == familyName && n.Name == symbolName)
            : null;

    public static List<Family> GetFamilies(this Document document) => document.GetElementsOfClass<Family>();
    public static IEnumerable<Element> GetAllElementsOnView(this Document document, View view) => new FilteredElementCollector(document, view.Id);
    public static IEnumerable<GraphicsStyle> GetGraphicStyles(this Document document) =>
        document.GetElementsOfClass<GraphicsStyle>();

    public static ProjectInfo GetProjectInfo(this Document document) => document.ProjectInformation;
    public static IEnumerable<Level> GetLevels(this Document document) => document.GetElementsOfCategoryIsNotElementType<Level>(BuiltInCategory.OST_Levels);
    public static IEnumerable<Room> GetRooms(
        this Document document, bool isPlacedOnly = false)
    {
        using var collector = NewElementCollector(document);
        var allRooms = collector
            .OfCategory(BuiltInCategory.OST_Rooms)
            .OfType<Room>()
            .ToArray();
        return isPlacedOnly
            ? allRooms.Where(n => n.Location != null)
            : allRooms;
    }
    public static FilteredElementCollector NewElementCollector(this Document document) => new(document);
}