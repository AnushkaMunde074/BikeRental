namespace BikeRental.API.Settings;

public class FleetSettings
{
    public HashSet<int> UnavailableBeachIds { get; set; } = new();
    public HashSet<int> UnavailableMountainIds { get; set; } = new();
    public Dictionary<int, int> AccessoryDefaults { get; set; } = new();
}
