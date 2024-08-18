namespace BIMdance.Revit.Logic.Converters;

public class ElectricalSystemConverter
{
    private readonly Document _document;
    private readonly BuildingProxy _buildingProxy;
    private Dictionary<long, TraceElectricalElementProxy> _baseEquipments;

    public ElectricalSystemConverter(Document document, BuildingProxy buildingProxy)
    {
        _document = document;
        _buildingProxy = buildingProxy;
    }

    public List<TraceElectricalSystemProxy> Convert(params long[] electricalSystemIds)
    {
        _baseEquipments = new Dictionary<long, TraceElectricalElementProxy>();

        var electricalSystems = new FilteredElementCollector(_document)
            .OfClass(typeof(ElectricalSystem))
            .OfType<ElectricalSystem>()
            .Where(x => !electricalSystemIds.Any() || electricalSystemIds.Contains(x.Id.GetValue()))
            .Select(GetElectricalSystemProxy)
            .ToList();

        return electricalSystems;
    }

    public TraceElectricalSystemProxy Convert(int electricalSystemId)
    {
        _baseEquipments = new Dictionary<long, TraceElectricalElementProxy>();

        return GetElectricalSystemProxy(new FilteredElementCollector(_document)
            .OfClass(typeof(ElectricalSystem))
            .OfType<ElectricalSystem>()
            .FirstOrDefault(x => electricalSystemId == x.Id.GetValue()));
    }

    public TraceElectricalSystemProxy Convert(ElectricalSystem electricalSystem)
    {
        _baseEquipments = new Dictionary<long, TraceElectricalElementProxy>();

        return GetElectricalSystemProxy(electricalSystem);
    }

    private TraceElectricalSystemProxy GetElectricalSystemProxy(ElectricalSystem electricalSystem)
    {
        try
        {
            var proxy = RevitMapper.Map<ElectricalSystem, TraceElectricalSystemProxy>(electricalSystem);
            AddBaseEquipment(electricalSystem, proxy);
            SetProperties(proxy);
            return proxy;
        }
        catch (Exception exception)
        {
            Logger.Error(exception);
            return new TraceElectricalSystemProxy();
        }
    }

    private void AddBaseEquipment(MEPSystem x, TraceElectricalSystemProxy proxy)
    {
        if (x.BaseEquipment == null)
            return;
        
        if (_baseEquipments.TryGetValue(x.BaseEquipment.Id.GetValue(), out var existBaseEquipment))
        {
            Connect(proxy, existBaseEquipment);
        }
        else
        {
            var baseEquipment = RevitMapper.Map<FamilyInstance, TraceElectricalElementProxy>(x.BaseEquipment);
            _baseEquipments.Add(baseEquipment.RevitId, baseEquipment);
            Connect(proxy, baseEquipment);
        }
    }

    private static void Connect(TraceElectricalSystemProxy electricalSystem, TraceElectricalElementProxy baseEquipment)
    {
        electricalSystem.BaseEquipment = baseEquipment;
        baseEquipment.ElectricalSystems.Add(electricalSystem);
    }

    private void SetProperties(TraceElectricalSystemProxy electricalSystem)
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