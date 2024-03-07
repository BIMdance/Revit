namespace BIMdance.Revit.Application;

public class AppPaths
{
    public const string AutodeskApplicationPlugins = @"%CommonAppDataFolder%\Autodesk\ApplicationPlugins\";
    public const string UserAutodeskApplicationPlugins = @"%AppData%\Autodesk\ApplicationPlugins\";
    
    private readonly string _applicationName;
    private readonly string _localApplicationData;
    private readonly string _families;
    
    public AppPaths(string applicationName, string revitVersion)
    {
        _applicationName = applicationName;
        _localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        RevitVersion = revitVersion;
        AssemblyDirectory = AssemblyUtils.GetExecutingAssemblyDirectory();
        _families = GetFolder(LocalData, nameof(Families));
        ClearTemp();
    }
    
    public string RevitVersion { get; }
    
    public string AssemblyDirectory { get; }
    public string AppSettings => Path.Combine(Settings, $"{nameof(AppSettings)}.json");
    public string Families    => GetFolder(_families, RevitVersion.ToString());
    public string Images      => GetFolder(LocalData, nameof(Images));
    public string Logs        => GetFolder(LocalData, nameof(Logs));
    public string Settings    => GetFolder(LocalData, nameof(Settings));
    public string LocalData   => GetFolder(_localApplicationData, _applicationName);
    public string Temp        => GetFolder(LocalData, nameof(Temp));
    
    public void ClearTemp() => new DirectoryInfo(Temp)
        .GetFiles("*.*", SearchOption.AllDirectories)
        .ForEach(file => FileUtils.Delete(file, exception => Debug.WriteLine(exception)));
    
    protected static string GetFolder(string parentDirectory, params string[] folderNames)
    {
        var newDirectory = Path.Combine(new [] { parentDirectory }.Union(folderNames).ToArray());
            
        if (!Directory.Exists(newDirectory))
            Directory.CreateDirectory(newDirectory);
            
        return newDirectory;
    }
    
    public void DeleteBackupFamilyFiles(Action<Exception> exceptionAction = null)
    {
        foreach (var file in new DirectoryInfo(Families).GetFiles("*.00*.rfa", SearchOption.AllDirectories))
        {
            if (FileUtils.Revit.IsBackup(file.FullName))
                FileUtils.Delete(file, exceptionAction);
        }
    }

    public string GetUniqueFamilyPath(string path = null)
    {
        var name = $"{NameUtils.GetUniqueName()}.rfa";
        return Path.Combine(path ?? Temp, name);
    }
}