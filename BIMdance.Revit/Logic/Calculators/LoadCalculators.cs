namespace BIMdance.Revit.Logic.Calculators;

public class LoadCalculators
{
    private readonly ElectricalContext _electricalContext;
    private readonly LoadRebalance _loadRebalance;

    internal LoadCalculators(ElectricalCalculator electricalCalculator, ElectricalContext electricalContext)
    {
        _electricalContext = electricalContext;
        _loadRebalance = new LoadRebalance(electricalCalculator, electricalContext);
    }
    
    // internal void CalculateAllLoads()
    // {
    //     var sources = _electricalContext.ElectricalSources;
    //     var electricalEquipments = StructureUtils.GetOrderedConnectedEquipments(sources, operatingMode).Reverse();
    //
    //     using var @lock = LockService.ToLock(
    //         Lock.CalculateParents,
    //         Lock.Phase,
    //         Lock.SpecifyAdditionalDemandFactor);
    //         
    //     CalculateAllChildLoads(electricalEquipments);
    // }

    // private void CalculateAllChildLoads(IEnumerable<ElectricalEquipmentProxy> electricalEquipments)
    // {
    //     var powerElectricalEquipments = electricalEquipments.Where(n => n.IsPower).ToList();
    //
    //     // using (ProgressBarController.StartProcess(powerElectricalEquipments.Count))
    //     {
    //         foreach (var electricalEquipment in powerElectricalEquipments)
    //         {
    //             // ProgressBarController.Next(electricalEquipment.Name);
    //             Calculate(electricalEquipment);
    //         }
    //     }
    //
    //     // var equipmentGroups = powerElectricalEquipments.Where(n => n.EquipmentGroup != null).Select(n => n.EquipmentGroup).Distinct(new ElementProxyComparer<ElectricalBase>()).ToList();
    //     //
    //     // using (ProgressBarController.StartProcess(equipmentGroups.Count))
    //     // {
    //     //     foreach (var equipmentGroup in equipmentGroups)
    //     //     {
    //     //         ProgressBarController.Next(equipmentGroup.Name);
    //     //         ElectricalCalculator.Loads.Calculate(equipmentGroup);
    //     //     }
    //     // }
    // }
    
    public void CalculateAllOperatingModes()
    {
        foreach (var operatingMode in _electricalContext.OperatingModes)
            CalculateAll(operatingMode);
    }

    public void CalculateAll(OperatingMode operatingMode)
    {
        var sources = _electricalContext.ElectricalSources;
        var connectedEquipments = StructureUtils.GetOrderedConnectedEquipments(sources, operatingMode).Reverse().ToArray();
        var disconnectedEquipments = _electricalContext.Equipments.Where(x => !connectedEquipments.Contains(x));

        foreach (var disconnectedEquipment in disconnectedEquipments)
        {
            disconnectedEquipment.GetEstimatedPowerParameters(operatingMode).IsSupplied = false;
            
            // foreach (var electricalSystem in disconnectedEquipment.GetConsumersOf<ElectricalSystemProxy>())
            //     electricalSystem.GetEstimatedPowerParameters(operatingMode).IsSupplied = false;
        }

        foreach (var electricalEquipment in connectedEquipments)
        {
            foreach (var electricalSystem in electricalEquipment.GetConsumersOf<ElectricalSystemProxy>())
            {
                var systemCalculationUnit = new CalculationUnit(electricalSystem, operatingMode);
                // systemCalculationUnit.EstimatedPowerParameters.IsSupplied = true;
                Calculate(systemCalculationUnit);
            }

            var equipmentCalculationUnit = new CalculationUnit(electricalEquipment, operatingMode); 
            equipmentCalculationUnit.EstimatedPowerParameters.IsSupplied = true;
            Calculate(equipmentCalculationUnit);
        }
    }

    public void Calculate(ElectricalBase electrical, OperatingMode operatingMode, bool calculateParentLoads = false) =>
        new LoadCalculatorDefault(electrical, operatingMode).CalculateLoads(calculateParentLoads: calculateParentLoads);

    internal void Calculate(CalculationUnit calculationUnit, bool calculateParentLoads = false) =>
        new LoadCalculatorDefault(calculationUnit).CalculateLoads(calculateParentLoads: calculateParentLoads);

    // public void CalculateEstimateTrue(ElectricalBase electrical) =>
    //     LoadCalculatorHelper.CalculateEstimateCurrent(new ActualElectrical(electrical));
    //
    // public void CalculatePowerFactor(ElectricalBase electrical) =>
    //     LoadCalculatorHelper.CalculatePowerFactor(new ActualElectrical(electrical));
            
    // public static void Calculate(ElectricalEquipment electricalEquipment) =>
    //     StaticLoadCalculator.CalculateLoads(electricalEquipment);
            
    // public void Calculate(EquipmentSet equipmentGroup) =>
    //     StaticLoadCalculator.CalculateLoad(equipmentGroup);

    public void CalculateParents(ElectricalBase electrical, OperatingMode operatingMode) =>
        new LoadCalculatorDefault(electrical, operatingMode).CalculateParentLoads(estimateOnly: false);

    // public void CalculateParentsEstimate(ElectricalBase electrical) =>
    //     LoadCalculatorHelper.CalculateParentLoads(electrical, estimateOnly: true);

    // public void ReCalculateEstimateTrue(ElectricalBase electrical, double value, double oldValue) =>
    //     new LoadCalculator(electrical).ReCalculateEstimateTrueLoad(electrical, value, oldValue);
    // StaticLoadCalculator.ReCalculateEstimateTrueLoad(electrical, value, oldValue);

    // public void ReCalculateEstimateTrue(ElectricalSystemProxy electricalCircuit) =>
    //     LoadCalculatorHelper.ReCalculateEstimateTrueLoad(electricalCircuit);
    //
    // public void ReCalculateEstimateTrue(SwitchBoardUnit electricalPanel, double value, double oldValue) =>
    //     LoadCalculatorHelper.ReCalculateEstimateTrueLoad(electricalPanel, value, oldValue);

    public void RebalanceAll(RebalanceLoadMode rebalanceLoadMode, OperatingMode operatingMode, double tolerancePercent) =>
        _loadRebalance.RebalanceAll(rebalanceLoadMode, operatingMode, tolerancePercent);

    public void Rebalance(SwitchBoardUnit electricalPanel, RebalanceLoadMode rebalanceLoadMode, OperatingMode operatingMode, double tolerancePercent) =>
        _loadRebalance.Rebalance(electricalPanel, rebalanceLoadMode, operatingMode, tolerancePercent);

    // public void SetTotalApparent(ElectricalBase electrical) =>
    //     PowerElectricalCalculator.SetApparentLoad(electrical);
    //
    // public void SetElementTotalApparent(ElectricalBase electrical) =>
    //     PowerElectricalCalculator.SetElementApparentLoad(electrical);
    //
    // public void SetTotalTrue(ElectricalBase electrical) =>
    //     PowerElectricalCalculator.SetTrueLoad(electrical);

    // public void SetElementApparentLoad1(ElectricalBase electrical, double value) =>
    //     LoadCalculatorHelper.SetElementApparentLoad1(electrical, value);
    //
    // public void SetElementApparentLoad2(ElectricalBase electrical, double value) =>
    //     LoadCalculatorHelper.SetElementApparentLoad2(electrical, value);
    //
    // public void SetElementApparentLoad3(ElectricalBase electrical, double value) =>
    //     LoadCalculatorHelper.SetElementApparentLoad3(electrical, value);

    public void UpdateTotalDemandFactor(ElectricalBase electrical, OperatingMode operatingMode, double totalDemandFactor) =>
        new LoadCalculatorDefault(new CalculationUnit(electrical, operatingMode)).SetTotalDemandFactor(totalDemandFactor);

    public void UpdateAdditionalDemandFactor(ElectricalBase electrical, OperatingMode operatingMode, double additionalDemandFactor) => 
        new LoadCalculatorDefault(new CalculationUnit(electrical, operatingMode)).SetAdditionalDemandFactor(additionalDemandFactor);
}