using BIMdance.Revit.Model.Electrical;
using BIMdance.Revit.Model.Electrical.Base;

namespace BIMdance.Revit.Model.RevitProxy;

public class DistributionSystemProxy : ElementProxy, IPrototype<DistributionSystemProxy>, IPropertyPrototype<DistributionSystemProxy>
{
    public DistributionSystemProxy() { }
    public DistributionSystemProxy(int revitId, string name) : base(revitId, name) { }
    private DistributionSystemProxy(DistributionSystemProxy prototype) => PullProperties(prototype);
    public DistributionSystemProxy Clone() => new(this);
    public void PullProperties(DistributionSystemProxy prototype)
    {
        this.Name = prototype.Name;
        this.RevitId = prototype.RevitId;
        this.ElectricalPhaseConfiguration = prototype.ElectricalPhaseConfiguration;
        this.LineToGroundVoltage = prototype.LineToGroundVoltage;
        this.LineToLineVoltage = prototype.LineToLineVoltage;
        this.PhasesNumber = prototype.PhasesNumber;
        this.NumWires = prototype.NumWires;
    }
    
    public ElectricalPhaseConfigurationProxy ElectricalPhaseConfiguration { get; set; } = ElectricalPhaseConfigurationProxy.Undefined;
    public VoltageTypeProxy LineToGroundVoltage { get; set; }
    public VoltageTypeProxy LineToLineVoltage { get; set; }
    public PhasesNumber PhasesNumber { get; set; } = PhasesNumber.One;
    public int NumWires { get; set; } = 3;
    public bool CanBeDeleted { get; set; } = true;

    public static DistributionSystemProxy Create1Phase(int revitId, string name, VoltageTypeProxy lineToGroundVoltage) =>
        new(revitId, name)
        {
            LineToGroundVoltage = lineToGroundVoltage
        };

    public static DistributionSystemProxy Create3PhasesDelta(int revitId, string name, VoltageTypeProxy lineToLineVoltage) =>
        new(revitId, name)
        {
            LineToLineVoltage = lineToLineVoltage,
            PhasesNumber = PhasesNumber.Three,
            ElectricalPhaseConfiguration = ElectricalPhaseConfigurationProxy.Delta,
            NumWires = 4,
        };
    
    public static DistributionSystemProxy Create3PhasesWye(int id, string name,
        VoltageTypeProxy lineToLineVoltage,
        VoltageTypeProxy lineToGroundVoltage = null) =>
        new(id, name)
        {
            LineToGroundVoltage = lineToGroundVoltage,
            LineToLineVoltage = lineToLineVoltage,
            PhasesNumber = PhasesNumber.Three,
            ElectricalPhaseConfiguration = ElectricalPhaseConfigurationProxy.Wye,
            NumWires = 4, //lineToGroundVoltage != null ? 4 : 3,
        };

    public bool IsLowVoltage() =>
        LineToLineVoltage is { ActualValue: <= 1000 } ||
        LineToGroundVoltage is { ActualValue: <= 1000 };

    public bool IsMediumVoltage() =>
        LineToLineVoltage is { ActualValue: > 1000 } and { ActualValue: <= 35000 };

    /// <summary>
    /// Проверка электрической совместимости систем 
    /// </summary>
    /// <param name="target">Система, для которой выполняется проверка совместимости текущей системы</param>
    /// <param name="relativeTolerance">Допустимое отколнение</param>
    /// <returns></returns>
    /// <remarks>
    /// Проверяется физическая возможность подключения.<br/>
    /// Например для того, чтобы не допустить подключение систем низкого напряжения к системам средненго напряжения. Или систем 400 В к системам 690 В.
    /// </remarks>
    public bool IsCompatible(DistributionSystemProxy target, double relativeTolerance = 0.1)
    {
        if (target == null)
            return false;
            
        return PhasesNumber switch
        {
            PhasesNumber.One
                when LineToGroundVoltage != null &&
                     target.LineToGroundVoltage != null =>
                Math.Abs(LineToGroundVoltage.ActualValue - target.LineToGroundVoltage.ActualValue) < relativeTolerance * LineToGroundVoltage.ActualValue, 

            PhasesNumber.Three
                when LineToLineVoltage != null &&
                     target.PhasesNumber == PhasesNumber.Three &&
                     target.LineToLineVoltage != null =>
                Math.Abs(LineToLineVoltage.ActualValue - target.LineToLineVoltage.ActualValue) < relativeTolerance * LineToLineVoltage.ActualValue,

            _ => false
        };
    }
    
    /// <summary>
    /// Проверка электрической совместимости систем 
    /// </summary>
    /// <param name="electrical">Элемент, для которого выполняется проверка совместимости текущей системы</param>
    /// <param name="relativeTolerance">Допустимое отколнение</param>
    /// <returns></returns>
    /// <remarks>
    /// Проверяется физическая возможность подключения.<br/>
    /// Например для того, чтобы не допустить подключение элементов низкого напряжения к системам средненго напряжения. Или элементов 400 В к системам 690 В и наоборот.
    /// </remarks>
    public bool IsCompatible(ElectricalBase electrical, double relativeTolerance = 0.1)
    {
        var powerParameters = electrical.PowerParameters;
        
        if (powerParameters == null || powerParameters.PhasesNumber > PhasesNumber)
            return false;
            
        return PhasesNumber switch
        {
            PhasesNumber.One
                when LineToGroundVoltage != null =>
                Math.Abs(LineToGroundVoltage.ActualValue - powerParameters.LineToGroundVoltage) < relativeTolerance * LineToGroundVoltage.ActualValue, 

            PhasesNumber.Three
                when LineToLineVoltage != null &&
                     powerParameters.PhasesNumber == PhasesNumber.Three =>
                Math.Abs(LineToLineVoltage.ActualValue - powerParameters.LineToGroundVoltage * MathConstants.Sqrt3) < relativeTolerance * LineToLineVoltage.ActualValue,

            _ => false
        };
    }

    /// <summary>
    /// Проверка совместимсти систем в Revit
    /// </summary>
    /// <param name="target">Система, для которой выполняется проверка совместимости текущей системы</param>
    /// <returns></returns>
    /// <remarks>
    /// Проверяется возможность подключения, ограниченная только логикой совместимости систем Revit.<br/>
    /// Например, если у одной системы установлено максимальное напряжение 10000 В, а минимальное напряжение 0 В,
    /// то к ней возможно подключить любую систему, у которой актуальное значение лежит в этих пределеах.</remarks>
    public bool IsPossibleConnectTo(DistributionSystemProxy target)
    {
        if (target == null)
            return false;
            
        return PhasesNumber switch
        {
            PhasesNumber.One
                when LineToGroundVoltage != null &&
                     target.LineToGroundVoltage != null =>
                LineToGroundVoltage.ActualValue >= target.LineToGroundVoltage.MinValue &&
                LineToGroundVoltage.ActualValue <= target.LineToGroundVoltage.MaxValue,

            PhasesNumber.Three
                when LineToLineVoltage != null &&
                     target.PhasesNumber == PhasesNumber.Three &&
                     target.LineToLineVoltage != null =>
                LineToLineVoltage.ActualValue >= target.LineToLineVoltage.MinValue &&
                LineToLineVoltage.ActualValue <= target.LineToLineVoltage.MaxValue,

            _ => false
        };
    }

    public double GetLineToGroundVoltage() =>
        LineToGroundVoltage?.ActualValue ??
        LineToLineVoltage?.ActualValue ?? 0
        / MathConstants.Sqrt3;
    
    public override string ToString() => $"{base.ToString()} {PhasesNumber} | {ElectricalPhaseConfiguration} | {NumWires} | {LineToGroundVoltage?.Name} | {LineToLineVoltage?.Name}";

    public static IEqualityComparer<DistributionSystemProxy> PropertiesEqualityComparer { get; } = new DistributionSystemProxyEqualityComparer();

    private sealed class DistributionSystemProxyEqualityComparer : IEqualityComparer<DistributionSystemProxy>
    {
        public bool Equals(DistributionSystemProxy x, DistributionSystemProxy y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
                
            return Equals(x.Name, y.Name) &&
                   x.ElectricalPhaseConfiguration == y.ElectricalPhaseConfiguration &&
                   Equals(x.LineToGroundVoltage, y.LineToGroundVoltage) &&
                   Equals(x.LineToLineVoltage, y.LineToLineVoltage) &&
                   x.PhasesNumber == y.PhasesNumber &&
                   x.NumWires == y.NumWires;
        }

        public int GetHashCode(DistributionSystemProxy obj)
        {
            unchecked
            {
                var hashCode = (int)obj.ElectricalPhaseConfiguration;
                hashCode = (hashCode * 397) ^ (obj.LineToGroundVoltage != null ? obj.LineToGroundVoltage.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.LineToLineVoltage != null ? obj.LineToLineVoltage.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)obj.PhasesNumber;
                hashCode = (hashCode * 397) ^ obj.NumWires;
                return hashCode;
            }
        }
    }
}