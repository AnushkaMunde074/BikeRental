using BikeRental.API.Data;
using BikeRental.API.DTOs;
using BikeRental.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeRental.API.Services;

public interface IAccessoryService
{
    Task<List<AccessoryDto>> GetAccessoriesAsync(string? bikeType = null);
    Task<OrderResponse> ProcessOrderAsync(List<AccessoryOrderItemRequest> items);
}

public class AccessoryService : IAccessoryService
{
    private readonly BikeRentalDbContext _db;
    private readonly ILogger<AccessoryService> _logger;

    // Bundle deal: Water Bottle (1) + Bike Light (3) = 10% off
    private static readonly HashSet<int> BundleIds = new() { 1, 3 };
    private const decimal BundleDiscountRate = 0.10m;

    public AccessoryService(BikeRentalDbContext db, ILogger<AccessoryService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<AccessoryDto>> GetAccessoriesAsync(string? bikeType = null)
    {
        var query = _db.Accessories.AsQueryable();

        if (!string.IsNullOrEmpty(bikeType))
            query = query.Where(a => a.CompatibleWith == "all" || a.CompatibleWith == bikeType);

        return await query
            .Select(a => new AccessoryDto(a.Id, a.Name, a.Category, a.Description, a.UnitPrice, a.StockCount, a.CompatibleWith))
            .ToListAsync();
    }

    public async Task<OrderResponse> ProcessOrderAsync(List<AccessoryOrderItemRequest> items)
    {
        if (items == null || items.Count == 0 || items.All(i => i.Quantity <= 0))
            return new OrderResponse(false, "No items with quantity > 0.", 0, 0, 0, false);

        var validItems = items.Where(i => i.Quantity > 0).ToList();
        var accessoryIds = validItems.Select(i => i.AccessoryId).ToList();
        var accessories = await _db.Accessories.Where(a => accessoryIds.Contains(a.Id)).ToListAsync();

        // Validate stock
        foreach (var item in validItems)
        {
            var accessory = accessories.FirstOrDefault(a => a.Id == item.AccessoryId);
            if (accessory == null)
                return new OrderResponse(false, $"Accessory #{item.AccessoryId} not found.", 0, 0, 0, false);
            if (accessory.StockCount < item.Quantity)
                return new OrderResponse(false, $"Insufficient stock for '{accessory.Name}'. Available: {accessory.StockCount}.", 0, 0, 0, false);
        }

        // Calculate subtotal
        decimal subtotal = 0;
        var orderItems = new List<OrderItem>();

        foreach (var item in validItems)
        {
            var accessory = accessories.First(a => a.Id == item.AccessoryId);
            var lineTotal = accessory.UnitPrice * item.Quantity;
            subtotal += lineTotal;

            accessory.StockCount -= item.Quantity;
            orderItems.Add(new OrderItem
            {
                AccessoryId = accessory.Id,
                Quantity = item.Quantity,
                UnitPrice = accessory.UnitPrice
            });
        }

        // Check bundle discount
        var orderedIds = validItems.Select(i => i.AccessoryId).ToHashSet();
        bool bundleApplied = BundleIds.IsSubsetOf(orderedIds);
        decimal discount = bundleApplied ? subtotal * BundleDiscountRate : 0;
        decimal total = subtotal - discount;

        // Save order
        var order = new Order
        {
            Subtotal = subtotal,
            DiscountAmount = discount,
            Total = total,
            BundleDiscountApplied = bundleApplied,
            Items = orderItems
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Order #{OrderId} placed. Total: {Total}", order.Id, total);
        return new OrderResponse(true, "Order placed successfully!", subtotal, discount, total, bundleApplied);
    }
}
