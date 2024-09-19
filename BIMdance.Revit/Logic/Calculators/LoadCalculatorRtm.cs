// using System;
// using System.Collections.Generic;
// using System.Linq;
// using CoLa.BimEd.Logic.Model.Electrical;
// using CoLa.BimEd.Logic.Model.Electrical.Base;
// using CoLa.BimEd.Logic.SDK.Collections;
// using CoLa.BimEd.Logic.SDK.Locks;
//
// namespace CoLa.BimEd.Logic.Electrical.Calculators;
//
// public class LoadCalculatorRtm
// {
//     private readonly List<double> _demandFactors = new();
//     private readonly List<double> _powerFactors = new();
//     private readonly List<double> _tangents = new();
//     private readonly List<double> _trueLoads = new();
//     private readonly List<double> _apparentLoads = new();
//
//     public LoadCalculatorRtm(ElectricalBase electricalBase, OperatingMode operatingMode = null) :
//         base(electricalBase, operatingMode) { }
//
//     internal void CalculateLoads(bool estimateOnly = false, bool calculateParentLoads = false)
//     {
//         if (LockService.IsLocked(Lock.Calculate))
//             return;
//
//         UpdateAllElements();
//
//         if (!Electrical.IsPower)
//         {
//             return;
//         }
//
//         if (AllElements.IsEmpty())
//         {
//             Reset();
//
//             if (calculateParentLoads)
//                 CalculateParentLoads(estimateOnly);
//
//             return;
//         }
//
//         IsLoadCalculating = true;
//
//         foreach (var element in AllElements)
//         {
//             var demandFactorValue = element.PowerParameters.LoadClassification.DemandFactor.Values.First().Factor;
//             var powerFactorValue = element.PowerParameters.OwnPowerFactor;
//             var tangent = Math.Tan(Math.Acos(powerFactorValue));
//             var apparentLoad = element.PowerParameters.OwnApparentLoad;
//             var trueLoad = apparentLoad * powerFactorValue;
//
//             _demandFactors.Add(demandFactorValue);
//             _powerFactors.Add(powerFactorValue);
//             _tangents.Add(tangent);
//             _apparentLoads.Add(apparentLoad);
//             _trueLoads.Add(trueLoad);
//         }
//
//         ApparentLoad = _apparentLoads.Sum(e => e);
//         TrueLoad = _trueLoads.Sum(e => e);
//
//         var ne = GetN(TrueLoad);
//         var estimateReactiveLoad = GetEstimateReactiveLoad(ne);
//         var sumKp = GetSumOfProducts(_trueLoads, _demandFactors);
//
//         DemandFactor = sumKp / TrueLoad;
//         var kp = RtmKpTable.GetKp(ne, DemandFactor);
//
//         EstimateTrueLoad = kp * sumKp;
//         EstimateApparentLoad = Math.Sqrt(Math.Pow(EstimateTrueLoad, 2) + Math.Pow(estimateReactiveLoad, 2));
//         EstimateCurrent = EstimateApparentLoad / (3 * Voltage);
//         PowerFactor = EstimateApparentLoad > 0
//             ? EstimateTrueLoad / EstimateApparentLoad
//             : 1;
//
//         UpdateParameters();
//
//         if (calculateParentLoads)
//             CalculateParentLoads(estimateOnly);
//
//         IsLoadCalculating = false;
//     }
//
//     public override void CalculateParentLoads(bool estimateOnly)
//     {
//         var source = Electrical.Source;
//
//         if (source == null)
//             return;
//
//         var calculator = new LoadCalculatorRtm(Electrical.Source, OperatingMode);
//         calculator.CalculateLoads(estimateOnly, calculateParentLoads: true);
//     }
//
//     private static double GetSumOfProducts(List<double> powers, List<double> coefficients)
//     {
//         if (powers.Count != coefficients.Count)
//             throw new ArgumentException($"The sizes of the lists do not match: powers.Count != coefficients.Count ({powers.Count} != {coefficients.Count}).");
//
//         return powers.Select((t, i) => t * coefficients[i]).Sum();
//     }
//
//     private int GetN(double trueLoad)
//     {
//         var sumOfSquaresOfTrueLoads = _trueLoads.Sum(e => e * e);
//         return (int)(trueLoad * trueLoad / sumOfSquaresOfTrueLoads);
//     }
//
//     private double GetEstimateReactiveLoad(int ne)
//     {
//         var sumKpTan = _trueLoads.Select((t, i) => t * _demandFactors[i] * _tangents[i]).Sum();
//         return ne <= 10 ? 1.1 * sumKpTan : sumKpTan;
//     }
// }