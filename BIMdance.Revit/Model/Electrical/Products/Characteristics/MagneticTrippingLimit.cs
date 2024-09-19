namespace BIMdance.Revit.Model.Electrical.Products.Characteristics;

public class MagneticTrippingLimit
{
    public double Nominal { get; }
    public double MinLimit { get; }
    public double MaxLimit { get; }
    public double RatedCurrent { get; }
    public double MinCurrent { get; }
    public double MaxCurrent { get; }

    public MagneticTrippingLimit(string valueAsString, double ratedCurrent) : this(valueAsString)
    {
        RatedCurrent = ratedCurrent;
        MinCurrent = ratedCurrent * MinLimit;
        MaxCurrent = ratedCurrent * MaxLimit;
    }

    public MagneticTrippingLimit(string valueAsString)
    {
        var split = valueAsString
            .Replace("_", string.Empty)
            .Replace("..", "-")
            .Split('X');

        if (split.Length < 1)
            return;

        var factors = split[0].Split('-');

        if (factors.Length == 1)
        {
            Nominal = factors[0].FirstDouble(defaultValue: 0);

            if (split.Length > 1)
            {
                if (split.Any(n => n.Contains("PT")))
                {
                    var limits = split.First(n => n.Contains("PT"));
                    var value = limits.Replace("+", string.Empty).Replace("-", string.Empty).FirstDouble(defaultValue: 0);

                    if (limits.Contains("+") || limits.Contains("-"))
                    {
                        MinLimit = limits.Contains("-") ? Nominal * (1 - value / 100) : Nominal;
                        MaxLimit = limits.Contains("+") ? Nominal * (1 + value / 100) : Nominal;
                    }
                    else
                    {
                        MinLimit = Nominal * (1 - value / 100);
                        MaxLimit = Nominal * (1 + value / 100);
                    }

                    return;
                }
            }

            MinLimit = Nominal * 0.8;
            MaxLimit = Nominal * 1.2;
        }
        else
        {
            MinLimit = factors[0].FirstDouble(defaultValue: 0);
            MaxLimit = factors[1].FirstDouble(defaultValue: 0);
            Nominal = (MinLimit + MaxLimit) / 2;
        }
    }
}