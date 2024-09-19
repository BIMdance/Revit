namespace BIMdance.Revit.Model.Electrical;

public class PowerParameters
{
    public PowerParameters(PhasesNumber phasesNumber, double lineToGroundVoltage)
    {
        LineToGroundVoltage = lineToGroundVoltage;
        PhasesNumber = phasesNumber;
            
        if (PhasesNumber is PhasesNumber.Three)
            Phase = Phase.L123;
    }

    public Phase Phase { get; set; } = Phase.Undefined;
    public PhasesNumber PhasesNumber { get; set; }
    public bool IsOnePhase => PhasesNumber == PhasesNumber.One;
    public bool IsTwoPhases => PhasesNumber == PhasesNumber.Two;
    public bool IsThreePhases => PhasesNumber == PhasesNumber.Three;
    public LoadClassificationProxy LoadClassification { get; set; }
    public double LineToGroundVoltage { get; set; }
    public double OwnApparentLoad { get; set; }
    public double OwnApparentLoad1 { get; set; }
    public double OwnApparentLoad2 { get; set; }
    public double OwnApparentLoad3 { get; set; }
    public double OwnPowerFactor { get; set; }

    public void SetBalancedOwnApparentLoad(double apparentLoad)
    {
        OwnApparentLoad = apparentLoad;
            
        switch (PhasesNumber)
        {
            case PhasesNumber.Two:
                var apparentLoadDiv2 = apparentLoad / 2;
                OwnApparentLoad1 = apparentLoadDiv2;
                OwnApparentLoad2 = apparentLoadDiv2;
                return;
                
            case PhasesNumber.Three:
                var apparentLoadDiv3 = apparentLoad / 3;
                OwnApparentLoad1 = apparentLoadDiv3;
                OwnApparentLoad2 = apparentLoadDiv3;
                OwnApparentLoad3 = apparentLoadDiv3;
                return;
        }
    }

    public void SetUnbalancedOwnApparentLoad(double apparentLoad1, double apparentLoad2, double apparentLoad3)
    {
        switch (PhasesNumber)
        {
            case PhasesNumber.One:
                OwnApparentLoad1 = apparentLoad1;
                return;
                
            case PhasesNumber.Two:
                OwnApparentLoad = apparentLoad1 + apparentLoad2;
                OwnApparentLoad1 = apparentLoad1;
                OwnApparentLoad2 = apparentLoad2;
                return;
                
            case PhasesNumber.Three:
                OwnApparentLoad = apparentLoad1 + apparentLoad2 + apparentLoad3;
                OwnApparentLoad1 = apparentLoad1;
                OwnApparentLoad2 = apparentLoad2;
                OwnApparentLoad3 = apparentLoad3;
                return;
        }
    }
}