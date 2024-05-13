namespace BIMdance.Revit.Utils;

public enum Converting
{
    MetersFromFeet,
    MetersFromInternal,
    MetersToFeet,
    MetersToInternal,
    MillimetersFromFeet,
    MillimetersFromInternal,
    MillimetersToFeet,
    MillimetersToInternal,
    OhmMeterFromInternal,
    OhmMeterToInternal,
    SquareMetersFromFeet,
    SquareMetersToFeet,
    SquareMillimetersFromFeet,
    SquareMillimetersToFeet,
    VoltsFromInternal,
    VoltsToInternal,
    VoltAmperesFromInternal,
    VoltAmperesToInternal,
    WattsFromInternal,
    WattsToInternal,
    FromKilo,
    FromMega,
    FromMicro,
    FromMilli,
    FromSquareKilo,
    FromSquareMega,
    FromSquareMilli,
    FromSquareMicro,
    ToKilo,
    ToMega,
    ToMicro,
    ToMilli,
    ToSquareKilo,
    ToSquareMega,
    ToSquareMilli,
    ToSquareMicro,
}
    
public static class UnitConverter
{
    private const double MetersInFoot = 0.3048;
    private const double MetersInFoot2 = MetersInFoot * MetersInFoot;
    private const double MetersInFoot3 = MetersInFoot2 * MetersInFoot;
    private const double MillimetersInFoot = 304.8;

    public static double MillimetersToInternal(this double value) => value.Convert(Converting.MillimetersToInternal);
    public static double MillimetersFromInternal(this double value) => value.Convert(Converting.MillimetersFromInternal);
    public static double MetersFromInternal(this double value) => value.Convert(Converting.MetersFromInternal);
    
    public static double Convert(this double value, Converting converting, bool round = false)
    {
        var result = GetFormula(converting).Invoke(value);
        return round ? System.Math.Round(result) : result;
    }

    public static double Convert(this double value, Converting converting, int significantDigits, int? digits = null)
    {
        return GetFormula(converting).Invoke(value).Round(significantDigits, digits);
    }

    private static Func<double, double> GetFormula(Converting converting)
    {
        switch (converting)
        {
            case Converting.MillimetersFromFeet:
            case Converting.MillimetersFromInternal:
                return feet => feet * MillimetersInFoot;

            case Converting.MillimetersToFeet:
            case Converting.MillimetersToInternal:
                return millimeters => millimeters / MillimetersInFoot;

            case Converting.MetersFromFeet:
            case Converting.MetersFromInternal:
                return feet => feet * MetersInFoot;

            case Converting.MetersToFeet:
            case Converting.MetersToInternal:
                return meters => meters / MetersInFoot;

            case Converting.OhmMeterFromInternal:
                return internalOhmMeter => internalOhmMeter * MetersInFoot3;

            case Converting.OhmMeterToInternal:
                return ohmMeters => ohmMeters / MetersInFoot3;

            case Converting.SquareMetersFromFeet:
            case Converting.VoltsFromInternal:
            case Converting.VoltAmperesFromInternal:
            case Converting.WattsFromInternal:
                return value => value * MetersInFoot2;

            case Converting.SquareMetersToFeet:
            case Converting.VoltsToInternal:
            case Converting.VoltAmperesToInternal:
            case Converting.WattsToInternal:
                return value => value / MetersInFoot2;

            case Converting.FromKilo:
            case Converting.ToMilli:
                return value => value * 1e3;

            case Converting.FromMega:
            case Converting.ToMicro:
                return value => value * 1e6;

            case Converting.FromMicro:
            case Converting.ToMega:
                return value => value * 1e-6;

            case Converting.FromMilli:
            case Converting.ToKilo:
                return value => value * 1e-3;

            case Converting.FromSquareKilo:
            case Converting.ToSquareMilli:
                return value => value * 1e6;

            case Converting.FromSquareMega:
            case Converting.ToSquareMicro:
                return value => value * 1e12;

            case Converting.FromSquareMicro:
            case Converting.ToSquareMega:
                return value => value * 1e-12;

            case Converting.FromSquareMilli:
            case Converting.ToSquareKilo:
                return value => value * 1e-6;

            default:
                throw new ArgumentOutOfRangeException(nameof(converting), converting, null);
        }
    }
}