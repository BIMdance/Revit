namespace BIMdance.Revit.Model.Electrical.MediumVoltage;

public abstract class RelayFunction
{
    protected RelayFunction(Guid guid, string description, string ansiCode)
    {
        Guid = guid;
        Description = description;
        AnsiCode = ansiCode;
        IsAnsi = !string.IsNullOrEmpty(AnsiCode);
    }
    public Guid Guid { get; }
    public bool IsAnsi { get; }
    public string AnsiCode { get; }
    public string Description { get; }
    public string Comments { get; set; }
    public override string ToString() => $"{(!string.IsNullOrEmpty(AnsiCode) ? $"{AnsiCode} - " : string.Empty)}{Description}{(!string.IsNullOrEmpty(Comments) ? $": {Comments}" : string.Empty)}";
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType() && Equals((RelayFunction) obj);
    }
    protected bool Equals(RelayFunction other) => Guid.Equals(other.Guid);
    public override int GetHashCode() => Guid.GetHashCode();
}

public class ControlAndMonitoringRelayFunction : RelayFunction
{
    public ControlAndMonitoringRelayFunction(Guid guid, string description, string ansiCode = null) :
        base(guid, description, ansiCode) { }
}

public class MeasurementRelayFunction : RelayFunction
{
    public MeasurementRelayFunction(Guid guid, string description, string ansiCode = null) :
        base(guid, description, ansiCode) { }
}

public class NetworkAndMachineDiagnosisRelayFunction : RelayFunction
{
    public NetworkAndMachineDiagnosisRelayFunction(Guid guid, string description, string ansiCode = null) :
        base(guid, description, ansiCode) { }
}

public class SwitchgearDiagnosisRelayFunction : RelayFunction
{
    public SwitchgearDiagnosisRelayFunction(Guid guid, string description, string ansiCode = null) :
        base(guid, description, ansiCode) { }
}

public class ProtectionRelayFunction : RelayFunction
{
    public ProtectionRelayFunction(Guid guid, string description, string ansiCode = null) :
        base(guid, description, ansiCode) { }
}