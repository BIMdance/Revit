namespace BIMdance.Revit.Logic.RevitVersions;

internal static class Revit2021
{
    internal static IEnumerable<ElectricalSystem> GetElectricalSystems(
        FamilyInstance familyInstance)
    {
        return familyInstance.MEPModel?.GetElectricalSystems() as IEnumerable<ElectricalSystem> ?? new List<ElectricalSystem>();
    }
}