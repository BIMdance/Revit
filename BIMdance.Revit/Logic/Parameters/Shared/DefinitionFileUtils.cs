// ReSharper disable InconsistentNaming

namespace BIMdance.Revit.Logic.Parameters.Shared;

public class DefinitionFileUtils : IDisposable
{
    private const string PARAM = "PARAM";
    private const string GROUP = "GROUP";
    private readonly string _path;
    private Dictionary<int, string> _groups;
    private Dictionary<Guid, (string Name, int GroupId)> _parametersNames;

    public DefinitionFileUtils(string path, byte[] definitionFile)
    {
        _path = path;
        DeleteFileIfExists(_path);
        File.WriteAllBytes(_path, definitionFile);
        PullSharedParameters();
    }

    public void AddSharedParameter(Guid guid, string name, string type, string description, bool visible = true, bool userModifiable = true)
    {
        File.AppendAllLines(_path, new[]
        {
            $"{PARAM}\t{guid}\t{name}\t{type}\t\t{1}\t{(visible ? 1 : 0)}\t{description}\t{(userModifiable ? 1 : 0)}",
        }, Encoding.Unicode);
        
        _parametersNames.Add(guid, (name, 1));
    }

    private void PullSharedParameters()
    {
        var content = File.ReadAllLines(_path);
        var rowIndex = 0;

        _groups = new Dictionary<int, string>();
        _parametersNames = new Dictionary<Guid, (string, int)>();

        foreach (var line in content)
        {
            rowIndex++;

            if (line.StartsWith(GROUP))
                PullGroupFromFile(line, rowIndex);

            if (line.StartsWith(PARAM))
                PullSharedParameterFromFile(line, rowIndex);
        }
    }

    private void PullGroupFromFile(string line, int rowIndex)
    {
        var values = line.Split('\t');

        if (values.Length < 3)
            throw new InvalidDataException($"[line:{rowIndex}] {GROUP} has invalid format.");

        if (!int.TryParse(values[1], out var id))
            throw new InvalidCastException($"{nameof(values)}[1] is not a {nameof(Int32)}");

        var name = values[2];

        _groups.Add(id, name);
    }

    private void PullSharedParameterFromFile(string line, int rowIndex)
    {
        var values = line.Split('\t');

        if (values.Length < 9)
            throw new InvalidDataException($"[line:{rowIndex}]: {PARAM} has invalid format.");

        if (!Guid.TryParse(values[1], out var guid))
            throw new InvalidCastException($"[line:{rowIndex}]: {nameof(values)}[1] = {values[1]} is not a {nameof(Guid)}");

        if (_parametersNames.ContainsKey(guid))
        {
            Logger.Error($"[{guid}] is contained more than once in the DefinitionFile: {_path}");
            return;
        }

        if (!int.TryParse(values[5], out var groupIndex))
        {
            Logger.Error($"[line:{rowIndex}]: {nameof(values)}[5] = {values[5]} is not a {nameof(Int32)}");
            return;
        }

        if (!_groups.TryGetValue(groupIndex, out _))
        {
            Logger.Error($"[line:{rowIndex}]: {nameof(groupIndex)} = {groupIndex}");
            return;
        }

        if (!int.TryParse(values[6], out _))
        {
            Logger.Error($"[line:{rowIndex}]: {nameof(values)}[6] = {values[6]} is not a {nameof(Int32)}");
            return;
        }

        if (!int.TryParse(values[8], out _))
        {
            Logger.Error($"[line:{rowIndex}]: {nameof(values)}[8] = {values[8]} is not a {nameof(Int32)}");
            return;
        }
        
        var name = values[2];
        var groupId = int.TryParse(values[5], out var id) ? id : -1;

        _parametersNames.Add(guid, (name, groupId));
    }

    public string GetGroupName(Guid guid) =>
        _parametersNames.TryGetValue(guid, out var parameter) && _groups.TryGetValue(parameter.GroupId, out var groupName)
            ? groupName
            : null;

    public string GetParameterName(Guid guid) =>
        _parametersNames.TryGetValue(guid, out var parameter)
            ? parameter.Name
            : null;

    public void Dispose() => DeleteFileIfExists(_path);

    private void DeleteFileIfExists(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch (Exception exception)
        {
            Logger.Error(exception);
        }
    }
}