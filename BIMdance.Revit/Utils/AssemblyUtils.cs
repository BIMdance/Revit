namespace BIMdance.Revit.Utils;

public static class AssemblyUtils
{
    public static Version GetApplicationVersion() =>
        new(GetApplicationFileVersionInfo().ProductVersion);

    public static FileVersionInfo GetApplicationFileVersionInfo() =>
        FileVersionInfo.GetVersionInfo(GetExecutingAssemblyPath());

    public static string GetExecutingAssemblyDirectory() =>
        Path.GetDirectoryName(GetExecutingAssemblyPath());

    public static string GetExecutingAssemblyPath()
    {
        var assembly = Assembly.GetCallingAssembly();
        var filePath = assembly.CodeBase.Replace(@"file:///", string.Empty);
        
        return filePath;
    }
    
    public static DirectoryInfo GetSolutionDirectoryInfo(string currentPath = null)
    {
        var directory = new DirectoryInfo(currentPath ?? GetExecutingAssemblyDirectory());
            
        while (directory != null && !directory.GetFiles("*.sln").Any())
            directory = directory.Parent;

        return directory;
    }
    
    public static void LoadProjectAssemblies(params string[] assemblyNames)
    {
        var assemblyDirectory = Path.GetDirectoryName(AssemblyUtils.GetExecutingAssemblyPath());

        if (string.IsNullOrWhiteSpace(assemblyDirectory))
        {
            Logger.Warn($"{typeof(AssemblyUtils).FullName}.{nameof(LoadProjectAssemblies)}: Current assembly directory was not defined.");
            return;
        }

        foreach (var assemblyName in assemblyNames)
        {
            try
            {
                var resourcesAssemblyPath = Path.Combine(assemblyDirectory, assemblyName);
            
                if (File.Exists(resourcesAssemblyPath))
                    Assembly.LoadFrom(resourcesAssemblyPath);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }
    }
}