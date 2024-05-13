namespace BIMdance.Revit.Logic;

public static class SharedParametersLogic
{
    public static void AddSharedParameters(Document document)
    {
        var circuitDesignationDefinition = new BaseSharedParameterDefinition(
            SharedParameterGuids.Circuit_Designation,
            ParameterGroupProxy.PG_CONSTRAINTS,
            isInstance: true);
        
        var circuitTopologyDefinition = new BaseSharedParameterDefinition(
            SharedParameterGuids.Circuit_Topology,
            ParameterGroupProxy.PG_CONSTRAINTS,
            isInstance: true);
        
        var cableDesignationDefinition = new BaseSharedParameterDefinition(
            SharedParameterGuids.Cable_Designation,
            ParameterGroupProxy.PG_CONSTRAINTS,
            isInstance: true);
        
        var cableDiameterDefinition = new BaseSharedParameterDefinition(
            SharedParameterGuids.Cable_Diameter,
            ParameterGroupProxy.PG_CONSTRAINTS,
            isInstance: true);
        
        var cableLengthDefinition = new BaseSharedParameterDefinition(
            SharedParameterGuids.Cable_Length,
            ParameterGroupProxy.PG_CONSTRAINTS,
            isInstance: true);
        
        var cableLengthMaxDefinition = new BaseSharedParameterDefinition(
            SharedParameterGuids.Cable_LengthMax,
            ParameterGroupProxy.PG_CONSTRAINTS,
            isInstance: true);
        
        var cableLengthInCableTrayDefinition = new BaseSharedParameterDefinition(
            SharedParameterGuids.Cable_Length_InCableTray,
            ParameterGroupProxy.PG_CONSTRAINTS,
            isInstance: true);
        
        var cableLengthOutsideCableTrayDefinition = new BaseSharedParameterDefinition(
            SharedParameterGuids.Cable_Length_OutsideCableTray,
            ParameterGroupProxy.PG_CONSTRAINTS,
            isInstance: true);
        
        var cablesCountDefinition = new BaseSharedParameterDefinition(
            SharedParameterGuids.Cables_Count,
            ParameterGroupProxy.PG_CONSTRAINTS,
            isInstance: true);
        
        var cableTraceDefinition = new BaseSharedParameterDefinition(
            SharedParameterGuids.CableTrace,
            ParameterGroupProxy.PG_ELECTRICAL,
            isInstance: true);
        
        var cableTrayFillingDefinition = new BaseSharedParameterDefinition(
            SharedParameterGuids.CableTray_Filling,
            ParameterGroupProxy.PG_ELECTRICAL,
            isInstance: true);
        
        var cableTrayFillingPercentDefinition = new BaseSharedParameterDefinition(
            SharedParameterGuids.CableTray_FillingPercent,
            ParameterGroupProxy.PG_ELECTRICAL,
            isInstance: true);
        
        var cableTrayConduitIdsDefinition = new BaseSharedParameterDefinition(
            SharedParameterGuids.CableTrayConduitIds,
            ParameterGroupProxy.PG_IDENTITY_DATA,
            isInstance: true);
        
        var electricalSystemIdsDefinition = new BaseSharedParameterDefinition(
            SharedParameterGuids.ElectricalSystemIds,
            ParameterGroupProxy.PG_IDENTITY_DATA,
            isInstance: true);
        
        var serviceTypeDefinition = new BaseSharedParameterDefinition(
            SharedParameterGuids.ServiceType,
            ParameterGroupProxy.PG_IDENTITY_DATA,
            isInstance: true);
        
        using var sharedParameterUtils = new SharedParametersUtils(document);
        
        sharedParameterUtils.AddSharedParametersToProject(new List<ProjectSharedParameter>
        {
            new(circuitDesignationDefinition,          BuiltInCategory.OST_ElectricalCircuit),
            new(circuitTopologyDefinition,             BuiltInCategory.OST_ElectricalCircuit),
            new(cableDesignationDefinition,            BuiltInCategory.OST_ElectricalCircuit),
            new(cableDiameterDefinition,               BuiltInCategory.OST_ElectricalCircuit),
            new(cableLengthDefinition,                 BuiltInCategory.OST_ElectricalCircuit),
            new(cableLengthMaxDefinition,              BuiltInCategory.OST_ElectricalCircuit),
            new(cableLengthInCableTrayDefinition,      BuiltInCategory.OST_ElectricalCircuit),
            new(cableLengthOutsideCableTrayDefinition, BuiltInCategory.OST_ElectricalCircuit),
            new(cablesCountDefinition,                 BuiltInCategory.OST_ElectricalCircuit),
            
            new(cableTraceDefinition,              CableTrayConduitCategories),
            // new(cableTrayFillingDefinition,        CableTrayConduitCategories),
            // new(cableTrayFillingPercentDefinition, CableTrayConduitCategories),
            
            new(cableTrayConduitIdsDefinition, ElectricalDevicesCategories),
            new(electricalSystemIdsDefinition, ElectricalCategories),
            new(serviceTypeDefinition,         ElectricalDevicesCategories),
        });
    }

    private static readonly List<BuiltInCategory> CableTrayConduitCategories = new()
    {
        BuiltInCategory.OST_CableTray,
        BuiltInCategory.OST_CableTrayFitting,
        BuiltInCategory.OST_Conduit,
        BuiltInCategory.OST_ConduitFitting
    };
    
    private static readonly List<BuiltInCategory> ElectricalDevicesCategories = new()
    {
        BuiltInCategory.OST_ElectricalEquipment,
        BuiltInCategory.OST_ElectricalFixtures,
        BuiltInCategory.OST_CommunicationDevices,
        BuiltInCategory.OST_DataDevices,
        BuiltInCategory.OST_FireAlarmDevices,
        BuiltInCategory.OST_GenericModel,
        BuiltInCategory.OST_LightingDevices,
        BuiltInCategory.OST_LightingFixtures,
        BuiltInCategory.OST_MechanicalEquipment,
        BuiltInCategory.OST_NurseCallDevices,
        BuiltInCategory.OST_SecurityDevices,
        BuiltInCategory.OST_TelephoneDevices,
    };

    private static readonly List<BuiltInCategory> ElectricalCategories = new()
    {
        BuiltInCategory.OST_CableTray,
        BuiltInCategory.OST_CableTrayFitting,
        BuiltInCategory.OST_Conduit,
        BuiltInCategory.OST_ConduitFitting,
        BuiltInCategory.OST_ElectricalEquipment,
        BuiltInCategory.OST_ElectricalFixtures,
        BuiltInCategory.OST_CommunicationDevices,
        BuiltInCategory.OST_DataDevices,
        BuiltInCategory.OST_FireAlarmDevices,
        BuiltInCategory.OST_GenericModel,
        BuiltInCategory.OST_LightingDevices,
        BuiltInCategory.OST_LightingFixtures,
        BuiltInCategory.OST_MechanicalEquipment,
        BuiltInCategory.OST_NurseCallDevices,
        BuiltInCategory.OST_SecurityDevices,
        BuiltInCategory.OST_TelephoneDevices,
    };
}