using BIMdance.Revit.Model.Attributes;

namespace BIMdance.Revit.Model.Electrical.Transformers;

public enum TransformerProperty
{
    [ResourceDescription(
        typeof(Equipment),
        nameof(Equipment.Property_VibrationDampingStands))]
    VibrationDampingStands,
        
    [ResourceDescription(
        typeof(Equipment),
        nameof(Equipment.Property_InstallationSupervision))]
    InstallationSupervision,
        
    [ResourceDescription(
        typeof(Equipment),
        nameof(Equipment.Property_ReducedNoiseLevel))]
    ReducedNoiseLevel,
        
    [ResourceDescription(
        typeof(Equipment),
        nameof(Equipment.Property_ExtendedWarranty))]
    ExtendedWarranty,
        
    [ResourceDescription(
        typeof(Equipment),
        nameof(Equipment.Property_SupplyOfSpareParts))]
    SupplyOfSpareParts,
        
    [ResourceDescription(
        typeof(Equipment),
        nameof(Equipment.Property_MaintenanceContract))]
    MaintenanceContract,
        
    [ResourceDescription(
        typeof(Equipment),
        nameof(Equipment.Property_MarinePackaging))]
    MarinePackaging,
        
    [ResourceDescription(
        typeof(Equipment),
        nameof(Equipment.Property_NameplateInRussian))]
    NameplateInRussian,
        
    [ResourceDescription(
        typeof(Equipment),
        nameof(Equipment.Property_PassportGOST))]
    PassportGost,
        
    [ResourceDescription(
        typeof(Equipment),
        nameof(Equipment.Property_FactoryTestReports))]
    FactoryTestReports,
}