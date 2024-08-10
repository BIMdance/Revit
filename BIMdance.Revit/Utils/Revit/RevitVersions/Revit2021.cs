// ReSharper disable InconsistentNaming

namespace BIMdance.Revit.Utils.Revit.RevitVersions;

internal static class Revit2021
{
    internal static class MEPModel
    {
        internal static IEnumerable<ElectricalSystem> GetElectricalSystems(
            FamilyInstance familyInstance) =>
            familyInstance.MEPModel?.GetElectricalSystems() ?? new List<ElectricalSystem>() as IEnumerable<ElectricalSystem>;

        internal static IEnumerable<ElectricalSystem> GetAssignedElectricalSystems(
            FamilyInstance familyInstance) =>
            familyInstance.MEPModel?.GetAssignedElectricalSystems() ?? new List<ElectricalSystem>() as IEnumerable<ElectricalSystem>;

        internal static IEnumerable<ElectricalSystem> GetElectricalSystems(
            Autodesk.Revit.DB.MEPModel mepModel) =>
            mepModel?.GetElectricalSystems() ?? new List<ElectricalSystem>() as IEnumerable<ElectricalSystem>;

        internal static IEnumerable<ElectricalSystem> GetAssignedElectricalSystems(
            Autodesk.Revit.DB.MEPModel mepModel) =>
            mepModel?.GetAssignedElectricalSystems() ?? new List<ElectricalSystem>() as IEnumerable<ElectricalSystem>;
    }
}