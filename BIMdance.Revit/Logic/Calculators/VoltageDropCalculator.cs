namespace BIMdance.Revit.Logic.Calculators;

internal class VoltageDropCalculator : CalculationUnit
{
    internal VoltageDropCalculator(CalculationUnit calculationUnit) : base(calculationUnit) { }
        
    internal void Calculate()
    {
        if (!Electrical.IsPower)
            return;
            
        var powerParameters = Electrical.BaseConnector.PowerParameters;
        var resistanceReactance = EstimatedPowerParameters.ResistanceReactance;
            
        if (resistanceReactance.TotalR1.Equals(0) &&
            resistanceReactance.TotalX1.Equals(0))
            return;

        var fi = Math.Acos(powerParameters.OwnPowerFactor);
        var phaseFactor = powerParameters.PhasesNumber == PhasesNumber.Three ? 1 : 2;
            
        //var electricalCircuit = powerElectrical.ElectricalType == ElectricalType.ElectricalCircuit
        //    ? powerElectrical.ReferenceAs<ElectricalCircuit>()
        //    : powerElectrical.ReferenceAsElectricalElement?.ElectricalCircuit;

        var sourceVoltageDrop = Electrical.BaseSource?.GetEstimatedPowerParameters(OperatingMode)?.VoltageDrop ?? 0;
            
        EstimatedPowerParameters.VoltageDrop =
            sourceVoltageDrop +
            phaseFactor * EstimatedPowerParameters.Current
                        * (resistanceReactance.InternalR1 * EstimatedPowerParameters.PowerFactor +
                           resistanceReactance.InternalX1 * Math.Sin(fi));
        // / (electricalCircuit?.Devices.EstimateCablesCount ?? 1);
    }

    internal static void NewCheckAllVoltageDrop()
    {
        // if (LockService.IsLocked(Lock.Calculate))
        //     return;
        // using var uow = DomainUnitOfWork.NewInstance();
        // var networkRepository = new ElectricalSourceRepository(uow);
        // var allPanelsAndCircuits = networkRepository.GetOrderedElectricalItems(ElectricalSystemTypeProxy.PowerCircuit)
        //     .Select(n => n.Power);

        // foreach (var powerElectrical in allPanelsAndCircuits)
        //     ElectricalWarningService.CheckVoltageDrop(powerElectrical);
    }
}