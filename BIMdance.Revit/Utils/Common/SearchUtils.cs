namespace BIMdance.Revit.Utils.Common;

public class SearchUtils
{
    private static readonly char[] Separators = {' ', ',', ';', '.', ':'};
    
    public static string[] SearchWords(string searchText) => 
        searchText.Split(Separators).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
}