namespace BIMdance.Revit.Model.Electrical.MediumVoltage.Properties;

public class SwitchGearVoltage : IComparable
{
    public SwitchGearVoltage(
        int ratedVoltage,
        int maxOperatingVoltage,
        int withstandVoltage,
        int impulseWithstandVoltage)
    {
        RatedVoltage = ratedVoltage;
        MaxOperatingVoltage = maxOperatingVoltage;
        WithstandVoltage = withstandVoltage;
        ImpulseWithstandVoltage = impulseWithstandVoltage;
    }

    public int RatedVoltage { get; } // 6 000
    public int MaxOperatingVoltage { get; } // 7 200
    public int WithstandVoltage { get; } // 32 000
    public int ImpulseWithstandVoltage { get; } // 60 000

    public override bool Equals(object obj) =>
        obj is SwitchGearVoltage other &&
        this.RatedVoltage == other.RatedVoltage;

    public override int GetHashCode() => RatedVoltage.GetHashCode();

    public int CompareTo(object obj)
    {
        if (obj is not SwitchGearVoltage term)
            return 1;

        return
            RatedVoltage < term.RatedVoltage ? -1 :
            RatedVoltage > term.RatedVoltage ? 1 :
            0;
    }

    public override string ToString() => $"{RatedVoltage} / {MaxOperatingVoltage}";
}