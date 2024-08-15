namespace BIMdance.Revit.Utils.Revit.Electrical;

public static class DistributionSysTypeExtension
{
    public static double GetInternalVoltageLineToGround(this DistributionSysType distributionSystem) =>
        distributionSystem?.VoltageLineToGround?.get_Parameter(BuiltInParameter.RBS_VOLTAGETYPE_VOLTAGE_PARAM)?.AsDouble() ??
        distributionSystem?.VoltageLineToLine?.get_Parameter(BuiltInParameter.RBS_VOLTAGETYPE_VOLTAGE_PARAM)?.AsDouble() / MathConstants.Sqrt3 ??
        0;
    
    public static double GetInternalVoltageLineToLine(this DistributionSysType distributionSystem) =>
        distributionSystem?.VoltageLineToLine?.get_Parameter(BuiltInParameter.RBS_VOLTAGETYPE_VOLTAGE_PARAM)?.AsDouble() ??
        distributionSystem?.VoltageLineToGround?.get_Parameter(BuiltInParameter.RBS_VOLTAGETYPE_VOLTAGE_PARAM)?.AsDouble() * MathConstants.Sqrt3 ??
        0;
}