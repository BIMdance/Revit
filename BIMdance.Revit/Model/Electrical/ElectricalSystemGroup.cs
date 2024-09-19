namespace BIMdance.Revit.Model.Electrical;

public sealed class ElectricalSystemGroup : ElectricalBase
{
    public ElectricalSystemGroup() { }
    
    internal ElectricalSystemGroup(
        string name,
        IReadOnlyCollection<ElectricalSystemProxy> electricalCircuits,
        PhasesNumber phasesNumber = PhasesNumber.Undefined)
    {
        if (electricalCircuits == null)
            throw new ArgumentNullException(nameof(electricalCircuits));

        if (electricalCircuits.IsEmpty())
            throw new ArgumentException(@"There must be some items in the collection.", nameof(electricalCircuits));

        if (electricalCircuits.Select(n => n.SystemType).Distinct().Count() > 1)
            throw new InvalidOperationException($"Different {nameof(SystemType)} of items in the {nameof(electricalCircuits)}.");
            
        Name = name;

        var firstCircuit = electricalCircuits.First();
            
        if (firstCircuit.BaseConnector.IsPower)
        {
            var maxCircuitPhasesNumber = electricalCircuits.Max(n => n.PowerParameters.PhasesNumber);
            var groupPhasesNumber = phasesNumber > maxCircuitPhasesNumber ? phasesNumber : maxCircuitPhasesNumber;
            var voltage = firstCircuit.BaseConnector.PowerParameters.LineToGroundVoltage;
            SetBaseConnector(new ConnectorProxy(this, 1, groupPhasesNumber, voltage));
        }
        else
        {
            SetBaseConnector(new ConnectorProxy(this, 1, SystemType));
        }

        AddRange(electricalCircuits);

        // Power = new PowerElectrical(this, groupPhasesNumber);
    }
}