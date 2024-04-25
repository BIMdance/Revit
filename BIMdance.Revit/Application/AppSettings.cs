namespace BIMdance.Revit.Application;

public class AppSettings
{
    private readonly AppPaths _appPaths;
    
    public AppSettings() { }
    public AppSettings(AppPaths appPaths)
    {
        _appPaths = appPaths;
        Load();
    }
    
    public bool IsUpdatersEnabled { get; set; } = true;

    public void Load()
    {
        if (!JsonUtils.TryLoad<AppSettings>(_appPaths.AppSettings, out var prototype))
            return;

        IsUpdatersEnabled = prototype.IsUpdatersEnabled;
    }
    
    public void Save() => JsonUtils.Save(_appPaths.AppSettings, this);
}