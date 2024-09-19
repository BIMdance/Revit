namespace BIMdance.Revit.Utils.Common;

public static class DeclensionUtils
{
    /// <summary>
    /// Возвращает слова в падеже, зависимом от заданного числа 
    /// </summary>
    /// <param name="number">Число от которого зависит выбранное слово</param>
    /// <param name="nominativ">Именительный падеж слова. Например "день"</param>
    /// <param name="genetiv">Родительный падеж слова. Например "дня"</param>
    /// <param name="plural">Множественное число слова. Например "дней"</param>
    /// <returns></returns>
    public static string GetDeclension(int number, string nominativ, string genetiv, string plural)
    {
        var twoLastDigits = number % 100;

        if (twoLastDigits is >= 11 and <= 19)
        {
            return plural;
        }

        var lastDigit = twoLastDigits % 10;

        return lastDigit switch
        {
            1 => nominativ,
            2 => genetiv,
            3 => genetiv,
            4 => genetiv,
            _ => plural
        };
    }
}