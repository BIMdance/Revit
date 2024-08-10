// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using System.Linq;

namespace BIMdance.Revit.ObsoleteAPI;

public static partial class Revit2016
{
    public class MEPModel
    {
        public static IEnumerable<ElectricalSystem> GetElectricalSystems(FamilyInstance familyInstance) =>
            familyInstance.MEPModel?.ElectricalSystems?.OfType<ElectricalSystem>() ??
            new List<ElectricalSystem>();

        public static IEnumerable<ElectricalSystem> GetAssignedElectricalSystems(FamilyInstance familyInstance) =>
            familyInstance.MEPModel?.AssignedElectricalSystems?.OfType<ElectricalSystem>() ??
            new List<ElectricalSystem>();

        public static IEnumerable<ElectricalSystem> GetElectricalSystems(Autodesk.Revit.DB.MEPModel mepModel) =>
            mepModel?.ElectricalSystems?.OfType<ElectricalSystem>() ??
            new List<ElectricalSystem>();

        public static IEnumerable<ElectricalSystem> GetAssignedElectricalSystems(Autodesk.Revit.DB.MEPModel mepModel) =>
            mepModel?.AssignedElectricalSystems?.OfType<ElectricalSystem>() ??
            new List<ElectricalSystem>();
    }
}