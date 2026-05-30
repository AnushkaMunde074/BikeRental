using BikeRental.API.Data;
using BikeRental.API.DTOs;
using BikeRental.API.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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
    private readonly FleetSettings _fleet;

    public BikeService(BikeRentalDbContext db, ILogger<BikeService> logger, IOptions<FleetSettings> fleetOptions)
    {
        _db = db;
        _logger = logger;
        _fleet = fleetOptions.Value;
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
        foreach (var bike in beachBikes)
            bike.IsAvailable = !_fleet.UnavailableBeachIds.Contains(bike.Id);

        // Reset mountain bikes to seed availability
        var mountainBikes = await _db.MountainBikes.ToListAsync();
        foreach (var bike in mountainBikes)
            bike.IsAvailable = !_fleet.UnavailableMountainIds.Contains(bike.Id);

        // Reset accessory stock
        var accessories = await _db.Accessories.ToListAsync();
        foreach (var acc in accessories)
            if (_fleet.AccessoryDefaults.TryGetValue(acc.Id, out var stock))
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
