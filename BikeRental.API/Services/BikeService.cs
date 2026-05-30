using BikeRental.API.Data;
using BikeRental.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BikeRental.API.Services;

public interface IBikeService
{
    Task<List<BeachCruiserDto>> GetBeachCruisersAsync();
    Task<List<MountainBikeDto>> GetMountainBikesAsync();
    Task<RentBikeResponse> RentBikeAsync(RentBikeRequest request);
    Task<ResetResponse> ResetFleetAsync();
}

public class BikeService : IBikeService
{
    private readonly BikeRentalDbContext _db;
    private readonly ILogger<BikeService> _logger;

    public BikeService(BikeRentalDbContext db, ILogger<BikeService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<BeachCruiserDto>> GetBeachCruisersAsync()
    {
        return await _db.BeachCruisers
            .Select(b => new BeachCruiserDto(b.Id, b.Name, b.Color, b.FrameSize, b.Description, b.PricePerDay, b.IsAvailable))
            .ToListAsync();
    }

    public async Task<List<MountainBikeDto>> GetMountainBikesAsync()
    {
        return await _db.MountainBikes
            .Select(b => new MountainBikeDto(b.Id, b.ModelName, b.Brand, b.GearCount, b.SuspensionType, b.FrameMaterial, b.Terrain, b.DailyRate, b.WeightKg, b.IsAvailable))
            .ToListAsync();
    }

    public async Task<RentBikeResponse> RentBikeAsync(RentBikeRequest request)
    {
        if (request.BikeType == "beach")
        {
            var bike = await _db.BeachCruisers.FindAsync(request.BikeId);
            if (bike == null)
                return new RentBikeResponse(false, "Bike not found.");
            if (!bike.IsAvailable)
                return new RentBikeResponse(false, "This bike is already rented.");

            bike.IsAvailable = false;
            _db.Rentals.Add(new Models.Rental { BikeType = "beach", BikeId = bike.Id });
        }
        else if (request.BikeType == "mountain")
        {
            var bike = await _db.MountainBikes.FindAsync(request.BikeId);
            if (bike == null)
                return new RentBikeResponse(false, "Bike not found.");
            if (!bike.IsAvailable)
                return new RentBikeResponse(false, "This bike is already rented.");

            bike.IsAvailable = false;
            _db.Rentals.Add(new Models.Rental { BikeType = "mountain", BikeId = bike.Id });
        }
        else
        {
            return new RentBikeResponse(false, "Invalid bike type. Use 'beach' or 'mountain'.");
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Bike rented: {BikeType} #{BikeId}", request.BikeType, request.BikeId);
        return new RentBikeResponse(true, "Rental confirmed. Enjoy your ride!");
    }

    public async Task<ResetResponse> ResetFleetAsync()
    {
        // Reset beach cruisers to seed availability
        var beachBikes = await _db.BeachCruisers.ToListAsync();
        var defaultUnavailableBeach = new HashSet<int> { 3, 6 };
        foreach (var bike in beachBikes)
            bike.IsAvailable = !defaultUnavailableBeach.Contains(bike.Id);

        // Reset mountain bikes to seed availability
        var mountainBikes = await _db.MountainBikes.ToListAsync();
        var defaultUnavailableMountain = new HashSet<int> { 103, 106 };
        foreach (var bike in mountainBikes)
            bike.IsAvailable = !defaultUnavailableMountain.Contains(bike.Id);

        // Reset accessory stock
        var accessories = await _db.Accessories.ToListAsync();
        var defaultStock = new Dictionary<int, int> { { 1, 15 }, { 2, 8 }, { 3, 20 }, { 4, 6 } };
        foreach (var acc in accessories)
            if (defaultStock.TryGetValue(acc.Id, out var stock))
                acc.StockCount = stock;

        // Close active rentals
        var activeRentals = await _db.Rentals.Where(r => r.ReturnedAt == null).ToListAsync();
        foreach (var rental in activeRentals)
            rental.ReturnedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        _logger.LogInformation("Fleet reset to defaults");
        return new ResetResponse(true, "Fleet reset. All bikes returned. Accessories restocked.");
    }
}
