namespace BIMdance.Revit.Utils;

public static class FileUtils
{
    public static void CreateDirectoryIfNotExist(string path)
    {
        var directory = Path.GetDirectoryName(path);

        if (directory is not null && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);
    }
    
    public static void Delete(FileInfo file, Action<Exception> exceptionAction = null) =>
        Delete(file.FullName, exceptionAction);

    public static void Delete(string path, Action<Exception> exceptionAction = null)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(path) &&
                File.Exists(path) && !IsFileLocked(path))
                File.Delete(path);
        }
        catch (Exception exception)
        {
            exceptionAction?.Invoke(exception);
        }
    }
    
    public static void Rename(FileInfo file, string newName, Action<Exception> exceptionAction = null) =>
        Rename(file.FullName, newName, exceptionAction);

    public static string Rename(string path, string newName, Action<Exception> exceptionAction = null)
    {
        try
        {
            var directoryName = Path.GetDirectoryName(path) ?? throw new DirectoryNotFoundException(path);
            var newExtension = Path.GetExtension(newName);
            
            if (string.IsNullOrWhiteSpace(newExtension))
                newExtension = Path.GetExtension(path);
            
            var newPath = Path.Combine(directoryName, Path.ChangeExtension(newName, newExtension));
            
            if (!string.IsNullOrWhiteSpace(path) &&
                File.Exists(path) && !IsFileLocked(path))
                File.Move(path, newPath);

            return newPath;
        }
        catch (Exception exception)
        {
            exceptionAction?.Invoke(exception);
            return null;
        }
    }

    public static bool IsFileLocked(string path) => IsFileLocked(new FileInfo(path));

    public static bool IsFileLocked(FileInfo file)
    {
        try
        {
            using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            stream.Close();
        }
        catch (IOException)
        {
            return true;
        }

        return false;
    }

    public static void WriteAllTextIfDifferent(string path, string content) =>
        WriteAllTextIfDifferent(path, content, Encoding.Default);
    
    public static void WriteAllTextIfDifferent(string path, string content, Encoding encoding)
    {
        if (File.Exists(path))
        {
            var newContentHashCode = content.GetHashCode();
            var oldContentHashCode = File.ReadAllText(path, encoding).GetHashCode();
            
            if (newContentHashCode == oldContentHashCode)
                return;
        }
        
        CreateDirectoryIfNotExist(path);
        File.WriteAllText(path, content, encoding);
    }

    public static class Revit
    {
        public static bool IsBackup(string fileName)
        {
            var regex = new Regex(@"\.\d\d\d\d((.rfa)|(.rvt))$");
            var match = regex.Match(fileName);
            return match.Captures.Count > 0;
        }
    }
}