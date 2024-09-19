// ReSharper disable LocalizableElement

using BIMdance.Revit.Logic.DataAccess;
using BIMdance.Revit.Model.Electrical.Factories;
using BIMdance.Revit.Model.Electrical.Transformers;

namespace BIMdance.Revit.Model.Context;

public class ElectricalContext
{
    private readonly Action<Action, string> _transactionGroupAction;
    private readonly Dictionary<Type, ElementProvider> _elementProviders;
    private OperatingMode _currentOperatingMode;
    private int _id;
    private bool _isLoadedInConstructor;
    
    public ElectricalContext(
        TransactionGroupProvider transactionGroupProvider,
        ElementProvider<ElectricalSettings> electricalSettingsProvider,
        Set<OperatingMode> operatingModes,
        Set<DistributionSystemProxy> distributionSystems,
        Set<VoltageTypeProxy> voltageTypes,
        Set<LoadClassificationProxy> loadClassifications,
        Set<DemandFactorProxy> demandFactors,
        Set<Network> networks,
        Set<Generator> generators,
        Set<SwitchGear> switchGears,
        Set<SwitchGearUnit> switchGearUnits,
        Set<Transformer> transformers,
        Set<SwitchBoard> switchBoards,
        Set<SwitchBoardUnit> switchBoardUnits,
        Set<ElectricalElementProxy> electricalElements,
        Set<ElectricalSystemGroup> electricalSystemGroups,
        Set<ElectricalSystemProxy> electricalSystems)
    {
        _transactionGroupAction = transactionGroupProvider.TransactionGroupAction;
        
        ElectricalSettingsProvider = electricalSettingsProvider;
        OperatingModes = operatingModes;
        DistributionSystems = distributionSystems;
        VoltageTypes = voltageTypes;
        LoadClassifications = loadClassifications;
        DemandFactors = demandFactors;
        Networks = networks;
        Generators = generators;
        SwitchGears = switchGears;
        SwitchGearUnits = switchGearUnits;
        Transformers = transformers;
        SwitchBoards = switchBoards;
        SwitchBoardUnits = switchBoardUnits;
        ElectricalElements = electricalElements;
        ElectricalSystemGroups = electricalSystemGroups;
        ElectricalSystems = electricalSystems;

        _isLoadedInConstructor = true;
        
        _elementProviders = GetType().GetProperties()
            .Where(x =>
                x.PropertyType.IsGenericType &&
                x.PropertyType.GenericTypeArguments[0].IsSubclassOf(typeof(ElementProxy)))
            .OrderBy(x => x.PropertyType, new PropertyTypeComparer())
            .ToDictionary(x => x.PropertyType.GenericTypeArguments[0], x => x.GetValue(this) as ElementProvider);

        _id = OperatingMode.DefaultOperatingModeId - 1;
        
        _elementProviders.Values.OfType<IEnumerable>().ForEach(x =>
        {
            var elements = x.OfType<ElementProxy>().Select(e => e.RevitId).ToArray();
            var minId = elements.Any() ? elements.Min() : 0; 
            if (minId < _id) _id = (int)minId;
        });
    }

    public OperatingMode CurrentOperatingMode
    {
        get => _currentOperatingMode;
        set
        {
            _currentOperatingMode = value;
            ElectricalSettings.CurrentOperatingMode = value?.RevitId ?? default;
            Update(ElectricalSettings);
        }
    }

    public ElectricalFactory ElectricalFactory { get; set; }
    public ElementProvider<ElectricalSettings> ElectricalSettingsProvider { get; }
    public ElectricalSettings ElectricalSettings => ElectricalSettingsProvider?.Element;
    public IEnumerable<ElectricalSource> ElectricalSources => Networks.Concat<ElectricalSource>(Generators);
    public IEnumerable<ElectricalEquipmentProxy> Equipments => SwitchGearUnits.Concat<ElectricalEquipmentProxy>(Transformers).Concat(SwitchBoardUnits);
    public Set<OperatingMode> OperatingModes { get; }
    public Set<DistributionSystemProxy> DistributionSystems { get; }
    public Set<VoltageTypeProxy> VoltageTypes { get; }
    public Set<LoadClassificationProxy> LoadClassifications { get; }
    public Set<DemandFactorProxy> DemandFactors { get; }
    public Set<Network> Networks { get; }
    public Set<Generator> Generators { get; }
    public Set<SwitchGear> SwitchGears { get; }
    public Set<SwitchGearUnit> SwitchGearUnits { get; }
    public Set<Transformer> Transformers { get; }
    public Set<SwitchBoard> SwitchBoards { get; }
    public Set<SwitchBoardUnit> SwitchBoardUnits { get; }
    public Set<ElectricalElementProxy> ElectricalElements { get; }
    public Set<ElectricalSystemProxy> ElectricalSystems { get; }
    public Set<ElectricalSystemGroup> ElectricalSystemGroups { get; }
    
    public Set<T> Set<T>() where T : class => _elementProviders.TryGetValue(typeof(T), out var value)
        ? value as Set<T>
        : default;

    public int NewId() => --_id;

    public void Add(ElectricalEquipmentProxy electricalEquipment)
    {
        switch (electricalEquipment)
        {
            case SwitchBoardUnit switchBoardUnit: Add(switchBoardUnit); break;
            case SwitchGearUnit switchGearUnit: Add(switchGearUnit); break;
            case Transformer transformer: Add(transformer); break;
            default: throw new InvalidOperationException($"{electricalEquipment} is not valid type: {electricalEquipment.GetType()}");
        }
    }

    public void Remove(ElectricalEquipmentProxy electricalEquipment)
    {
        switch (electricalEquipment)
        {
            case SwitchBoardUnit switchBoardUnit: Remove(switchBoardUnit); break;
            case SwitchGearUnit switchGearUnit: Remove(switchGearUnit); break;
            case Transformer transformer: Remove(transformer); break;
            default: throw new InvalidOperationException($"{electricalEquipment} is not valid type: {electricalEquipment.GetType()}");
        }
    }

    public void Update(ElectricalEquipmentProxy electricalEquipment)
    {
        switch (electricalEquipment)
        {
            case SwitchBoardUnit switchBoardUnit: Update(switchBoardUnit); break;
            case SwitchGearUnit switchGearUnit: Update(switchGearUnit); break;
            case Transformer transformer: Update(transformer); break;
            default: throw new InvalidOperationException($"{electricalEquipment} is not valid type: {electricalEquipment.GetType()}");
        }
    }

    public void Add<T>(T item) where T : class => ItemAction(
        item,
        elementAction: repository => repository.Add(item),
        operationModeAction: operatingMode => OperatingModes.Add(operatingMode));

    public bool Contains<T>(T item) where T : class => ItemFunc(
        item,
        elementFunc: repository => repository.Contains(item),
        operationModeFunc: operatingMode => OperatingModes.Contains(operatingMode));

    public void Remove<T>(T item) where T : class => ItemAction(
        item,
        elementAction: repository => repository.Remove(item),
        operationModeAction: operatingMode => OperatingModes.Remove(operatingMode));

    public void Update<T>(T item) where T : class => ItemAction(
        item,
        elementAction: repository => repository.Update(item),
        operationModeAction: _ => OperatingModes.ChangedElements.UpdateStorage(),
        electricalSettingsAction: _ => ElectricalSettingsProvider.IsChanged = true);
    
    private void ItemAction<T>(
        T item,
        Action<Set<T>> elementAction,
        Action<OperatingMode> operationModeAction,
        Action<ElectricalSettings> electricalSettingsAction = null)
        where T : class
    {
        switch (item)
        {
            case ElementProxy or EquipmentSet
                when _elementProviders.TryGetValue(typeof(T), out var value) &&
                     value is Set<T> repository:
                elementAction(repository);
                break;
            
            case OperatingMode operatingMode:
                operationModeAction(operatingMode);
                break;
            
            case ElectricalSettings electricalSettings:
                electricalSettingsAction?.Invoke(electricalSettings);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(item), item, $"{nameof(item)} must be {nameof(ElementProxy)} or {nameof(OperatingMode)}");
        }
    }
    
    private TResult ItemFunc<T, TResult>(
        T item,
        Func<Set<T>, TResult> elementFunc,
        Func<OperatingMode, TResult> operationModeFunc)
        where T : class => item switch
    {
        ElementProxy or EquipmentSet when _elementProviders.TryGetValue(typeof(T), out var value) && value is Set<T> repository => elementFunc(repository),
        OperatingMode operatingMode => operationModeFunc(operatingMode),
        _ => throw new ArgumentOutOfRangeException(nameof(item), item, $"{nameof(item)} must be {nameof(ElementProxy)} or {nameof(OperatingMode)}")
    };

    private const int NotDirectSave = -1;

    public void Save()
    {
        if (_transactionGroupAction is not null)
            _transactionGroupAction(SaveAction, "Save");
        else
            SaveAction();
    }

    private void SaveAction() => _elementProviders
        .Where(x => GetIndex(x.Key) > NotDirectSave)
        .ForEach(x =>
        {
            // try { (x.Value ?? _elementProviderProperties[x.Key].GetValue(this) as ElementProvider)?.Save(); }
            try { x.Value.Save(); }
            catch (Exception exception) { Logger.Error(exception); }
        });

    public void Update()
    {
        try
        {
            if (!_isLoadedInConstructor) _elementProviders.Values.ForEach(x => x?.Load());
            
            CurrentOperatingMode = OperatingModes.FirstOrDefault(x => x.RevitId == ElectricalSettings.CurrentOperatingMode) ?? OperatingModes.FirstOrDefault();
            
            if (CurrentOperatingMode is null)
            {
                CurrentOperatingMode = OperatingMode.CreateDefault();
                OperatingModes.Add(CurrentOperatingMode);
                OperatingModes.Save();
            }
            
            ConnectToSources();
        }
        finally
        {
            _isLoadedInConstructor = false;
        }
    }

    private void ConnectToSources()
    {
        var notConnected = SwitchBoardUnits.OfType<ElectricalEquipmentProxy>()
            .Concat(Transformers)
            .Concat(SwitchBoardUnits)
            .Where(x =>
                x.BaseSource is null or BlankSource or ElectricalSystemProxy { BaseSource: null } ||
                x.BaseSource.RevitId is 0);

        foreach (var equipment in notConnected)
        {
            var distributionSystem = equipment.DistributionSystem;
            var compatibleNetworks = Networks.Where(x => distributionSystem.IsCompatible(x.DistributionSystem)).ToArray();
            var network =
                compatibleNetworks.FirstOrDefault(x => x.RevitId == equipment.BaseSource?.RevitId) ??
                compatibleNetworks.FirstOrDefault();

            network ??= ElectricalFactory.ElectricalNetwork.Create(distributionSystem);
            equipment.ConnectTo(network);
        }
        
        Networks.Save();
    }

    private class PropertyTypeComparer : IComparer<Type>
    {
        public int Compare(Type x, Type y)
        {
            var indexX = GetIndex(x);
            var indexY = GetIndex(y);
            return indexX < indexY ? -1 : indexX > indexY ? 1 : 0;
        }
    }
    
    private static int GetIndex(Type type)
    {
        type = type.IsGenericType ? type.GenericTypeArguments[0] : type;
        
        if (type == typeof(DemandFactorProxy))       return NotDirectSave;
        if (type == typeof(VoltageTypeProxy))        return NotDirectSave;
        if (type == typeof(OperatingMode))           return 0;
        if (type == typeof(LoadClassificationProxy)) return 1;
        if (type == typeof(DistributionSystemProxy)) return 2;
        if (type == typeof(Network))                 return 3;
        if (type == typeof(Generator))               return 4;
        if (type == typeof(SwitchGear))              return 5;
        if (type == typeof(SwitchGearUnit))          return 6;
        if (type == typeof(Transformer))             return 7;
        if (type == typeof(SwitchBoard))             return 8;
        if (type == typeof(SwitchBoardUnit))         return 9;
        if (type == typeof(ElectricalSystemProxy))   return 127;
        
        return 63;
    }
}