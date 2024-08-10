namespace BIMdance.Revit.Utils;

public class RevitOptions
{
    public static readonly SaveAsOptions OverwriteSaveAs = new() { Compact = true, OverwriteExistingFile = true, MaximumBackups = 1 };
    public static IFamilyLoadOptions FamilyOverwriteFamilyLoad { get; } = new OverwriteLoadOption(FamilySource.Family);
    public static IFamilyLoadOptions ProjectOverwriteFamilyLoad { get; } = new OverwriteLoadOption(FamilySource.Project);
}

public class OverwriteLoadOption : IFamilyLoadOptions
{
    private readonly FamilySource _familySource;

    public OverwriteLoadOption(FamilySource familySource) => _familySource = familySource;

    public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
    {
        overwriteParameterValues = true;
        return true;
    }

    public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
    {
        source = _familySource;
        overwriteParameterValues = true;
        return true;
    }
}