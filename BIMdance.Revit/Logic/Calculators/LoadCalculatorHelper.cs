// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using CoLa.BimEd.Logic.Model.Comparers;
// using CoLa.BimEd.Logic.Model.Electrical;
// using CoLa.BimEd.Logic.Model.Electrical.Base;
// using CoLa.BimEd.Logic.Model.Electrical.Enums;
// using CoLa.BimEd.Logic.Model.RevitProxy;
// using CoLa.BimEd.Logic.Model.Utils;
// using CoLa.BimEd.Logic.SDK.Collections;
// using CoLa.BimEd.Logic.SDK.DependencyInjection;
// using CoLa.BimEd.Logic.SDK.Extensions;
// using CoLa.BimEd.Logic.SDK.Locks;
// using CoLa.BimEd.Logic.SDK.Logs;
//
// namespace CoLa.BimEd.Logic.Electrical.Calculators;
//
// internal static class LoadCalculatorHelper
// {
//     // internal static void CalculateLoad(EquipmentSet equipmentGroup, bool estimateOnly = false)
//     // {
//     //     if (LockService.IsLocked(Lock.Calculate))
//     //         return;
//     //
//     //     var powerElectrical = equipmentGroup?.Power;
//     //
//     //     if (powerElectrical == null)
//     //         return;
//     //
//     //     var allPowerElements = GetAllPowerElements(equipmentGroup);
//     //
//     //     // CalculateLoad(powerElectrical, allPowerElements, estimateOnly);
//     // }
//
//     internal static void CalculateLoads(ElectricalEquipmentProxy electricalEquipment)
//     {
//         if (LockService.IsLocked(Lock.Calculate))
//             return;
//
//         if (LockService.IsLocked(Lock.SpecifyAdditionalDemandFactor) == false)
//             LockService.ToLock(Lock.SpecifyAdditionalDemandFactor);
//
//         foreach (var powerCircuit in electricalEquipment.GetConsumersOf<ElectricalSystemProxy>())
//         {
//             CalculateLoad(powerCircuit);
//         }
//
//         CalculateLoad(electricalEquipment);
//     }
//
//     internal static void CalculateLoad(ElectricalBase electrical, bool estimateOnly = false)
//     {
//         if (electrical == null ||
//             LockService.IsLocked(Lock.Calculate))
//             return;
//
//         if (!electrical.IsPower)
//             return;
//
//         var allPowerElements = electrical.GetAllConsumersOf<ElectricalElementProxy>().ToList();
//             
//         CalculateLoad(electrical, allPowerElements, estimateOnly);
//     }
//
//     private static void CalculateLoad(ElectricalBase electrical, IList<ElectricalElementProxy> allPowerElements, bool estimateOnly = false)
//     {
//         if (allPowerElements.IsEmpty())
//             return;
//             
//         var actual = new ActualElectrical(electrical);
//
//         using (LockService.ToLock(Lock.IsElectricalLoadCalculating))
//         {
//             if (estimateOnly)
//             {
//                 CalculateEstimateTrueLoad(actual, allPowerElements);
//             }
//             else
//             {
//                 CalculateApparentAndTrueLoad(actual, allPowerElements);
//                 CalculateEstimateTrueLoad(actual, allPowerElements);
//             }
//         }
//
//         CalculateEstimateCurrent(actual);
//     }
//
//     internal static void CalculateEstimateCurrent(ActualElectrical actual)
//     {
//         if (LockService.IsLocked(Lock.IsElectricalLoadCalculating))
//             return;
//
//         if (actual.PowerParameters.PhasesNumber == PhasesNumber.One)
//         {
//             CalculateOnePhaseEstimateCurrent(actual);
//         }
//         else
//         {
//             var estimatedPowerParameters = actual.EstimatedPowerParameters;
//             SetApparentLoad(estimatedPowerParameters);
//             SetTrueLoad(estimatedPowerParameters);
//             CalculateThreePhasesEstimateCurrent(actual);
//         }
//     }
//
//     private static void CalculateThreePhasesEstimateCurrent(ActualElectrical actual)
//     {
//         CalculatePowerFactor(actual);
//         CalculateEstimateCurrents(actual);
//     }
//
//     internal static void CalculatePowerFactor(ActualElectrical actual)
//     {
//         if (LockService.IsLocked(Lock.IsElectricalLoadCalculating))
//             return;
//
//         var estimatedPowerParameters = actual.EstimatedPowerParameters;
//             
//         SetTotalDemandFactor(
//             actual,
//             estimatedPowerParameters.TrueLoad > 0
//                 ? estimatedPowerParameters.EstimateTrueLoad / estimatedPowerParameters.TrueLoad
//                 : 1);
//
//         estimatedPowerParameters.EstimateApparentLoad = estimatedPowerParameters.TotalDemandFactor * estimatedPowerParameters.ApparentLoad;
//
//         estimatedPowerParameters.PowerFactor = estimatedPowerParameters.ApparentLoad > 0
//             ? estimatedPowerParameters.TrueLoad / estimatedPowerParameters.ApparentLoad
//             : 1;
//     }
//
//     internal static void CalculateEstimateCurrents(ActualElectrical actual)
//     {
//         if (LockService.IsLocked(Lock.IsElectricalLoadCalculating))
//             return;
//
//         CalculateEstimateCurrent(actual, Phase.L1);
//         CalculateEstimateCurrent(actual, Phase.L2);
//         CalculateEstimateCurrent(actual, Phase.L3);
//     }
//
//     internal static void CalculateEstimateCurrent(ActualElectrical actual, Phase phase)
//     {
//         if (LockService.IsLocked(Lock.IsElectricalLoadCalculating))
//             return;
//
//         var (apparentLoad, trueLoad, estimateTrueLoad) = GetLoads(actual.EstimatedPowerParameters, phase);
//         var demandFactor = trueLoad > 0 ? estimateTrueLoad / trueLoad : 1;
//         var estimateApparentLoad = demandFactor * apparentLoad;
//         var voltage = actual.EstimatedPowerParameters.LineToGroundVoltage;
//         var estimateCurrent = voltage > 0 ? estimateApparentLoad / voltage : 0;
//
//         SetEstimateCurrent(actual, phase, estimateCurrent);
//     }
//
//     internal static void CalculateOnePhaseEstimateCurrent(ActualElectrical actual)
//     {
//         if (LockService.IsLocked(Lock.IsElectricalLoadCalculating))
//             return;
//
//         var estimatedPowerParameters = actual.EstimatedPowerParameters;
//         var powerParameters = actual.PowerParameters;
//             
//         var totalDemandFactor = estimatedPowerParameters.TrueLoad > 0
//             ? estimatedPowerParameters.EstimateTrueLoad / estimatedPowerParameters.TrueLoad
//             : 1;
//             
//         SetTotalDemandFactor(actual, totalDemandFactor);
//         // powerElectrical.TotalDemandFactor = powerElectrical.TrueLoad > 0 ? powerElectrical.EstimateTrueLoad / powerElectrical.TrueLoad : 1;
//         estimatedPowerParameters.EstimateApparentLoad = estimatedPowerParameters.ApparentLoad * estimatedPowerParameters.TotalDemandFactor;
//         estimatedPowerParameters.Current = estimatedPowerParameters.EstimateApparentLoad / estimatedPowerParameters.LineToGroundVoltage / (int)powerParameters.PhasesNumber;
//     }
//
//     internal static void CalculateParentLoads(ElectricalBase electrical, bool estimateOnly)
//     {
//         if (LockService.IsLocked(Lock.Calculate) || electrical == null)
//             return;
//
//         try
//         {
//             using (LockService.ToLock(Lock.Update))
//             {
//                 if (electrical is ElectricalSystemProxy electricalCircuit)
//                     CalculateLoad(electricalCircuit, estimateOnly);
//
//                 var electricalPanel = GetElectricalPanel(electrical);
//
//                 if (LockService.IsLocked(Lock.CalculateParents))
//                     return;
//
//                 while (electricalPanel != null)
//                 {
//                     CalculateLoad(electricalPanel, estimateOnly);
//                     CalculateLoad(electricalPanel.GetFirstSourceOf<ElectricalSystemProxy>(), estimateOnly);
//
//                     electricalPanel = electricalPanel.GetFirstSourceOf<SwitchBoardUnit>();
//                 }
//             }
//         }
//         finally
//         {
//             try
//             {
//                 //BimEdController.UpdateController.UpdateLockedElementPropertyNames();
//             }
//             catch (Exception exception)
//             {
//                 DI.Get<Logger>().Error(exception);
//             }
//         }
//     }
//
//     private static SwitchBoardUnit GetElectricalPanel(ElectricalBase electrical)
//     {
//         switch (electrical)
//         {
//             case SwitchBoardUnit electricalPanel:
//                 return electricalPanel;
//
//             case ElectricalSystemProxy electricalCircuit:
//                 return electricalCircuit.GetFirstSourceOf<SwitchBoardUnit>();
//
//             case null:
//                 throw new NullReferenceException(nameof(electrical));
//
//             default:
//                 throw new ArgumentOutOfRangeException(nameof(electrical), electrical, new ArgumentOutOfRangeException().Message);
//         }
//     }
//
//     // private static List<ElectricalElementProxy> GetAllPowerElements(EquipmentSet equipmentGroup) =>
//     //     equipmentGroup.OfType<ElectricalEquipment>().SelectMany(n => n.GetAllConsumersOf<ElectricalElementProxy>()).ToList();
//
//     #region CalculateLoadApparentAndTrue
//
//     private static void CalculateApparentAndTrueLoad(ActualElectrical actual, IEnumerable<ElectricalElementProxy> elements)
//     {
//         switch (actual.PowerParameters.PhasesNumber)
//         {
//             case PhasesNumber.One:
//                 CalculateLoadApparentAndTrue1(actual.EstimatedPowerParameters, elements);
//                 break;
//
//             case PhasesNumber.Two:
//             case PhasesNumber.Three:
//                 CalculateLoadApparentAndTrue3(actual.EstimatedPowerParameters, elements);
//                 break;
//         }
//     }
//
//     private static void CalculateLoadApparentAndTrue1(EstimatedPowerParameters estimatedPowerParameters, IEnumerable<ElectricalElementProxy> electricalElements)
//     {
//         ResetLoadApparentAndTrue(estimatedPowerParameters);
//
//         foreach (var electricalElement in electricalElements)
//         {
//             var elementPowerElectrical = electricalElement.PowerParameters;
//             estimatedPowerParameters.ApparentLoad += elementPowerElectrical.OwnApparentLoad;
//             estimatedPowerParameters.TrueLoad += elementPowerElectrical.OwnApparentLoad * elementPowerElectrical.OwnPowerFactor;
//         }
//     }
//
//     private static void CalculateLoadApparentAndTrue3(EstimatedPowerParameters estimatedPowerParameters, IEnumerable<ElectricalElementProxy> electricalElements)
//     {
//         ResetLoadApparentAndTrue(estimatedPowerParameters);
//
//         foreach (var element in electricalElements)
//         {
//             switch (element.PowerParameters.PhasesNumber)
//             {
//                 case PhasesNumber.One:
//                     AddLoadApparentAndTrueOnePhase(estimatedPowerParameters, element);
//                     continue;
//
//                 case PhasesNumber.Two:
//                     AddLoadApparentAndTrueTwoPhases(estimatedPowerParameters, element);
//                     continue;
//
//                 case PhasesNumber.Three:
//                     AddLoadApparentAndTrueThreePhases(estimatedPowerParameters, element);
//                     continue;
//             }
//         }
//
//         SetTrueLoad(estimatedPowerParameters, ignoreLock: true);
//     }
//
//     private static void AddLoadApparentAndTrueOnePhase(EstimatedPowerParameters estimatedPowerParameters, ElectricalElementProxy electricalElement)
//     {
//         var powerElectrical = electricalElement.PowerParameters;
//         var loadApparent = powerElectrical.OwnApparentLoad;
//         var loadTrue = powerElectrical.OwnApparentLoad * powerElectrical.OwnPowerFactor;
//         var electricalCircuit = electricalElement.GetFirstSourceOf<ElectricalSystemProxy>();
//         var phase = electricalCircuit.PowerParameters.Phase;
//
//         CorrectPhaseValue(ref phase, PhasesNumber.One);
//
//         var phaseService = new PhaseUtils();
//             
//         if (electricalCircuit.PowerParameters.Phase != phase)
//             phaseService.SetPhase(electricalCircuit, phase);
//
//         switch (phase)
//         {
//             case Phase.L1:
//                 AddApparentLoad1(estimatedPowerParameters, loadApparent);
//                 AddTrueLoad1(estimatedPowerParameters, loadTrue);
//                 break;
//
//             case Phase.L2:
//                 AddApparentLoad2(estimatedPowerParameters, loadApparent);
//                 AddTrueLoad2(estimatedPowerParameters, loadTrue);
//                 break;
//
//             case Phase.L3:
//                 AddApparentLoad3(estimatedPowerParameters, loadApparent);
//                 AddTrueLoad3(estimatedPowerParameters, loadTrue);
//                 break;
//
//             default:
//                 throw new ArgumentOutOfRangeException(nameof(phase), phase, new ArgumentOutOfRangeException().Message);
//         }
//     }
//
//     private static void AddLoadApparentAndTrueTwoPhases(EstimatedPowerParameters estimatedPowerParameters, ElectricalElementProxy electricalElement)
//     {
//         var elementPowerElectrical = electricalElement.PowerParameters;
//         var powerFactor = elementPowerElectrical.OwnPowerFactor;
//         var apparentLoad1 = elementPowerElectrical.OwnApparentLoad1;
//         var apparentLoad2 = elementPowerElectrical.OwnApparentLoad2;
//         var trueLoad1 = apparentLoad1 * powerFactor;
//         var trueLoad2 = apparentLoad2 * powerFactor;
//             
//         var phase = elementPowerElectrical.Phase;
//
//         CorrectPhaseValue(ref phase, PhasesNumber.Two);
//
//         if (elementPowerElectrical.Phase != phase)
//         {
//             var phaseService = new PhaseUtils();
//             phaseService.SetPhase(electricalElement, phase);
//         }
//
//         switch (phase)
//         {
//             case Phase.L1:
//                 AddApparentLoad1(estimatedPowerParameters, elementPowerElectrical.OwnApparentLoad);
//                 return;
//
//             case Phase.L2:
//                 AddApparentLoad2(estimatedPowerParameters, elementPowerElectrical.OwnApparentLoad);
//                 return;
//
//             case Phase.L3:
//                 AddApparentLoad3(estimatedPowerParameters, elementPowerElectrical.OwnApparentLoad);
//                 return;
//
//             case Phase.L12:
//                 AddApparentLoad1(estimatedPowerParameters, apparentLoad1);
//                 AddApparentLoad2(estimatedPowerParameters, apparentLoad2);
//                 AddTrueLoad1(estimatedPowerParameters, trueLoad1);
//                 AddTrueLoad2(estimatedPowerParameters, trueLoad2);
//                 return;
//                 
//             case Phase.L13:
//                 AddApparentLoad1(estimatedPowerParameters, apparentLoad1);
//                 AddApparentLoad3(estimatedPowerParameters, apparentLoad2);
//                 AddTrueLoad1(estimatedPowerParameters, trueLoad1);
//                 AddTrueLoad3(estimatedPowerParameters, trueLoad2);
//                 return;
//                 
//             case Phase.L23:
//                 AddApparentLoad2(estimatedPowerParameters, apparentLoad1);
//                 AddApparentLoad3(estimatedPowerParameters, apparentLoad2);
//                 AddTrueLoad2(estimatedPowerParameters, trueLoad1);
//                 AddTrueLoad3(estimatedPowerParameters, trueLoad2);
//                 return;
//
//             default:
//                 throw new ArgumentOutOfRangeException(nameof(elementPowerElectrical.Phase), elementPowerElectrical.Phase, new ArgumentOutOfRangeException().Message);
//         }
//     }
//
//     private static void CorrectPhaseValue(ref Phase phase, PhasesNumber phasesNumber)
//     {
//         switch (phasesNumber)
//         {
//             case PhasesNumber.One:
//                 if (phase < Phase.L1)
//                     phase = Phase.L1;
//                 else if (phase > Phase.L3)
//                     phase = (Phase)(1 + (int)phase % 3);
//                 return;
//
//             case PhasesNumber.Two:
//                 if (phase < Phase.L1)
//                     phase = Phase.L12;
//                 else if (phase < Phase.L12)
//                     phase += 3;
//                 return;
//         }
//     }
//
//     private static void AddLoadApparentAndTrueThreePhases(EstimatedPowerParameters estimatedPowerParameters, ElectricalElementProxy electricalElement)
//     {
//         var powerParameters = electricalElement.PowerParameters;
//         var powerFactor = powerParameters.OwnPowerFactor;
//             
//         AddApparentLoad1(estimatedPowerParameters, powerParameters.OwnApparentLoad1);
//         AddApparentLoad2(estimatedPowerParameters, powerParameters.OwnApparentLoad2);
//         AddApparentLoad3(estimatedPowerParameters, powerParameters.OwnApparentLoad3);
//
//         AddTrueLoad1(estimatedPowerParameters, powerParameters.OwnApparentLoad1 * powerFactor);
//         AddTrueLoad2(estimatedPowerParameters, powerParameters.OwnApparentLoad2 * powerFactor);
//         AddTrueLoad3(estimatedPowerParameters, powerParameters.OwnApparentLoad3 * powerFactor);
//     }
//
//     private static void ResetLoadApparentAndTrue(EstimatedPowerParameters estimatedPowerParameters)
//     {
//         var threePhases = estimatedPowerParameters.ThreePhases;
//             
//         if (threePhases == null)
//         {
//             estimatedPowerParameters.ApparentLoad = 0;
//             estimatedPowerParameters.TrueLoad = 0;
//         }
//         else
//         {
//             threePhases.ApparentLoad1 = 0;
//             threePhases.ApparentLoad2 = 0;
//             threePhases.ApparentLoad3 = 0;
//             threePhases.TrueLoad1 = 0;
//             threePhases.TrueLoad2 = 0;
//             threePhases.TrueLoad3 = 0;
//         }
//     }
//
//     #endregion
//         
//     private static void CalculateEstimateTrueLoad(ActualElectrical actual, IList<ElectricalElementProxy> allPowerElements)
//     {
//         ResetEstimateLoadTrue(actual);
//
//         allPowerElements ??= actual.Electrical.GetAllConsumersOf<ElectricalElementProxy>().ToList();
//
//         var loadClassifications = allPowerElements.Select(n => n.PowerParameters.LoadClassification)
//             .Distinct(new ElementProxyComparer<LoadClassificationProxy>());
//
//         LoadClassificationProxy maxLoadClassification = null;
//         var maxLoad = 0d;
//         var demandFactorTrueLoad = 0d;
//
//         foreach (var loadClassification in loadClassifications)
//         {
//             var loadClassificationLoad = CalculateLoadClassification(actual, allPowerElements, loadClassification);
//                 
//             demandFactorTrueLoad += loadClassificationLoad;
//
//             if (loadClassificationLoad < maxLoad)
//                 continue;
//
//             maxLoad = loadClassificationLoad;
//             maxLoadClassification = loadClassification;
//         }
//
//         var estimatedPowerParameters = actual.EstimatedPowerParameters;
//         actual.EstimatedPowerParameters.DemandFactor = /*powerElectrical.EstimateTrueLoad*/ demandFactorTrueLoad / estimatedPowerParameters.TrueLoad;
//         actual.LoadClassification = maxLoadClassification;
//     }
//
//     #region CalculateLoadTrueEstimate
//
//     private static void ResetEstimateLoadTrue(ActualElectrical actual)
//     {
//         var estimatedPowerParameters = actual.EstimatedPowerParameters;
//         var threePhases = estimatedPowerParameters.ThreePhases;
//             
//         switch (actual.PowerParameters.PhasesNumber)
//         {
//             case PhasesNumber.One:
//                 estimatedPowerParameters.EstimateTrueLoad = 0;
//                 break;
//
//             case PhasesNumber.Two:
//             case PhasesNumber.Three:
//                 threePhases.EstimateTrueLoad1 = 0;
//                 threePhases.EstimateTrueLoad2 = 0;
//                 threePhases.EstimateTrueLoad3 = 0;
//                 break;
//
//             default:
//                 throw new ArgumentOutOfRangeException(nameof(actual.PowerParameters.PhasesNumber), actual.PowerParameters.PhasesNumber, new ArgumentOutOfRangeException().Message);
//         }
//     }
//
//     private static double CalculateLoadClassification(
//         ActualElectrical actual,
//         IEnumerable<ElectricalElementProxy> allPowerElements,
//         LoadClassificationProxy loadClassification)
//     {
//         var connectorParameters = actual.PowerParameters;
//         var elements = allPowerElements.Where(n =>
//                 n.PowerParameters.LoadClassification?.RevitId == loadClassification?.RevitId &&
//                 n.PowerParameters.OwnApparentLoad > 0)
//             .ToList();
//
//         switch (connectorParameters.PhasesNumber)
//         {
//             case PhasesNumber.One:
//                 return CalculateEstimateTrueLoad1(actual, loadClassification, elements);
//
//             case PhasesNumber.Two:
//             case PhasesNumber.Three:
//                 return CalculateEstimateTrueLoad3(actual, loadClassification, elements);
//
//             default:
//                 throw new ArgumentOutOfRangeException();
//         }
//     }
//
//     private static double CalculateEstimateTrueLoad1(
//         ActualElectrical actual,
//         LoadClassificationProxy loadClassification,
//         List<ElectricalElementProxy> allPowerElementsLoadClassification)
//     {
//         var directConnectedTrueLoad = 0d;
//         var directConnectedAdditionalTrueLoad = 0d;
//         var nonDirectConnectedTrueLoad = 0d;
//         var nonDirectConnectedAdditionalTrueLoad = 0d;
//         var nonDirectConnectedElements = new List<ElectricalElementProxy>();
//
//         foreach (var element in allPowerElementsLoadClassification)
//         {
//             var elementPowerElectrical = element.PowerParameters;
//             var additionalDemandFactor = element.GetAllSourcesOf<ElectricalSystemProxy>(to: actual.Electrical)
//                 .Aggregate(1.0, (current, n) => current * n.EstimatedPowerParameters.AdditionalDemandFactor);
//
//             var elementTrueLoad = elementPowerElectrical.OwnApparentLoad * elementPowerElectrical.OwnPowerFactor;
//
//             if (ElementIsDirectConnected(actual.Electrical, element))
//             {
//                 directConnectedTrueLoad += elementTrueLoad;
//                 directConnectedAdditionalTrueLoad += elementTrueLoad * additionalDemandFactor;
//             }
//             else
//             {
//                 nonDirectConnectedElements.Add(element);
//                 nonDirectConnectedTrueLoad += elementTrueLoad;
//                 nonDirectConnectedAdditionalTrueLoad += elementTrueLoad * additionalDemandFactor;
//             }
//         }
//
//         var demandFactor = loadClassification?.DemandFactor?.GetValue(nonDirectConnectedElements) ?? 1;
//         var estimateTrueLoad = directConnectedTrueLoad + nonDirectConnectedTrueLoad * demandFactor;
//         var estimateAdditionalTrueLoad = directConnectedAdditionalTrueLoad + nonDirectConnectedAdditionalTrueLoad * demandFactor;
//
//         AddEstimateTrueLoad(actual, estimateAdditionalTrueLoad);
//
//         return estimateTrueLoad;
//     }
//
//     private static double CalculateEstimateTrueLoad3(
//         ActualElectrical actual,
//         LoadClassificationProxy loadClassification,
//         List<ElectricalElementProxy> allPowerElementsLoadClassification)
//     {
//         var directConnectedTrueLoad = new ThreePhasesLoad();
//         var directConnectedAdditionalTrueLoad = new ThreePhasesLoad();
//         var nonDirectConnectedTrueLoad = new ThreePhasesLoad();
//         var nonDirectConnectedAdditionalTrueLoad = new ThreePhasesLoad();
//         var nonDirectConnectedElements = new List<ElectricalElementProxy>();
//
//         foreach (var element in allPowerElementsLoadClassification)
//         {
//             if (ElementIsDirectConnected(actual.Electrical, element))
//             {
//                 CalculateTrueLoad(ref directConnectedTrueLoad, ref directConnectedAdditionalTrueLoad, actual.Electrical, element);//, parentCircuit);
//             }
//             else
//             {
//                 CalculateTrueLoad(ref nonDirectConnectedTrueLoad, ref nonDirectConnectedAdditionalTrueLoad, actual.Electrical, element);//, parentCircuit);
//                 nonDirectConnectedElements.Add(element);
//             }
//         }
//
//         var demandFactor = loadClassification?.DemandFactor?.GetValue(nonDirectConnectedElements) ?? 1;
//         var estimateTrueLoad = directConnectedTrueLoad + nonDirectConnectedTrueLoad * demandFactor;
//         var estimateAdditionalTrueLoad = directConnectedAdditionalTrueLoad + nonDirectConnectedAdditionalTrueLoad * demandFactor;// * circuitAdditionalDemandFactor;
//         var threePhases = actual.EstimatedPowerParameters.ThreePhases;
//
//         if (loadClassification != null)
//         {
//             AddLoadToLoadClassification(threePhases.LoadClassificationTrueEstimate1, loadClassification, estimateAdditionalTrueLoad.Load1);
//             AddLoadToLoadClassification(threePhases.LoadClassificationTrueEstimate2, loadClassification, estimateAdditionalTrueLoad.Load2);
//             AddLoadToLoadClassification(threePhases.LoadClassificationTrueEstimate3, loadClassification, estimateAdditionalTrueLoad.Load3);
//         }
//
//         AddEstimateTrueLoad1(actual, estimateAdditionalTrueLoad.Load1);
//         AddEstimateTrueLoad2(actual, estimateAdditionalTrueLoad.Load2);
//         AddEstimateTrueLoad3(actual, estimateAdditionalTrueLoad.Load3);
//
//         return estimateTrueLoad.Total;
//     }
//
//     private static void AddLoadToLoadClassification(IDictionary<LoadClassificationProxy, double> loadClassificationDictionary, LoadClassificationProxy loadClassification, double value)
//     {
//         if (loadClassificationDictionary.ContainsKey(loadClassification))
//             loadClassificationDictionary[loadClassification] += value;
//         else
//             loadClassificationDictionary.Add(loadClassification, value);
//     }
//
//     private static bool ElementIsDirectConnected(ElectricalBase parentElement, ElectricalElementProxy element) =>
//         parentElement is ElectricalSystemProxy electricalCircuit &&
//         electricalCircuit.RevitId == element.GetFirstSourceOf<ElectricalSystemProxy>()?.RevitId;
//
//     private static void CalculateTrueLoad(
//         ref ThreePhasesLoad trueLoad,
//         ref ThreePhasesLoad additionalTrueLoad,
//         ElectricalBase electrical,
//         ElectricalElementProxy element)
//     {
//         var additionalDemandFactor = element.GetAllSourcesOf<ElectricalSystemProxy>(to: electrical)
//             .Aggregate(1.0, (current, n) => current * n.EstimatedPowerParameters.AdditionalDemandFactor);
//
//         switch (element.PowerParameters.PhasesNumber)
//         {
//             case PhasesNumber.One:
//                 AddOnePhaseLoad(ref trueLoad, ref additionalTrueLoad, element, additionalDemandFactor);
//                 return;
//
//             case PhasesNumber.Two:
//             case PhasesNumber.Three:
//                 AddThreePhasesLoad(ref trueLoad, ref additionalTrueLoad, element, additionalDemandFactor);
//                 return;
//         }
//     }
//
//     private static void AddOnePhaseLoad(ref ThreePhasesLoad trueLoad, ref ThreePhasesLoad additionalTrueLoad, ElectricalElementProxy element, double additionalDemandFactor)
//     {
//         var powerElectrical = element.PowerParameters;
//         var elementTrueLoad = powerElectrical.OwnApparentLoad * powerElectrical.OwnPowerFactor;
//         var elementAdditionalTrueLoad = elementTrueLoad * additionalDemandFactor;
//             
//         switch (element.GetFirstSourceOf<ElectricalSystemProxy>().PowerParameters.Phase)
//         {
//             case Phase.L1:
//                 trueLoad.Load1 += elementTrueLoad;
//                 additionalTrueLoad.Load1 += elementAdditionalTrueLoad;
//                 return;
//
//             case Phase.L2:
//                 trueLoad.Load2 += elementTrueLoad;
//                 additionalTrueLoad.Load2 += elementAdditionalTrueLoad;
//                 return;
//
//             case Phase.L3:
//                 trueLoad.Load3 += elementTrueLoad;
//                 additionalTrueLoad.Load3 += elementAdditionalTrueLoad;
//                 return;
//         }
//     }
//
//     private static void AddThreePhasesLoad(ref ThreePhasesLoad trueLoad, ref ThreePhasesLoad additionalTrueLoad, ElectricalElementProxy element, double additionalDemandFactor)
//     {
//         var elementPowerElectrical = element.PowerParameters;
//         var elementTrueLoad3 = elementPowerElectrical.IsTwoPhases
//             ? TwoPhasesElementTrueLoad(element)
//             : ThreePhasesElementTrueLoad(element);
//
//         trueLoad += elementTrueLoad3;
//         additionalTrueLoad += elementTrueLoad3 * additionalDemandFactor;
//     }
//
//     private static ThreePhasesLoad TwoPhasesElementTrueLoad(ElectricalBase electrical)
//     {
//         var powerParameters = electrical.PowerParameters;
//         var powerFactor = powerParameters.OwnPowerFactor;
//
//         return powerParameters.Phase switch
//         {
//             Phase.L1 => new ThreePhasesLoad { Load1 = powerParameters.OwnApparentLoad * powerFactor, },
//             Phase.L2 => new ThreePhasesLoad { Load2 = powerParameters.OwnApparentLoad * powerFactor, },
//             Phase.L3 => new ThreePhasesLoad { Load3 = powerParameters.OwnApparentLoad * powerFactor, },
//             Phase.L12 => new ThreePhasesLoad
//             {
//                 Load1 = powerParameters.OwnApparentLoad1 * powerFactor,
//                 Load2 = powerParameters.OwnApparentLoad2 * powerFactor,
//             },
//             Phase.L13 => new ThreePhasesLoad
//             {
//                 Load1 = powerParameters.OwnApparentLoad1 * powerFactor,
//                 Load3 = powerParameters.OwnApparentLoad3 * powerFactor,
//             },
//             Phase.L23 => new ThreePhasesLoad
//             {
//                 Load2 = powerParameters.OwnApparentLoad2 * powerFactor,
//                 Load3 = powerParameters.OwnApparentLoad3 * powerFactor,
//             },
//             _ => throw new ArgumentOutOfRangeException(nameof(powerParameters.Phase), powerParameters.Phase,
//                 $"Invalid value for 2-phases load.\n{new ArgumentOutOfRangeException().Message}")
//         };
//     }
//
//     private static ThreePhasesLoad ThreePhasesElementTrueLoad(ElectricalBase electrical)
//     {
//         var powerParameters = electrical.PowerParameters;
//         var powerFactor = powerParameters.OwnPowerFactor;
//             
//         return new ThreePhasesLoad
//         {
//             Load1 = powerParameters.OwnApparentLoad1 * powerFactor,
//             Load2 = powerParameters.OwnApparentLoad2 * powerFactor,
//             Load3 = powerParameters.OwnApparentLoad3 * powerFactor
//         };
//     }
//
//     private struct ThreePhasesLoad
//     {
//         public double Load1 { get; set; }
//         public double Load2 { get; set; }
//         public double Load3 { get; set; }
//         public double Total => Load1 + Load2 + Load3;
//
//     public static ThreePhasesLoad operator +(ThreePhasesLoad threePhasesLoad1, ThreePhasesLoad threePhasesLoad2) => new()
//         {
//             Load1 = threePhasesLoad1.Load1 + threePhasesLoad2.Load1,
//             Load2 = threePhasesLoad1.Load2 + threePhasesLoad2.Load2,
//             Load3 = threePhasesLoad1.Load3 + threePhasesLoad2.Load3,
//         };
//
//         public static ThreePhasesLoad operator *(ThreePhasesLoad threePhasesLoad, double multiplier) => new()
//         {
//             Load1 = threePhasesLoad.Load1 * multiplier,
//             Load2 = threePhasesLoad.Load2 * multiplier,
//             Load3 = threePhasesLoad.Load3 * multiplier,
//         };
//
//         public static ThreePhasesLoad operator *(double multiplier, ThreePhasesLoad threePhasesLoad) => threePhasesLoad * multiplier;
//
//         public override string ToString()
//         {
//             return $"{Total} = {Load1} + {Load2} + {Load3}";
//         }
//     }
//
//     #endregion
//
//     public static void ReCalculateEstimateTrueLoad(ElectricalBase electrical, double value, double oldValue)
//     {
//         if (LockService.IsLocked(Lock.Calculate))
//             return;
//
//         switch (electrical)
//         {
//             case ElectricalSystemProxy circuit:
//                 ReCalculateEstimateTrueLoad(circuit);
//                 break;
//
//             case SwitchBoardUnit panel:
//                 ReCalculateEstimateTrueLoad(panel, value, oldValue);
//                 break;
//
//             case ElectricalElementProxy _:
//                 break;
//
//             default:
//                 throw new ArgumentOutOfRangeException(nameof(electrical), electrical, new ArgumentOutOfRangeException().Message);
//         }
//     }
//
//     public static void ReCalculateEstimateTrueLoad(ElectricalSystemProxy electricalCircuit)
//     {
//         try
//         {
//             ReCalculateLoadTrueEstimateTransaction(electricalCircuit);
//         }
//         catch (TypeInitializationException exception)
//         {
//             if (exception.InnerException is not FileLoadException &&
//                 exception.InnerException is not FileNotFoundException)
//                 throw;
//
//             CalculateEstimateTrueLoad(electricalCircuit);
//         }
//         catch (FileLoadException)
//         {
//             CalculateEstimateTrueLoad(electricalCircuit);
//         }
//         catch (FileNotFoundException)
//         {
//             CalculateEstimateTrueLoad(electricalCircuit);
//         }
//
//         // TODO 2021.01.10
//         // ElectricalWarningService.CheckCable(electricalCircuit);
//         // ElectricalWarningService.CheckCircuitBreaker(electricalCircuit);
//     }
//
//     #region ReCalculateEstimateTrueLoad(IPowerElectricalSystemProxy powerCircuit)
//
//     private static void ReCalculateLoadTrueEstimateTransaction(ElectricalSystemProxy powerCircuit)
//     {
//         CalculateEstimateTrueLoad(powerCircuit);
//
//         //
//         // try
//         // {
//         //     using (LockService.ToLock(Lock.Update))
//         //     {
//         //         ProcessingDocument.ExecuteTransaction(() => CalculateEstimateTrueLoad(powerCircuit),
//         //             TransactionNames.ElectricalSystems);
//         //     }
//         // }
//         // finally
//         // {
//         //     try
//         //     {
//         //         BimEdController.UpdateController.UpdateLockedElementPropertyNames();
//         //     }
//         //     catch (Exception exception)
//         //     {
//         //         Logger.Error(exception);
//         //     }
//         // }
//     }
//
//     private static void CalculateEstimateTrueLoad(ElectricalSystemProxy powerCircuit)
//     {
//         //SetDemandFactorAdditionalToChild(powerCircuit);
//         CalculateParentLoads(powerCircuit, estimateOnly: true);
//     }
//
//     private static void SetDemandFactorAdditionalToChild(ElectricalSystemProxy powerCircuit)
//     {
//         foreach (var electricalPanel in powerCircuit.GetConsumersOf<SwitchBoardUnit>())
//         {
//             //electricalPanel.PowerElectrical.AdditionalDemandFactor = powerCircuit.PowerElectrical.AdditionalDemandFactor;
//             var actual = new ActualElectrical(powerCircuit);
//             SetAdditionalDemandFactor(actual, actual.EstimatedPowerParameters.AdditionalDemandFactor);
//         }
//     }
//
//     #endregion
//
//     public static void ReCalculateEstimateTrueLoad(SwitchBoardUnit electricalPanel, double value, double oldValue)
//     {
//         switch (electricalPanel.PowerParameters.PhasesNumber)
//         {
//             case PhasesNumber.One:
//                 RecalculateLoadTrueEstimate1(electricalPanel, value, oldValue);
//                 break;
//
//             case PhasesNumber.Two:
//             case PhasesNumber.Three:
//                 RecalculateLoadTrueEstimate3(electricalPanel, value, oldValue);
//                 break;
//
//             default:
//                 throw new ArgumentOutOfRangeException(nameof(PowerParameters.PhasesNumber), electricalPanel.PowerParameters.PhasesNumber, new ArgumentOutOfRangeException().Message);
//         }
//     }
//
//     #region ReCalculateEstimateTrueLoad(IPowerElectricalPanel powerPanel, double value, double oldValue)
//
//     private static void RecalculateLoadTrueEstimate1(SwitchBoardUnit switchboardUnit, double value, double oldValue)
//     {
//         var actual = new ActualElectrical(switchboardUnit);
//             
//         if (oldValue.Equals(0))
//         {
//             var loadTrueEstimate = switchboardUnit.GetConsumersOf<ElectricalSystemProxy>()
//                 .Sum(n => n.EstimatedPowerParameters.EstimateTrueLoad);
//
//             AddEstimateTrueLoad(actual, loadTrueEstimate * value);
//         }
//         else
//         {
//             AddEstimateTrueLoad(actual, switchboardUnit.EstimatedPowerParameters.EstimateTrueLoad * (value / oldValue - 1));
//         }
//     }
//
//     private static void RecalculateLoadTrueEstimate3(SwitchBoardUnit switchboardUnit, double value, double oldValue)
//     {
//         try
//         {
//             var actual = new ActualElectrical(switchboardUnit);
//             var threePhases = actual.EstimatedPowerParameters.ThreePhases;
//                 
//             if (oldValue.Equals(0))
//             {
//                 var circuits = switchboardUnit.GetConsumersOf<ElectricalSystemProxy>().ToList();
//                 var circuits1 = circuits.Where(n => n.PowerParameters.PhasesNumber != PhasesNumber.Three).ToList();
//                 var circuits3 = circuits.Where(n => n.PowerParameters.PhasesNumber == PhasesNumber.Three).ToList();
//
//                 var loadTrueEstimate1 = circuits1.Where(n => n.PowerParameters.Phase == Phase.L1).Sum(n => n.EstimatedPowerParameters.EstimateTrueLoad)
//                                         + circuits3.Sum(n => n.EstimatedPowerParameters.ThreePhases.EstimateTrueLoad1);
//                 var loadTrueEstimate2 = circuits1.Where(n => n.PowerParameters.Phase == Phase.L2).Sum(n => n.EstimatedPowerParameters.EstimateTrueLoad)
//                                         + circuits3.Sum(n => n.EstimatedPowerParameters.ThreePhases.EstimateTrueLoad2);
//                 var loadTrueEstimate3 = circuits1.Where(n => n.PowerParameters.Phase == Phase.L3).Sum(n => n.EstimatedPowerParameters.EstimateTrueLoad)
//                                         + circuits3.Sum(n => n.EstimatedPowerParameters.ThreePhases.EstimateTrueLoad3);
//
//                 AddEstimateTrueLoad1(actual, loadTrueEstimate1 * value);
//                 AddEstimateTrueLoad2(actual, loadTrueEstimate2 * value);
//                 AddEstimateTrueLoad3(actual, loadTrueEstimate3 * value);
//             }
//             else
//             {
//                 AddEstimateTrueLoad1(actual, threePhases.EstimateTrueLoad1 * (value / oldValue - 1));
//                 AddEstimateTrueLoad2(actual, threePhases.EstimateTrueLoad2 * (value / oldValue - 1));
//                 AddEstimateTrueLoad3(actual, threePhases.EstimateTrueLoad3 * (value / oldValue - 1));
//             }
//         }
//         catch (Exception ex)
//         {
//             DI.Get<Logger>().Error(ex);
//         }
//     }
//
//     #endregion
//
//     internal static double GetPhaseEstimateCurrent(EstimatedPowerParameters estimatedPowerParameters, Phase phase)
//     {
//         var (apparentLoad, trueLoad, estimateTrueLoad) = GetLoads(estimatedPowerParameters, phase);
//         var demandFactor = trueLoad > 0 ? estimateTrueLoad / trueLoad : 1;
//         var estimateApparentLoad = demandFactor * apparentLoad;
//         var voltage = estimatedPowerParameters.LineToGroundVoltage;
//         var estimateCurrent = voltage > 0 ? estimateApparentLoad / voltage : 0;
//
//         return estimateCurrent;
//     }
//
//     private static (double apparentLoad, double trueLoad, double estimateTrueLoad)
//         GetLoads(EstimatedPowerParameters estimatedPowerParameters, Phase phase)
//     {
//         var threePhases = estimatedPowerParameters.ThreePhases;
//             
//         try
//         {
//             return phase switch
//             {
//                 Phase.Undefined => (0, 0, 0),
//                 Phase.L123 => (0, 0, 0),
//                 Phase.L1 => (threePhases.ApparentLoad1, threePhases.TrueLoad1, threePhases.EstimateTrueLoad1),
//                 Phase.L2 => (threePhases.ApparentLoad2, threePhases.TrueLoad2, threePhases.EstimateTrueLoad2),
//                 Phase.L3 => (threePhases.ApparentLoad3, threePhases.TrueLoad3, threePhases.EstimateTrueLoad3),
//                 _ => (0, 0, 0)
//             };
//         }
//         catch (Exception exception)
//         {
//             DI.Get<Logger>().Error(exception);
//             return (0, 0, 0);
//         }
//     }
//
//     internal static void SetApparentLoad(EstimatedPowerParameters estimatedPowerParameters)
//     {
//         if (LockService.IsLocked(Lock.IsElectricalLoadCalculating))
//             return;
//
//         var threePhases = estimatedPowerParameters.ThreePhases;
//             
//         estimatedPowerParameters.ApparentLoad =
//             threePhases.ApparentLoad1 +
//             threePhases.ApparentLoad2 +
//             threePhases.ApparentLoad3;
//     }
//
//     internal static void SetElementApparentLoad(ElectricalBase electrical)
//     {
//         if (LockService.IsLocked(Lock.IsElectricalLoadCalculating))
//             return;
//             
//         var powerParameters = electrical.PowerParameters;
//             
//         powerParameters.OwnApparentLoad =
//             powerParameters.OwnApparentLoad1 +
//             powerParameters.OwnApparentLoad2 +
//             powerParameters.OwnApparentLoad3;
//     }
//
//     private static void SetEstimateCurrent(ActualElectrical actual, Phase phase, double estimateCurrent)
//     {
//         var threePhases = actual.EstimatedPowerParameters.ThreePhases;
//             
//         switch (phase)
//         {
//             case Phase.L1:
//                 threePhases.EstimateCurrent1 = estimateCurrent;
//                 break;
//
//             case Phase.L2:
//                 threePhases.EstimateCurrent2 = estimateCurrent;
//                 break;
//
//             case Phase.L3:
//                 threePhases.EstimateCurrent3 = estimateCurrent;
//                 break;
//         }
//             
//         threePhases.Asymmetry = CalculateAsymmetry(actual);
//             
//         SetEstimateCurrent(actual.EstimatedPowerParameters);
//     }
//
//     internal static double CalculateAsymmetry(ActualElectrical actual)
//     {
//         var threePhases = actual.EstimatedPowerParameters.ThreePhases; 
//         var currents = new List<double>
//         {
//             threePhases.EstimateCurrent1,
//             threePhases.EstimateCurrent2,
//             threePhases.EstimateCurrent3
//         };
//
//         if (actual.PowerParameters.IsTwoPhases)
//             currents = currents.Where(n => n > 1e-3).ToList();
//
//         return currents.Any()
//             ? (1 - currents.Min() / currents.Max()).Round(3, 3)
//             : 0;
//     }
//     
//     private static void SetEstimateCurrent(EstimatedPowerParameters estimatedPowerParameters)
//     {
//         if (LockService.IsLocked(Lock.IsElectricalLoadCalculating))
//             return;
//             
//         var threePhases = estimatedPowerParameters.ThreePhases;
//             
//         estimatedPowerParameters.Current = new[]
//         {
//             threePhases.EstimateCurrent1,
//             threePhases.EstimateCurrent2,
//             threePhases.EstimateCurrent3
//         }.Max();
//     }
//
//     private static void SetTrueLoad(EstimatedPowerParameters estimatedPowerParameters, bool ignoreLock = false)
//     {
//         if (LockService.IsLocked(Lock.IsElectricalLoadCalculating) && ignoreLock == false)
//             return;
//
//         estimatedPowerParameters.TrueLoad =
//             estimatedPowerParameters.ThreePhases.TrueLoad1 +
//             estimatedPowerParameters.ThreePhases.TrueLoad2 +
//             estimatedPowerParameters.ThreePhases.TrueLoad3;
//     }
//         
//     public static void SetElementApparentLoad1(ElectricalBase electrical, double value)
//     {
//         electrical.PowerParameters.OwnApparentLoad1 = value;
//         SetElementApparentLoad(electrical);
//     }
//
//     public static void SetElementApparentLoad2(ElectricalBase electrical, double value)
//     {
//         electrical.PowerParameters.OwnApparentLoad2 = value;
//         SetElementApparentLoad(electrical);
//     }
//
//     public static void SetElementApparentLoad3(ElectricalBase electrical, double value)
//     {
//         electrical.PowerParameters.OwnApparentLoad3 = value;
//         SetElementApparentLoad(electrical);
//     }
//
//     public static void AddApparentLoad1(EstimatedPowerParameters estimatedPowerParameters, double value)
//     {
//         var threePhases = estimatedPowerParameters.ThreePhases;
//         threePhases.ApparentLoad1 += value;
//         SetApparentLoad(estimatedPowerParameters);
//     }
//
//     public static void AddApparentLoad2(EstimatedPowerParameters estimatedPowerParameters, double value)
//     {
//         var threePhases = estimatedPowerParameters.ThreePhases;
//         threePhases.ApparentLoad2 += value;
//         SetApparentLoad(estimatedPowerParameters);
//     }
//
//     public static void AddApparentLoad3(EstimatedPowerParameters estimatedPowerParameters, double value)
//     {
//         var threePhases = estimatedPowerParameters.ThreePhases;
//         threePhases.ApparentLoad3 += value;
//         SetApparentLoad(estimatedPowerParameters);
//     }
//
//     private static void AddEstimateTrueLoad(ActualElectrical actual, double value)
//     {
//         var estimatedParameters = actual.EstimatedPowerParameters;
//         estimatedParameters.EstimateTrueLoad += value;
//
//         if (LockService.IsLocked(Lock.Calculate) ||
//             LockService.IsLocked(Lock.IsElectricalLoadCalculating) ||
//             actual.PowerParameters.IsThreePhases)
//             return;
//
//         CalculateEstimateCurrents(actual);
//     }
//
//     private static void AddEstimateTrueLoad1(ActualElectrical actual, double value)
//     {
//         var threePhases = actual.EstimatedPowerParameters.ThreePhases;
//         threePhases.EstimateTrueLoad1 += value;
//
//         if (LockService.IsLocked(Lock.Calculate))
//             return;
//
//         CalculatePowerFactor(actual);
//         CalculateEstimateCurrent(actual, Phase.L1);
//     }
//
//     private static void AddEstimateTrueLoad2(ActualElectrical actual, double value)
//     {
//         var estimatedPowerParameters = actual.EstimatedPowerParameters;
//         var threePhases = estimatedPowerParameters.ThreePhases;
//         threePhases.EstimateTrueLoad2 += value;
//
//         if (LockService.IsLocked(Lock.Calculate))
//             return;
//
//         CalculatePowerFactor(actual);
//         CalculateEstimateCurrent(actual, Phase.L2);
//     }
//
//     private static void AddEstimateTrueLoad3(ActualElectrical actual, double value)
//     {
//         var estimatedPowerParameters = actual.EstimatedPowerParameters;
//         var threePhases = estimatedPowerParameters.ThreePhases;
//         threePhases.EstimateTrueLoad3 += value;
//
//         if (LockService.IsLocked(Lock.Calculate))
//             return;
//
//         CalculatePowerFactor(actual);
//         CalculateEstimateCurrent(actual, Phase.L3);
//     }
//
//     // public static void SetEstimateTrueLoad1(ActualElectrical actual, double value)
//     // {
//     //     var estimatedPowerParameters = actual.EstimatedPowerParameters;
//     //     var threePhases = estimatedPowerParameters.ThreePhases;
//     //     threePhases.EstimateTrueLoad1 = new PowerActive(value);
//     //
//     //     if (LockService.IsLocked(Lock.Calculate))
//     //         return;
//     //
//     //     CalculatePowerFactor(actual);
//     //     CalculateEstimateCurrent(estimatedPowerParameters, Phase.L1);
//     // }
//     //
//     // public static void SetEstimateTrueLoad2(ActualElectrical actual, double value)
//     // {
//     //     var estimatedPowerParameters = actual.EstimatedPowerParameters;
//     //     var threePhases = estimatedPowerParameters.ThreePhases;
//     //     threePhases.EstimateTrueLoad2 = new PowerActive(value);
//     //
//     //     if (LockService.IsLocked(Lock.Calculate))
//     //         return;
//     //
//     //     CalculatePowerFactor(actual);
//     //     CalculateEstimateCurrent(estimatedPowerParameters, Phase.L2);
//     // }
//     //
//     // public static void SetEstimateTrueLoad3(ActualElectrical actual, double value)
//     // {
//     //     var estimatedPowerParameters = actual.EstimatedPowerParameters;
//     //     var threePhases = estimatedPowerParameters.ThreePhases;
//     //     threePhases.EstimateTrueLoad3 = new PowerActive(value);
//     //
//     //     if (LockService.IsLocked(Lock.Calculate))
//     //         return;
//     //
//     //     CalculatePowerFactor(actual);
//     //     CalculateEstimateCurrent(estimatedPowerParameters, Phase.L3);
//     // }
//
//     public static void AddTrueLoad1(EstimatedPowerParameters estimatedPowerParameters, double value)
//     {
//         var threePhases = estimatedPowerParameters.ThreePhases;
//         threePhases.TrueLoad1 += value;
//         SetTrueLoad(estimatedPowerParameters);
//     }
//
//     public static void AddTrueLoad2(EstimatedPowerParameters estimatedPowerParameters, double value)
//     {
//         var threePhases = estimatedPowerParameters.ThreePhases;
//         threePhases.TrueLoad2 += value;
//         SetTrueLoad(estimatedPowerParameters);
//     }
//
//     public static void AddTrueLoad3(EstimatedPowerParameters estimatedPowerParameters, double value)
//     {
//         var threePhases = estimatedPowerParameters.ThreePhases;
//         threePhases.TrueLoad3 += value;
//         SetTrueLoad(estimatedPowerParameters);
//     }
//
//     private static void SetTotalDemandFactor(ActualElectrical actual, double value)
//     {
//         value = value.Round(3, 3);
//         var oldValue = actual.EstimatedPowerParameters.TotalDemandFactor;
//
//         if (Math.Abs(oldValue - value) < 1e-3) return;
//
//         actual.EstimatedPowerParameters.TotalDemandFactor = value;
//
//         if (LockService.IsLocked(Lock.SpecifyAdditionalDemandFactor) ||
//             actual.Electrical is not ElectricalSystemProxy)
//             return;
//
//         SpecifyAdditionalDemandFactor(actual, value, oldValue);
//     }
//         
//     private static void SpecifyAdditionalDemandFactor(ActualElectrical actual, double value, double oldValue)
//     {
//         var additionalDemandFactor = actual.EstimatedPowerParameters.AdditionalDemandFactor;
//             
//         if (Math.Abs(oldValue) < 1e-3)
//         {
//             additionalDemandFactor = 1;
//             oldValue = actual.EstimatedPowerParameters.TotalDemandFactor;
//         }
//
//         SetAdditionalDemandFactor(actual, additionalDemandFactor * value / oldValue);
//     }
//
//     private static void SetAdditionalDemandFactor(ActualElectrical actual, double value)
//     {
//         using (LockService.ToLock(Lock.SpecifyAdditionalDemandFactor))
//         {
//             value = value.Round(3, 3);
//             var oldValue = actual.EstimatedPowerParameters.AdditionalDemandFactor;
//
//             if (Math.Abs(oldValue - value) < 1e-3)
//                 return;
//
//             actual.EstimatedPowerParameters.AdditionalDemandFactor = value;
//
//             if (LockService.IsLocked(Lock.Calculate))
//                 return;
//
//             ReCalculateEstimateTrueLoad(actual.Electrical, value, oldValue);
//         }
//     }
//
// }
//

// using CoLa.BimEd.Logic.Model.Electrical.Base;
// using CoLa.BimEd.Logic.Model.RevitProxy;
//
// internal record ActualElectrical
// {
//     internal ActualElectrical(ElectricalBase electrical)
//     {
//         Electrical = electrical;
//         PowerParameters = Electrical.PowerParameters;
//         EstimatedPowerParameters = electrical.BaseConnector.GetEstimatedPowerParameters();
//     }
//     public ElectricalBase Electrical { get; }
//     public PowerParameters PowerParameters { get; }
//     public EstimatedPowerParameters EstimatedPowerParameters { get; }
//     public LoadClassificationProxy LoadClassification { get; set; }
// }
