namespace BIMdance.Revit.Application;

public class AppInfo
{
    public AppInfo(string applicationName, string revitVersion = null)
    {
        ApplicationName = applicationName;
        CurrentSessionGuid = Guid.NewGuid();
        RevitVersion = revitVersion;
        Version = AssemblyUtils.GetApplicationVersion().ToString();
    }

    public string ApplicationName { get; }
    public Guid CurrentSessionGuid { get; }
    public string RevitVersion { get; set; }
    public string Version { get; }
}