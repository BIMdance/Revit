namespace BIMdance.Revit.Utils;

public static class LookupTableUtils
{
    public static void AddLookupTablesFromFile(this Document familyDocument, string csvFile) =>
        familyDocument.AddLookupTablesFromFiles(new[] { csvFile });
    
    public static void AddLookupTablesFromFiles(this Document familyDocument, IEnumerable<string> csvFiles)
    {
        if (!familyDocument.IsFamilyDocument)
            throw new InvalidOperationException($"Document '{familyDocument.Title}' must be a Family.");
        
        FamilySizeTableManager.CreateFamilySizeTableManager(familyDocument, familyDocument.OwnerFamily.Id);
        var tableManager = FamilySizeTableManager.GetFamilySizeTableManager(familyDocument, familyDocument.OwnerFamily.Id);

        foreach (var csvFile in csvFiles)
        {
            var errorInfo = new FamilySizeTableErrorInfo();
            var importSizeTable = tableManager.ImportSizeTable(familyDocument, csvFile, errorInfo);
            
            if (!importSizeTable)
                Logger.Error(
                    $"{errorInfo.FamilySizeTableErrorType}\n" +
                    $"{nameof(errorInfo.FilePath)}: {errorInfo.FilePath}\n" +
                    $"{nameof(errorInfo.InvalidHeaderText)}: {errorInfo.InvalidHeaderText}\n" +
                    $"{nameof(errorInfo.InvalidRowIndex)}: {errorInfo.InvalidRowIndex}\n" +
                    $"{nameof(errorInfo.InvalidColumnIndex)}: {errorInfo.InvalidColumnIndex}\n");
        }
    }
    
    public static void AddLookupTablesFromContent(this Document document, string tableName, string content) =>
        document.AddLookupTablesFromContents(new Dictionary<string, string> { [tableName] = content });

    public static void AddLookupTablesFromContents(this Document document, Dictionary<string, string> csvContents)
    {
        if (!document.IsFamilyDocument)
            throw new InvalidOperationException($"Document '{document.Title}' must be a Family.");
        
        FamilySizeTableManager.CreateFamilySizeTableManager(document, document.OwnerFamily.Id);
        var tableManager = FamilySizeTableManager.GetFamilySizeTableManager(document, document.OwnerFamily.Id);

        foreach (var csvContent in csvContents)
        {
            var fileName = csvContent.Key;
            var csvFile = Path.Combine(Path.GetTempPath(), $"{fileName}.csv");
            
            try
            {
                var errorInfo = new FamilySizeTableErrorInfo();
                File.WriteAllText(csvFile, csvContent.Value, Encoding.Default); 
                tableManager.ImportSizeTable(document, csvFile, errorInfo);
                Console.WriteLine(errorInfo.FamilySizeTableErrorType);
            }
            finally
            {
                if (File.Exists(csvFile))
                    File.Delete(csvFile);
            }
        }
    }
    
    public static void AddLookupTablesFromContent(this Document document, string familyName, string tableName, string content) =>
        document.AddLookupTablesFromContents(familyName, new Dictionary<string, string> { [tableName] = content });

    public static void AddLookupTablesFromContents(this Document document, string familyName, Dictionary<string, string> csvContents)
    {
        var family = document.GetFamilyByName(familyName);
        FamilySizeTableManager.CreateFamilySizeTableManager(document, family.Id);
        var tableManager = FamilySizeTableManager.GetFamilySizeTableManager(document, family.Id);

        foreach (var csvContent in csvContents)
        {
            var fileName = csvContent.Key;
            var csvFile = Path.Combine(Path.GetTempPath(), $"{fileName}.csv");
            
            try
            {
                var errorInfo = new FamilySizeTableErrorInfo();
                File.WriteAllText(csvFile, csvContent.Value, Encoding.Default);
                tableManager.ImportSizeTable(document, csvFile, errorInfo);
                Console.WriteLine(errorInfo.FamilySizeTableErrorType);
            }
            finally
            {
                if (File.Exists(csvFile))
                    File.Delete(csvFile);
            }
        }
    }
}