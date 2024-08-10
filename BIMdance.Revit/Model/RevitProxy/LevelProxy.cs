namespace BIMdance.Revit.Model.RevitProxy;

public class LevelProxy : ElementProxy
{
    public double Elevation { get; set; }
    public List<RoomProxy> Rooms { get; set; } = new();
}