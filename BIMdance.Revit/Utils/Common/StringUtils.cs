namespace BIMdance.Revit.Utils.Common;

public static class StringUtils
{
    public static string DeleteMultipleSpaces(this string text)
    {
        const string pattern = @"\s+";
        const string target = " ";
        var regex = new Regex(pattern);
        return regex.Replace(text, target);
    }

    public static string Reverse(this string text)
    {
        var result = string.Empty;
        var reverse = Enumerable.Reverse(text);

        return reverse.Aggregate(result, (current, c) => current + c);
    }

    public static void Split(this string text, char separator, out string firstPart, out string secondPart)
    {
        var separatorIndex = text.IndexOf(separator);
        firstPart = separatorIndex > 0 ? text.Substring(0, separatorIndex) : text;
        secondPart = separatorIndex > 0 && separatorIndex < text.Length - 1 ? text.Substring(separatorIndex + 1) : null;
    }

    public static void Split(this string text, char separator, out string firstPart, out string secondPart, out string thirdPart)
    {
        text.Split(separator, out firstPart, out text);
        text.Split(separator, out secondPart, out thirdPart);
    }

    public static void Split(this string text, char separator, out string firstPart, out string secondPart, out string thirdPart, out string fourthPart)
    {
        text.Split(separator, out firstPart, out text);
        text.Split(separator, out secondPart, out text);
        text.Split(separator, out thirdPart, out fourthPart);
    }
    
    public static double ToDouble(this string text) => Convert.ToDouble(text.Replace('.', ','));

    public static int FirstInt(this string text, int defaultValue = default)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        const string pattern = @"(\-?\d+)|$";
        text = Regex.Matches(text, pattern)[0].Value;
        return text.Length != 0
            ? int.Parse(text)
            : defaultValue;
    }

    public static int LastInt(this string text, int errorValue)
    {
        if (string.IsNullOrEmpty(text))
            return errorValue;
        var i = text.Length - 1;
        while (i >= 0)
        {
            if (text[i] < 48 || text[i] > 57)
                break;
            i--;
        }
        text = text.Substring(i + 1);
        return text != string.Empty
            ? Convert.ToInt32(text)
            : errorValue;
    }

    public static int LastInt(this string text, int errorValue, out string prefix, out int count)
    {
        var i = text.Length - 1;
        while (i >= 0)
        {
            if (text[i] < 48 || text[i] > 57)
                break;
            i--;
        }

        prefix = text.Substring(0, i + 1);
        text = text.Substring(i + 1);

        var result = 0;
        var error = false;
        if (text != string.Empty)
            result = Convert.ToInt32(text);
        else
            error = true;

        count = text.Length;

        return !error ? result : errorValue;
    }

    public static double FirstDouble(this string text, double defaultValue)
    {
        return text.FirstDouble() ?? defaultValue;
    }
        
    public static double? FirstDouble(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return default;
            
        const string pattern = @"\-?\d+(\.\d+)?|$";
        text = text.Replace(",", ".");
        text = Regex.Matches(text, pattern)[0].Value;

        return text.Length != 0
            ? double.Parse(text, CultureInfo.InvariantCulture)
            : default;
    }

    public static double? FirstDoubleOrNull(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return null;

        const string pattern = @"\-?\d+(\.\d+)?|$";
        text = text.Replace(",", ".");
        text = Regex.Matches(text, pattern)[0].Value;

        return text.Length == 0
            ? null
            : double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var number)
                ? number
                : null;
    }

    public static string FirstDoubleToString(this string text)
    {
        return text.FirstDouble()?.ToString(CultureInfo.CurrentCulture);
    }

    public static string TextWithoutEndInt(this string text)
    {
        const string pattern = @"(.+)\s(\d+)$|$";
        var result = Regex.Matches(text, pattern)[0].Groups[1].Value;

        return result.Length == 0 ? text : result;
    }

    public static int LastInt(this string text)
    {
        const string pattern = @"\s(\d+)$|$";
        text = Regex.Matches(text, pattern)[0].Value;

        return text.Length == 0 ? 0 : int.Parse(text);
    }

    public static double LastDouble(this string text, char decimalSymbol = ',')
    {
        var currDecSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        var pattern = $@"\d+(\{currDecSeparator}\d+)?|$";
        text = text.Replace(".", currDecSeparator).Replace(decimalSymbol.ToString(), currDecSeparator);
        var matches = Regex.Matches(text, pattern);
        if (matches.Count > 1)
        {
            text = matches[matches.Count - 2].Value;
            return text.Length == 0 ? 0 : double.Parse(text);
        }
        else
        {
            return 0;
        }
    }

    public static string ToLastInt(this string text)
    {
        var i = text.Length - 1;
        while (i >= 0)
        {
            if (text[i] < 48 || text[i] > 57)
                break;
            i--;
        }
        return text.Substring(0, i + 1);
    }

    public static string ToComma(this string text)
    {
        if (text.IndexOf(',') >= 0)
            text = text.Substring(0, text.IndexOf(','));
        return text;
    }

    private const string True = "true";
    private const string False = "false";

    public static string GetBooleanString(bool b) => b ? True : False;

    public static int GetTextWidth(this string text, Font font)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        var g = Graphics.FromImage(new Bitmap(1, 1));

        return (int)g.MeasureString(text, font).Width;
    }

    public static int GetMaxWidthPixel(List<string> items, Font font)
    {
        var itemMaxLength = "0";
        
        foreach (var item in items.Where(item => item.Length > itemMaxLength.Length))
            itemMaxLength = item;
        
        var g = Graphics.FromImage(new Bitmap(1, 1));

        return (int)g.MeasureString(itemMaxLength, font).Width + 10;
    }

    public static bool ContainsRevitProhibitedSymbols(string text, bool ifTextIsNullOrEmptyResult = false)
    {
        if (string.IsNullOrEmpty(text))
            return ifTextIsNullOrEmptyResult;
            
        var charArray = text.ToCharArray();

        foreach (var c in charArray)
        {
            switch (c)
            {
                case '\\':
                case ':':
                case '{':
                case '}':
                case '[':
                case ']':
                case '|':
                case ';':
                case '<':
                case '>':
                case '?':
                case '`':
                case '~':
                    return true;
            }
        }

        return false;
    }

    public static string RemoveRevitProhibitedSymbols(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        text = text.Replace("\\", "-");
        text = text.Replace(":", "-");
        text = text.Replace("{", "(");
        text = text.Replace("}", ")");
        text = text.Replace("[", "(");
        text = text.Replace("]", ")");
        text = text.Replace("|", "-");
        text = text.Replace(";", ",");
        text = text.Replace("<", " ");
        text = text.Replace(">", " ");
        text = text.Replace("?", " ");
        text = text.Replace("`", " ");
        text = text.Replace("~", "-");

        while (text.Contains("  "))
            text = text.Replace("  ", " ");

        text = text.Trim();

        return text;
    }

    public static bool IsNotNullAndNotEmpty(string value) => !string.IsNullOrEmpty(value);
    
    public static string Join(string separator, params object[] values) =>
        string.Join(separator, values.Select(n => n?.ToString()).Where(n => !string.IsNullOrEmpty(n)));
}