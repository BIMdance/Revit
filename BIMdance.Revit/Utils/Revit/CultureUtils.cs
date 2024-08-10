namespace BIMdance.Revit.Utils.Revit;

public static class CultureUtils
{
    public static void SetCurrentCulture(LanguageType languageType)
    {
        if (!Enum.IsDefined(typeof(LanguageType), languageType))
        {
            throw new ArgumentException(nameof(languageType));
        }

        var language = languageType switch
        {
            LanguageType.Unknown => "en",
            LanguageType.English_GB => "en-GB",
            LanguageType.English_USA => "en-US",
            LanguageType.German => "de-DE",
            LanguageType.Spanish => "es-ES",
            LanguageType.French => "fr-FR",
            LanguageType.Italian => "it-IT",
            LanguageType.Dutch => "nl-BE",
            LanguageType.Chinese_Simplified => "zh-CHS",
            LanguageType.Chinese_Traditional => "zh-CHT",
            LanguageType.Japanese => "ja-JP",
            LanguageType.Korean => "ko-KR",
            LanguageType.Russian => "ru-RU",
            LanguageType.Czech => "cs-CZ",
            LanguageType.Polish => "pl-PL",
            LanguageType.Hungarian => "hu-HU",
            LanguageType.Brazilian_Portuguese => "pt-BR",
            _ => "en-US"
        };

        var culture = new CultureInfo(language);
            
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }
}