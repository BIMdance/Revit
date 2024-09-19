namespace BIMdance.Revit.Logic.Utils;

public static class ModelNameUtils
{
    public static Func<BuiltInCategoryProxy, string> GetCategoryNameFunc { private get; set; }
    public static Func<ElectricalSystemTypeProxy, string> GetSystemTypeNameFunc { private get; set; }

    public static string GetName(BuiltInCategoryProxy category) => GetCategoryNameFunc?.Invoke(category) ?? category switch
    {
        BuiltInCategoryProxy.INVALID                  => BuiltInCategories.Invalid,
        BuiltInCategoryProxy.OST_CommunicationDevices => BuiltInCategories.CommunicationDevices,
        BuiltInCategoryProxy.OST_DataDevices          => BuiltInCategories.DataDevices,
        BuiltInCategoryProxy.OST_ElectricalEquipment  => BuiltInCategories.ElectricalEquipment,
        BuiltInCategoryProxy.OST_ElectricalFixtures   => BuiltInCategories.ElectricalFixtures,
        BuiltInCategoryProxy.OST_FireAlarmDevices     => BuiltInCategories.FireAlarmDevices,
        BuiltInCategoryProxy.OST_LightingDevices      => BuiltInCategories.LightingDevices,
        BuiltInCategoryProxy.OST_LightingFixtures     => BuiltInCategories.LightingFixtures,
        BuiltInCategoryProxy.OST_MechanicalEquipment  => BuiltInCategories.MechanicalEquipment,
        BuiltInCategoryProxy.OST_NurseCallDevices     => BuiltInCategories.NurseCallDevices,
        BuiltInCategoryProxy.OST_SecurityDevices      => BuiltInCategories.SecurityDevices,
        BuiltInCategoryProxy.OST_SpecialityEquipment  => BuiltInCategories.SecurityDevices,
        BuiltInCategoryProxy.OST_TelephoneDevices     => BuiltInCategories.TelephoneDevices,
        _ => category.ToString()
        // _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
    };
    
    public static string GetName(ElectricalSystemTypeProxy systemType) => GetSystemTypeNameFunc?.Invoke(systemType) ?? systemType switch
    {
        ElectricalSystemTypeProxy.UndefinedSystemType => ElectricalSystemTypes.Undefined,
        ElectricalSystemTypeProxy.Data                => ElectricalSystemTypes.Data,
        ElectricalSystemTypeProxy.PowerCircuit        => ElectricalSystemTypes.PowerCircuit,
        ElectricalSystemTypeProxy.Telephone           => ElectricalSystemTypes.Telephone,
        ElectricalSystemTypeProxy.Security            => ElectricalSystemTypes.Security,
        ElectricalSystemTypeProxy.FireAlarm           => ElectricalSystemTypes.FireAlarm,
        ElectricalSystemTypeProxy.NurseCall           => ElectricalSystemTypes.NurseCall,
        ElectricalSystemTypeProxy.Controls            => ElectricalSystemTypes.Controls,
        ElectricalSystemTypeProxy.Communication       => ElectricalSystemTypes.Communication,
        ElectricalSystemTypeProxy.PowerBalanced       => ElectricalSystemTypes.PowerBalanced,
        ElectricalSystemTypeProxy.PowerUnBalanced     => ElectricalSystemTypes.PowerUnbalanced,
        _ => systemType.ToString()
        // _ => throw new ArgumentOutOfRangeException(nameof(systemType), systemType, null)
    };
}