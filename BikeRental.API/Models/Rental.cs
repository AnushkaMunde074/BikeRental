namespace BikeRental.API.Models;

public class Rental
{
    public int Id { get; set; }
    public string BikeType { get; set; } = string.Empty; // "beach" or "mountain"
    public int BikeId { get; set; }
    public DateTime RentedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReturnedAt { get; set; }
    public bool IsActive => ReturnedAt == null;
}
