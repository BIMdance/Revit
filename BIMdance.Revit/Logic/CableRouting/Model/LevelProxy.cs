namespace BIMdance.Revit.Logic.CableRouting.Model;

public class LevelProxy : ElementProxy
{
    public double Elevation { get; set; }
    public List<RoomProxy> Rooms { get; set; } = new();
}