// ReSharper disable InconsistentNaming
namespace BIMdance.Revit.Model.RevitProxy;

public enum UnitTypeProxy
{
    /// <summary>Undefined unit value</summary>
    UT_Undefined = -2, // 0xFFFFFFFE
    /// <summary>A custom unit value</summary>
    UT_Custom = -1, // 0xFFFFFFFF
    /// <summary>Length, e.g. ft, in, m, mm</summary>
    UT_Length = 0,
    /// <summary>Area, e.g. ftÂ², inÂ², mÂ², mmÂ²</summary>
    UT_Area = 1,
    /// <summary>Volume, e.g. ftÂ³, inÂ³, mÂ³, mmÂ³</summary>
    UT_Volume = 2,
    /// <summary>Angular measurement, e.g. radians, degrees</summary>
    UT_Angle = 3,
    /// <summary>General format unit, appropriate for general counts or percentages</summary>
    UT_Number = 4,
    /// <summary>Sheet length</summary>
    UT_SheetLength = 5,
    /// <summary>Site angle</summary>
    UT_SiteAngle = 6,
    /// <summary>Density (HVAC) e.g.	kg/mÂ³</summary>
    UT_HVAC_Density = 7,
    /// <summary>Energy (HVAC) e.g.	(mÂ² Â· kg)/sÂ², J</summary>
    UT_HVAC_Energy = 8,
    /// <summary>Friction (HVAC) e.g. kg/(mÂ² Â· sÂ²), Pa/m</summary>
    UT_HVAC_Friction = 9,
    /// <summary>Power (HVAC) e.g. (mÂ² Â· kg)/sÂ³, W</summary>
    UT_HVAC_Power = 10, // 0x0000000A
    /// <summary>Power Density (HVAC), e.g. kg/sÂ³, W/mÂ²</summary>
    UT_HVAC_Power_Density = 11, // 0x0000000B
    /// <summary>Pressure (HVAC) e.g. kg/(m Â· sÂ²), Pa</summary>
    UT_HVAC_Pressure = 12, // 0x0000000C
    /// <summary>Temperature (HVAC) e.g. K, C, F</summary>
    UT_HVAC_Temperature = 13, // 0x0000000D
    /// <summary>Velocity (HVAC) e.g. m/s</summary>
    UT_HVAC_Velocity = 14, // 0x0000000E
    /// <summary>Air Flow (HVAC) e.g. mÂ³/s</summary>
    UT_HVAC_Airflow = 15, // 0x0000000F
    /// <summary>Duct Size (HVAC) e.g. mm, in</summary>
    UT_HVAC_DuctSize = 16, // 0x00000010
    /// <summary>Cross Section (HVAC) e.g. mmÂ², inÂ²</summary>
    UT_HVAC_CrossSection = 17, // 0x00000011
    /// <summary>Heat Gain (HVAC) e.g. (mÂ² Â· kg)/sÂ³, W</summary>
    UT_HVAC_HeatGain = 18, // 0x00000012
    /// <summary>Current (Electrical) e.g. A</summary>
    UT_Electrical_Current = 19, // 0x00000013
    /// <summary>Electrical Potential e.g.	(mÂ² Â· kg) / (sÂ³Â· A), V</summary>
    UT_Electrical_Potential = 20, // 0x00000014
    /// <summary>Frequency (Electrical) e.g. 1/s, Hz</summary>
    UT_Electrical_Frequency = 21, // 0x00000015
    /// <summary>Illuminance (Electrical) e.g. (cd Â· sr)/mÂ², lm/mÂ²</summary>
    UT_Electrical_Illuminance = 22, // 0x00000016
    /// <summary>Luminous Flux (Electrical) e.g. cd Â· sr, lm</summary>
    UT_Electrical_Luminous_Flux = 23, // 0x00000017
    /// <summary>Power (Electrical) e.g.	(mÂ² Â· kg)/sÂ³, W</summary>
    UT_Electrical_Power = 24, // 0x00000018
    /// <summary>Roughness factor (HVAC) e,g. ft, in, mm</summary>
    UT_HVAC_Roughness = 25, // 0x00000019
    /// <summary>Force, e.g. (kg Â· m)/sÂ², N</summary>
    UT_Force = 26, // 0x0000001A
    /// <summary>Force per unit length, e.g. kg/sÂ², N/m</summary>
    UT_LinearForce = 27, // 0x0000001B
    /// <summary>Force per unit area, e.g. kg/(m Â· sÂ²), N/mÂ²</summary>
    UT_AreaForce = 28, // 0x0000001C
    /// <summary>Moment, e.g. (kg Â· mÂ²)/sÂ², N Â· m</summary>
    UT_Moment = 29, // 0x0000001D
    /// <summary>Force scale, e.g. m / N</summary>
    UT_ForceScale = 30, // 0x0000001E
    /// <summary>Linear force scale, e.g. mÂ² / N</summary>
    UT_LinearForceScale = 31, // 0x0000001F
    /// <summary>Area force scale, e.g. mÂ³ / N</summary>
    UT_AreaForceScale = 32, // 0x00000020
    /// <summary>Moment scale, e.g. 1 / N</summary>
    UT_MomentScale = 33, // 0x00000021
    /// <summary>Apparent Power (Electrical), e.g. (mÂ² Â· kg)/sÂ³, W</summary>
    UT_Electrical_Apparent_Power = 34, // 0x00000022
    /// <summary>Power Density (Electrical), e.g. kg/sÂ³, W/mÂ²</summary>
    UT_Electrical_Power_Density = 35, // 0x00000023
    /// <summary>Density (Piping) e.g. kg/mÂ³</summary>
    UT_Piping_Density = 36, // 0x00000024
    /// <summary>Flow (Piping), e.g. mÂ³/s</summary>
    UT_Piping_Flow = 37, // 0x00000025
    /// <summary>Friction (Piping), e.g. kg/(mÂ² Â· sÂ²), Pa/m</summary>
    UT_Piping_Friction = 38, // 0x00000026
    /// <summary>Pressure (Piping), e.g. kg/(m Â· sÂ²), Pa</summary>
    UT_Piping_Pressure = 39, // 0x00000027
    /// <summary>Temperature (Piping), e.g. K</summary>
    UT_Piping_Temperature = 40, // 0x00000028
    /// <summary>Velocity (Piping), e.g. m/s</summary>
    UT_Piping_Velocity = 41, // 0x00000029
    /// <summary>Dynamic Viscosity (Piping), e.g. kg/(m Â· s), Pa Â· s</summary>
    UT_Piping_Viscosity = 42, // 0x0000002A
    /// <summary>Pipe Size (Piping), e.g.	m</summary>
    UT_PipeSize = 43, // 0x0000002B
    /// <summary>Roughness factor (Piping), e.g. ft, in, mm</summary>
    UT_Piping_Roughness = 44, // 0x0000002C
    /// <summary>Stress, e.g. kg/(m Â· sÂ²), ksi, MPa</summary>
    UT_Stress = 45, // 0x0000002D
    /// <summary>Unit weight, e.g. N/mÂ³</summary>
    UT_UnitWeight = 46, // 0x0000002E
    /// <summary>Thermal expansion, e.g. 1/K</summary>
    UT_ThermalExpansion = 47, // 0x0000002F
    /// <summary>Linear moment, e,g. (N Â· m)/m, lbf / ft</summary>
    UT_LinearMoment = 48, // 0x00000030
    /// <summary>Linear moment scale, e.g. ft/kip, m/kN</summary>
    UT_LinearMomentScale = 49, // 0x00000031
    /// <summary>Point Spring Coefficient, e.g. kg/sÂ², N/m</summary>
    UT_ForcePerLength = 50, // 0x00000032
    /// <summary>
    ///    Rotational Point Spring Coefficient, e.g. (kg Â· mÂ²)/(sÂ² Â· rad), (N Â· m)/rad
    /// </summary>
    UT_ForceLengthPerAngle = 51, // 0x00000033
    /// <summary>Line Spring Coefficient, e.g. kg/(m Â· sÂ²), (N Â· m)/mÂ²</summary>
    UT_LinearForcePerLength = 52, // 0x00000034
    /// <summary>Rotational Line Spring Coefficient, e.g. (kg Â· m)/(sÂ² Â· rad), N/rad</summary>
    UT_LinearForceLengthPerAngle = 53, // 0x00000035
    /// <summary>Area Spring Coefficient, e.g.  kg/(mÂ² Â· sÂ²), N/mÂ³</summary>
    UT_AreaForcePerLength = 54, // 0x00000036
    /// <summary>Pipe Volume, e.g. gallons, liters</summary>
    UT_Piping_Volume = 55, // 0x00000037
    /// <summary>Dynamic Viscosity (HVAC), e.g. kg/(m Â· s), Pa Â· s</summary>
    UT_HVAC_Viscosity = 56, // 0x00000038
    /// <summary>
    ///    Coefficient of Heat Transfer (U-value) (HVAC), e.g. kg/(sÂ³ Â· K), W/(mÂ² Â· K)
    /// </summary>
    UT_HVAC_CoefficientOfHeatTransfer = 57, // 0x00000039
    /// <summary>Air Flow Density (HVAC), mÂ³/(s Â· mÂ²)</summary>
    UT_HVAC_Airflow_Density = 58, // 0x0000003A
    /// <summary>Slope, rise/run</summary>
    UT_Slope = 59, // 0x0000003B
    /// <summary>Cooling load (HVAC), e.g. (mÂ² Â· kg)/sÂ³, W, kW, Btu/s, Btu/h</summary>
    UT_HVAC_Cooling_Load = 60, // 0x0000003C
    /// <summary>
    ///    Cooling load per unit area (HVAC), e.g. kg/sÂ³, W/mÂ², W/ftÂ², Btu/(hÂ·ftÂ²)
    /// </summary>
    UT_HVAC_Cooling_Load_Divided_By_Area = 61, // 0x0000003D
    /// <summary>
    ///    Cooling load per unit volume (HVAC), e.g. kg/(sÂ³ Â· m), W/mÂ³, Btu/(hÂ·ftÂ³)
    /// </summary>
    UT_HVAC_Cooling_Load_Divided_By_Volume = 62, // 0x0000003E
    /// <summary>Heating load (HVAC), e.g. (mÂ² Â· kg)/sÂ³, W, kW, Btu/s, Btu/h</summary>
    UT_HVAC_Heating_Load = 63, // 0x0000003F
    /// <summary>
    ///    Heating load per unit area (HVAC), e.g. kg/sÂ³, W/mÂ², W/ftÂ², Btu/(hÂ·ftÂ²)
    /// </summary>
    UT_HVAC_Heating_Load_Divided_By_Area = 64, // 0x00000040
    /// <summary>
    ///    Heating load per unit volume (HVAC), e.g. kg/(sÂ³ Â· m), W/mÂ³, Btu/(hÂ·ftÂ³)
    /// </summary>
    UT_HVAC_Heating_Load_Divided_By_Volume = 65, // 0x00000041
    /// <summary>
    ///    Airflow per unit volume (HVAC), e.g. mÂ³/(s Â· mÂ³), CFM/ftÂ³, CFM/CF, L/(sÂ·mÂ³)
    /// </summary>
    UT_HVAC_Airflow_Divided_By_Volume = 66, // 0x00000042
    /// <summary>
    ///    Airflow per unit cooling load (HVAC), e.g. (m Â· sÂ²)/kg, ftÂ²/ton, SF/ton, mÂ²/kW
    /// </summary>
    UT_HVAC_Airflow_Divided_By_Cooling_Load = 67, // 0x00000043
    /// <summary>Area per unit cooling load (HVAC), e.g.  sÂ³/kg, ftÂ²/ton, mÂ²/kW</summary>
    UT_HVAC_Area_Divided_By_Cooling_Load = 68, // 0x00000044
    /// <summary>Wire Size (Electrical), e.g.	mm, inch</summary>
    UT_WireSize = 69, // 0x00000045
    /// <summary>Slope (HVAC)</summary>
    UT_HVAC_Slope = 70, // 0x00000046
    /// <summary>Slope (Piping)</summary>
    UT_Piping_Slope = 71, // 0x00000047
    /// <summary>Currency</summary>
    UT_Currency = 72, // 0x00000048
    /// <summary>Electrical efficacy (lighting), e.g. cdÂ·srÂ·sÂ³/(mÂ²Â·kg), lm/W</summary>
    UT_Electrical_Efficacy = 73, // 0x00000049
    /// <summary>Wattage (lighting), e.g. (mÂ² Â· kg)/sÂ³, W</summary>
    UT_Electrical_Wattage = 74, // 0x0000004A
    /// <summary>Color temperature (lighting), e.g. K</summary>
    UT_Color_Temperature = 75, // 0x0000004B
    /// <summary>Sheet length in decimal form, decimal inches, mm</summary>
    UT_DecSheetLength = 76, // 0x0000004C
    /// <summary>Luminous Intensity (Lighting), e.g. cd, cd</summary>
    UT_Electrical_Luminous_Intensity = 77, // 0x0000004D
    /// <summary>Luminance (Lighting), cd/mÂ², cd/mÂ²</summary>
    UT_Electrical_Luminance = 78, // 0x0000004E
    /// <summary>Area per unit heating load (HVAC), e.g.  sÂ³/kg, ftÂ²/ton, mÂ²/kW</summary>
    UT_HVAC_Area_Divided_By_Heating_Load = 79, // 0x0000004F
    /// <summary>Heating and coooling factor, percentage</summary>
    UT_HVAC_Factor = 80, // 0x00000050
    /// <summary>Temperature (electrical), e.g. F, C</summary>
    UT_Electrical_Temperature = 81, // 0x00000051
    /// <summary>Cable tray size (electrical), e.g. in, mm</summary>
    UT_Electrical_CableTraySize = 82, // 0x00000052
    /// <summary>Conduit size (electrical), e.g. in, mm</summary>
    UT_Electrical_ConduitSize = 83, // 0x00000053
    /// <summary>Structural reinforcement volume, e.g. inÂ³, cmÂ³</summary>
    UT_Reinforcement_Volume = 84, // 0x00000054
    /// <summary>Structural reinforcement length, e.g. mm, in, ft</summary>
    UT_Reinforcement_Length = 85, // 0x00000055
    /// <summary>Electrical demand factor, percentage</summary>
    UT_Electrical_Demand_Factor = 86, // 0x00000056
    /// <summary>Duct Insulation Thickness (HVAC), e.g. mm, in</summary>
    UT_HVAC_DuctInsulationThickness = 87, // 0x00000057
    /// <summary>Duct Lining Thickness (HVAC), e.g. mm, in</summary>
    UT_HVAC_DuctLiningThickness = 88, // 0x00000058
    /// <summary>Pipe Insulation Thickness (Piping), e.g. mm, in</summary>
    UT_PipeInsulationThickness = 89, // 0x00000059
    /// <summary>Thermal Resistance (HVAC), R Value, e.g. mÂ²Â·K/W</summary>
    UT_HVAC_ThermalResistance = 90, // 0x0000005A
    /// <summary>Thermal Mass (HVAC), e.g.  J/K, BTU/F</summary>
    UT_HVAC_ThermalMass = 91, // 0x0000005B
    /// <summary>Acceleration, e.g. m/sÂ², km/sÂ², in/sÂ², ft/sÂ², mi/sÂ²</summary>
    UT_Acceleration = 92, // 0x0000005C
    /// <summary>Bar Diameter, e.g. ', LF, ", m, cm, mm</summary>
    UT_Bar_Diameter = 93, // 0x0000005D
    /// <summary>Crack Width, e.g. ', LF, ", m, cm, mm</summary>
    UT_Crack_Width = 94, // 0x0000005E
    /// <summary>Displacement/Deflection, e.g. ', LF, ", m, cm, mm</summary>
    UT_Displacement_Deflection = 95, // 0x0000005F
    /// <summary>Energy, e.g. J, kJ, kgf-m, lb-ft, N-m</summary>
    UT_Energy = 96, // 0x00000060
    /// <summary>FREQUENCY, Frequency (Structural) e.g. Hz</summary>
    UT_Structural_Frequency = 97, // 0x00000061
    /// <summary>Mass, e.g.  kg, lb, t</summary>
    UT_Mass = 98, // 0x00000062
    /// <summary>Mass per Unit Length, e.g. kg/m, lb/ft</summary>
    UT_Mass_per_Unit_Length = 99, // 0x00000063
    /// <summary>Moment of Inertia, e.g. ft^4, in^4, mm^4, cm^4, m^4</summary>
    UT_Moment_of_Inertia = 100, // 0x00000064
    /// <summary>Surface Area, e.g. ftÂ²/ft, mÂ²/m</summary>
    UT_Surface_Area = 101, // 0x00000065
    /// <summary>Period, e.g. ms, s, min, h</summary>
    UT_Period = 102, // 0x00000066
    /// <summary>Pulsation, e.g. rad/s</summary>
    UT_Pulsation = 103, // 0x00000067
    /// <summary>Reinforcement Area, e.g. SF, ftÂ², inÂ², mmÂ², cmÂ², mÂ²</summary>
    UT_Reinforcement_Area = 104, // 0x00000068
    /// <summary>
    ///    Reinforcement Area per Unit Length, e.g. ftÂ²/ft, inÂ²/ft, mmÂ²/m, cmÂ²/m, mÂ²/m
    /// </summary>
    UT_Reinforcement_Area_per_Unit_Length = 105, // 0x00000069
    /// <summary>Reinforcement Cover, e.g. ', LF, ", m, cm, mm</summary>
    UT_Reinforcement_Cover = 106, // 0x0000006A
    /// <summary>Reinforcement Spacing, e.g. ', LF, ", m, cm, mm</summary>
    UT_Reinforcement_Spacing = 107, // 0x0000006B
    /// <summary>Rotation, e.g. Â°, rad, grad</summary>
    UT_Rotation = 108, // 0x0000006C
    /// <summary>Section Area, e.g.  ftÂ²/ft, inÂ²/ft, mmÂ²/m, cmÂ²/m, mÂ²/m</summary>
    UT_Section_Area = 109, // 0x0000006D
    /// <summary>Section Dimension, e.g.  ', LF, ", m, cm, mm</summary>
    UT_Section_Dimension = 110, // 0x0000006E
    /// <summary>Section Modulus, e.g. ft^3, in^3, mm^3, cm^3, m^3</summary>
    UT_Section_Modulus = 111, // 0x0000006F
    /// <summary>Section Property, e.g.  ', LF, ", m, cm, mm</summary>
    UT_Section_Property = 112, // 0x00000070
    /// <summary>Section Property, e.g. km/h, m/s, ft/min, ft/s, mph</summary>
    UT_Structural_Velocity = 113, // 0x00000071
    /// <summary>Warping Constant, e.g. ft^6, in^6, mm^6, cm^6, m^6</summary>
    UT_Warping_Constant = 114, // 0x00000072
    /// <summary>Weight, e.g. N, daN, kN, MN, kip, kgf, Tf, lb, lbf</summary>
    UT_Weight = 115, // 0x00000073
    /// <summary>
    ///    Weight per Unit Length, e.g. N/m, daN/m, kN/m, MN/m, kip/ft, kgf/m, Tf/m, lb/ft, lbf/ft, kip/in
    /// </summary>
    UT_Weight_per_Unit_Length = 116, // 0x00000074
    /// <summary>Thermal Conductivity (HVAC), e.g. W/(mÂ·K)</summary>
    UT_HVAC_ThermalConductivity = 117, // 0x00000075
    /// <summary>Specific Heat (HVAC), e.g. J/(gÂ·Â°C)</summary>
    UT_HVAC_SpecificHeat = 118, // 0x00000076
    /// <summary>Specific Heat of Vaporization, e.g. J/g</summary>
    UT_HVAC_SpecificHeatOfVaporization = 119, // 0x00000077
    /// <summary>Permeability, e.g. ng/(PaÂ·sÂ·mÂ²)</summary>
    UT_HVAC_Permeability = 120, // 0x00000078
    /// <summary>Electrical Resistivity, e.g.</summary>
    UT_Electrical_Resistivity = 121, // 0x00000079
    /// <summary>Mass Density, e.g. kg/mÂ³, lb/ftÂ³</summary>
    UT_MassDensity = 122, // 0x0000007A
    /// <summary>Mass Per Unit Area, e.g. kg/mÂ², lb/ftÂ²</summary>
    UT_MassPerUnitArea = 123, // 0x0000007B
    /// <summary>Length unit for pipe dimension, e.g. in, mm</summary>
    UT_Pipe_Dimension = 124, // 0x0000007C
    /// <summary>Mass, e.g.  kg, lb, t</summary>
    UT_PipeMass = 125, // 0x0000007D
    /// <summary>Mass per Unit Length, e.g. kg/m, lb/ft</summary>
    UT_PipeMassPerUnitLength = 126, // 0x0000007E
}