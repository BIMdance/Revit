namespace BIMdance.Revit.Model.Electrical.Products.SwitchGears;

public class SwitchGearComponentCollections
{
    private readonly Dictionary<Type, object> _components = new();

    public UniqueCollection<CurrentTransformer> CurrentTransformers { get; set; } = new();
    public UniqueCollection<MeasuringVoltageTransformer> MeasuringVoltageTransformers { get; set; } = new();
    public UniqueCollection<Manufactured> DefaultComponents { get; set; } = new();
    public UniqueCollection<FaultPassageIndicator> FaultPassageIndicators { get; set; } = new();
    public UniqueCollection<AuxiliaryContact> AuxiliaryContacts { get; set; } = new();
    public UniqueCollection<Fuse> Fuses { get; set; } = new();
    public UniqueCollection<ProtectionRelay> ProtectionRelays { get; set; } = new();
    public UniqueCollection<MotorMechanism> RemoteControls { get; set; } = new();
    public UniqueCollection<ShuntTripRelease> ShuntTripReleases { get; set; } = new();
    public UniqueCollection<SwitchController> SwitchControllers { get; set; } = new();
    public UniqueCollection<UndervoltageRelease> UndervoltageReleases { get; set; } = new();

    public T GetDefaultComponent<T>() where T : Manufactured =>
        DefaultComponents.FirstOrDefault(n => n.GetType() == typeof(T)) as T ??
        GetComponents<T>().FirstOrDefault();

    public T Get<T>(Guid guid) where T : Manufactured =>
        GetComponents<T>().FirstOrDefault(x => x.Product.Guid == guid) ??
        throw new KeyNotFoundException($"Product was not found.\n Type: {typeof(T)}\n Guid: {guid}");

    public UniqueCollection<T> GetComponents<T>() where T : Manufactured
    {
        var type = typeof(T);

        if (_components.TryGetValue(type, out var value) &&
            value is UniqueCollection<T> uniqueCollection)
        {
            return uniqueCollection;
        }
        
        var collection = GetType().GetProperties()
            .FirstOrDefault(x => x.PropertyType == typeof(UniqueCollection<T>))?
            .GetValue(this) as UniqueCollection<T> ?? new UniqueCollection<T>();
        
        _components.Add(type, collection);
        
        return collection;
    }
}