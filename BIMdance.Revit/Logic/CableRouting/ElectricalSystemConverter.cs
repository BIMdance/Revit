namespace BIMdance.Revit.Logic.CableRouting;

public class ElectricalSystemConverter
{
    private readonly Document _document;
    private readonly BuildingProxy _buildingProxy;
    private Dictionary<long, ElectricalElementProxy> _baseEquipments;

    public ElectricalSystemConverter(Document document, BuildingProxy buildingProxy)
    {
        _document = document;
        _buildingProxy = buildingProxy;
    }

    public List<ElectricalSystemProxy> Convert(params long[] electricalSystemIds)
    {
        _baseEquipments = new Dictionary<long, ElectricalElementProxy>();

        var electricalSystems = new FilteredElementCollector(_document)
            .OfClass(typeof(ElectricalSystem))
            .OfType<ElectricalSystem>()
            .Where(x => !electricalSystemIds.Any() || electricalSystemIds.Contains(x.Id.GetValue()))
            .Select(GetElectricalSystemProxy)
            .ToList();

        return electricalSystems;
    }

    public ElectricalSystemProxy Convert(int electricalSystemId)
    {
        _baseEquipments = new Dictionary<long, ElectricalElementProxy>();

        return GetElectricalSystemProxy(new FilteredElementCollector(_document)
            .OfClass(typeof(ElectricalSystem))
            .OfType<ElectricalSystem>()
            .FirstOrDefault(x => electricalSystemId == x.Id.GetValue()));
    }

    public ElectricalSystemProxy Convert(ElectricalSystem electricalSystem)
    {
        _baseEquipments = new Dictionary<long, ElectricalElementProxy>();

        return GetElectricalSystemProxy(electricalSystem);
    }

    private ElectricalSystemProxy GetElectricalSystemProxy(ElectricalSystem electricalSystem)
    {
        try
        {
            var proxy = RevitMapper.Map<ElectricalSystem, ElectricalSystemProxy>(electricalSystem);
            AddBaseEquipment(electricalSystem, proxy);
            SetProperties(proxy);
            return proxy;
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            return new ElectricalSystemProxy();
        }
    }

    private void AddBaseEquipment(MEPSystem x, ElectricalSystemProxy proxy)
    {
        if (x.BaseEquipment == null)
            return;
        
        if (_baseEquipments.TryGetValue(x.BaseEquipment.Id.GetValue(), out var existBaseEquipment))
        {
            Connect(proxy, existBaseEquipment);
        }
        else
        {
            var baseEquipment = RevitMapper.Map<FamilyInstance, ElectricalElementProxy>(x.BaseEquipment);
            _baseEquipments.Add(baseEquipment.RevitId, baseEquipment);
            Connect(proxy, baseEquipment);
        }
    }

    private static void Connect(ElectricalSystemProxy electricalSystem, ElectricalElementProxy baseEquipment)
    {
        electricalSystem.BaseEquipment = baseEquipment;
        baseEquipment.ElectricalSystems.Add(electricalSystem);
    }

    private void SetProperties(ElectricalSystemProxy electricalSystem)
    {
        var baseEquipment = electricalSystem.BaseEquipment;

        if (baseEquipment != null)
        {
            baseEquipment.Level = _buildingProxy.Levels.FirstOrDefault(level => level.RevitId == baseEquipment.LevelId);
            baseEquipment.Room = _buildingProxy.GetRoom(baseEquipment);
        }

        foreach (var element in electricalSystem.Elements)
        {
            element.ElectricalSystems.Add(electricalSystem);
            element.Level = _buildingProxy.Levels.FirstOrDefault(level => level.RevitId == element.LevelId);
            element.Room = _buildingProxy.GetRoom(element);
        }

        if (electricalSystem.CablesCount < 1)
        {
            electricalSystem.CablesCount = 1;
        }
    }
}