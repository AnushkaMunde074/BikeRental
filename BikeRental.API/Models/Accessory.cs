namespace BikeRental.API.Models;

public class Accessory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int StockCount { get; set; }
    public string CompatibleWith { get; set; } = "all"; // "mountain", "beach", "all"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
