namespace BIMdance.Revit.ObsoleteAPI;

public static partial class Revit2016
{
    public static class Schedules
    {
        public static void SetHasTotal(ScheduleField scheduleField)
        {
            if (scheduleField.CanTotal())
                scheduleField.HasTotals = true;
        }
    }
}