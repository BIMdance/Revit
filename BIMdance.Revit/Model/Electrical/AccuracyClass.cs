namespace BIMdance.Revit.Model.Electrical;

public abstract class AccuracyClass
{
    protected AccuracyClass(double accuracy) => Accuracy = accuracy;
    public double Accuracy { get; protected set; }
    public override bool Equals(object obj) => GetType() == obj?.GetType();
    public override int GetHashCode() => GetType().GetHashCode();
}

public class MeasuringAccuracyClass : AccuracyClass
{
    public MeasuringAccuracyClass(double accuracy, bool specialApplication = false) : base(accuracy) =>
        SpecialApplication = specialApplication;

    public bool SpecialApplication { get; protected set; }
    public override string ToString() =>
        $"{Accuracy:0.0}{(SpecialApplication ? "S" : string.Empty)}";
}

public class ProtectionAccuracyClass : AccuracyClass
{
    public ProtectionAccuracyClass(double accuracy, int accuracyLimitFactor = 0) : base(accuracy) =>
        AccuracyLimitFactor = accuracyLimitFactor;

    public int AccuracyLimitFactor { get; protected set; }
    public override string ToString() =>
        $"{Accuracy}P{(AccuracyLimitFactor > 0 ? AccuracyLimitFactor.ToString(CultureInfo.InvariantCulture) : string.Empty)}";
}