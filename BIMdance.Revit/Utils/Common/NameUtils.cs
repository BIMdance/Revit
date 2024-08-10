namespace BIMdance.Revit.Utils.Common;

public static class NameUtils
{
    public static string GetNextName(string lastName, IReadOnlyCollection<string> otherNames = null)
    {
        if (lastName == null)
            return null;

        lastName = lastName.Trim();
        
        if (otherNames != null && !otherNames.Contains(lastName))
            return lastName;

        var basis = lastName.ToLastInt();
        var separator = basis.EndsWith(" ") ? " " : string.Empty;

        var suffixes = (otherNames ?? new List<string>())
            .Concat(new [] {lastName})
            .Where(n => n.StartsWith(basis))
            .Select(n => n.Substring(basis.Length))
            .Where(n => int.TryParse(n, out _))
            .Select(int.Parse)
            .ToList();
            
        return suffixes.Any()
            ? $"{basis.Trim()}{separator}{suffixes.Max() + 1}"
            : $"{basis.Trim()} {2}";
    }

    public static string GetUniqueName() => Guid.NewGuid().ToString().Split('-').Last();
}