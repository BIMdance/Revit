namespace BIMdance.Revit.Model.Electrical;

public class CurrentTimePoint : IComparable
{
    public CurrentTimePoint(double current, double time)
    {
        Guid = Guid.NewGuid();
        Current = current;
        Time = time;
    }

    public Guid Guid { get; }
    public double Current { get; set; }
    public double Time { get; set; }

    public override bool Equals(object obj)
    {
        if (obj == null || this.GetType() != obj.GetType())
            return false;

        var otherCurrentTimePoint = (CurrentTimePoint)obj;

        return Guid == otherCurrentTimePoint.Guid ||
               Current.IsEqualTo(otherCurrentTimePoint.Current) &&
               Time.IsEqualTo(otherCurrentTimePoint.Time);
    }

    public override int GetHashCode() => Guid.GetHashCode();

    public int CompareTo(object obj)
    {
        if (obj is not CurrentTimePoint point)
            return 1;

        return Equals(Current, point.Current)
            ? Time < point.Time ? -1 :
              Time > point.Time ? 1 : 0
            : Current < point.Current ? -1 :
              Current > point.Current ? 1 : 0;
    }

    public override string ToString() => $"{Current} ({Time})";
}