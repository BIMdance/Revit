using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;

namespace BIMdance.Revit.ObsoleteAPI;

public class MEPModelRevit2016
{
    public static IEnumerable<ElectricalSystem> GetElectricalSystems(FamilyInstance familyInstance)
    {
        return familyInstance.MEPModel?.ElectricalSystems?.OfType<ElectricalSystem>() ??
               new List<ElectricalSystem>();
    }

    public static IEnumerable<ElectricalSystem> GetAssignedElectricalSystems(FamilyInstance familyInstance)
    {
        return familyInstance.MEPModel?.AssignedElectricalSystems?.OfType<ElectricalSystem>() ??
               new List<ElectricalSystem>();
    }

    public static IEnumerable<ElectricalSystem> GetElectricalSystems(MEPModel mepModel)
    {
        return mepModel?.ElectricalSystems?.OfType<ElectricalSystem>() ??
               new List<ElectricalSystem>();
    }

    public static IEnumerable<ElectricalSystem> GetAssignedElectricalSystems(MEPModel mepModel)
    {
        return mepModel?.AssignedElectricalSystems?.OfType<ElectricalSystem>() ??
               new List<ElectricalSystem>();
    }
}