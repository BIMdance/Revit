namespace BIMdance.Revit.Utils;

public static class DoubleExtension
{
    public const double Epsilon = 1E-8;
    public const double NegativeEpsilon = -Epsilon;

    public static double Round(this double value) =>
        value.Round(significantDigits: null);

    public static double Round(this double value, int? significantDigits) =>
        value.Round(significantDigits, digits: null);

    public static double RoundTo(this double value, int? digits) =>
        value.Round(significantDigits: null, digits: digits);
        
    /// <summary>
    /// Округлить число до указанного числа значащих цифр
    /// </summary>
    /// <param name="value">Округляемое число</param>
    /// <param name="significantDigits">Число значащих цифр</param>
    /// <param name="digits">Число знаков в дробной части</param>
    /// <returns></returns>
    public static double Round(this double value, int? significantDigits, int? digits)
    {
        if (double.IsInfinity(value) || (significantDigits is null && digits is null))
            return System.Math.Round(value);
            
        var sign = System.Math.Sign(value);
        var absValue = System.Math.Abs(value);

        if (absValue < Epsilon ||
            digits is null && significantDigits is < 1 or > 16)
            return value;

        if (significantDigits is { } intSignificantDigits and > 0)
        {
            long i = 1;
            var x = System.Math.Pow(10, intSignificantDigits);

            if (absValue > x)
            {
                while (absValue > x)
                {
                    absValue /= 10;
                    i *= 10;
                }
                value = sign * System.Math.Round(absValue) * i;
            }
            else
            {
                x /= 10;
                while (absValue < x)
                {
                    absValue *= 10;
                    i *= 10;
                }
                value = sign * System.Math.Round(absValue) / i;
            }
        }

        if (digits is not { } intDigits)
            return value;
        
        if (intDigits >= 0)
        {
            value = System.Math.Round(value, (int)digits);
        }
        else
        {
            var m = System.Math.Pow(10, System.Math.Abs(intDigits));
            value = System.Math.Round(value / m) * m;
        }

        return value;
    }

    public static string ToStringInvariant(this double value, int significantDigits, int? digits = null) =>
        value.Round(significantDigits, digits).ToStringInvariant();
        
    public static string ToStringInvariant(this double value) =>
        value.ToString(CultureInfo.InvariantCulture);
    
    public static string ToStringInvariant(this double value, string format) =>
        value.ToString(format, CultureInfo.InvariantCulture);

    public static bool IsEqualTo(this double source, double value) =>
        source.IsEqualTo(value, Epsilon);

    public static bool IsEqualTo(this double source, double value, double epsilon) =>
        System.Math.Abs(source - value) < epsilon;

    public static bool IsEqualToZero(this double value) =>
        System.Math.Abs(value) < Epsilon;
}