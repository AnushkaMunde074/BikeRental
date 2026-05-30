namespace BikeRental.API.DTOs;

public record BeachCruiserDto(int Id, string Name, string Color, string FrameSize, string Description, decimal PricePerDay, bool IsAvailable);

public record MountainBikeDto(int Id, string ModelName, string Brand, int GearCount, string SuspensionType, string FrameMaterial, string Terrain, decimal DailyRate, double WeightKg, bool IsAvailable);

public record AccessoryDto(int Id, string Name, string Category, string Description, decimal UnitPrice, int StockCount, string CompatibleWith);

public record RentBikeRequest(string BikeType, int BikeId);

public record RentBikeResponse(bool Success, string Message);

public record AccessoryOrderItemRequest(int AccessoryId, int Quantity);

public record OrderResponse(bool Success, string Message, decimal Subtotal, decimal DiscountAmount, decimal Total, bool BundleDiscountApplied);

public record ResetResponse(bool Success, string Message);
