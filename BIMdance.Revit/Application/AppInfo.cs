namespace BIMdance.Revit.Application;

public class AppInfo
{
    public AppInfo(string applicationName, bool isRevitStarted = false, string revitVersion = null)
    {
        ApplicationName = applicationName;
        CurrentSessionGuid = Guid.NewGuid();
        IsRevitStarted = isRevitStarted;
        RevitVersion = revitVersion;
        Version = AssemblyUtils.GetApplicationVersion().ToString();
    }

    public string ApplicationName { get; }
    public Guid CurrentSessionGuid { get; }
    public bool IsRevitStarted { get; }
    public string RevitVersion { get; set; }
    public string Version { get; }
}