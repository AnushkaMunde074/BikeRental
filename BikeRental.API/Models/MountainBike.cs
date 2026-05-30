namespace BikeRental.API.Models;

public class MountainBike
{
    public int Id { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int GearCount { get; set; }
    public string SuspensionType { get; set; } = string.Empty;
    public string FrameMaterial { get; set; } = string.Empty;
    public string Terrain { get; set; } = string.Empty;
    public decimal DailyRate { get; set; }
    public double WeightKg { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
