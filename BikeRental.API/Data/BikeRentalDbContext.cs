using BikeRental.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeRental.API.Data;

public class BikeRentalDbContext : DbContext
{
    public BikeRentalDbContext(DbContextOptions<BikeRentalDbContext> options) : base(options) { }

    public DbSet<BeachCruiser> BeachCruisers => Set<BeachCruiser>();
    public DbSet<MountainBike> MountainBikes => Set<MountainBike>();
    public DbSet<Accessory> Accessories => Set<Accessory>();
    public DbSet<Rental> Rentals => Set<Rental>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BeachCruiser>(e =>
        {
            e.HasKey(b => b.Id);
            e.Property(b => b.PricePerDay).HasColumnType("decimal(8,2)");
        });

        modelBuilder.Entity<MountainBike>(e =>
        {
            e.HasKey(b => b.Id);
            e.Property(b => b.DailyRate).HasColumnType("decimal(8,2)");
        });

        modelBuilder.Entity<Accessory>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.UnitPrice).HasColumnType("decimal(8,2)");
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.Subtotal).HasColumnType("decimal(10,2)");
            e.Property(o => o.DiscountAmount).HasColumnType("decimal(10,2)");
            e.Property(o => o.Total).HasColumnType("decimal(10,2)");
        });

        modelBuilder.Entity<OrderItem>(e =>
        {
            e.HasKey(oi => oi.Id);
            e.Property(oi => oi.UnitPrice).HasColumnType("decimal(8,2)");
            e.HasOne(oi => oi.Order).WithMany(o => o.Items).HasForeignKey(oi => oi.OrderId);
            e.HasOne(oi => oi.Accessory).WithMany().HasForeignKey(oi => oi.AccessoryId);
        });

        // Seed data migrated from legacy JSON/XML files
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BeachCruiser>().HasData(
            new BeachCruiser { Id = 1, Name = "Sunset Drifter", Color = "Coral Orange", FrameSize = "17\"", Description = "A smooth-riding cruiser perfect for sunset beach rides.", PricePerDay = 14.99m, IsAvailable = true },
            new BeachCruiser { Id = 2, Name = "Ocean Breeze", Color = "Sky Blue", FrameSize = "15\"", Description = "Light and breezy, built for coastal paths.", PricePerDay = 12.99m, IsAvailable = true },
            new BeachCruiser { Id = 3, Name = "Sandy Glider", Color = "Sand Beige", FrameSize = "18\"", Description = "Wide tires for sandy terrain. Glides where others sink.", PricePerDay = 15.99m, IsAvailable = false },
            new BeachCruiser { Id = 4, Name = "Tidal Wave", Color = "Deep Teal", FrameSize = "16\"", Description = "Bold color, smooth ride. Turns heads on the boardwalk.", PricePerDay = 13.99m, IsAvailable = true },
            new BeachCruiser { Id = 5, Name = "Palm Cruiser", Color = "Mint Green", FrameSize = "17\"", Description = "Retro style meets modern comfort.", PricePerDay = 14.49m, IsAvailable = true },
            new BeachCruiser { Id = 6, Name = "Shoreline Classic", Color = "Cream White", FrameSize = "19\"", Description = "The original beach bike. Timeless design.", PricePerDay = 17.99m, IsAvailable = false }
        );

        modelBuilder.Entity<MountainBike>().HasData(
            new MountainBike { Id = 101, ModelName = "TrailBlazer X1", Brand = "RockRider", GearCount = 21, SuspensionType = "Front", FrameMaterial = "Aluminum", Terrain = "Cross-country", DailyRate = 24.99m, WeightKg = 13.2, IsAvailable = true },
            new MountainBike { Id = 102, ModelName = "Summit Pro 29", Brand = "PeakVelo", GearCount = 27, SuspensionType = "Full", FrameMaterial = "Carbon Fiber", Terrain = "All-mountain", DailyRate = 39.99m, WeightKg = 11.8, IsAvailable = true },
            new MountainBike { Id = 103, ModelName = "DirtDevil 7.0", Brand = "MudMaster", GearCount = 18, SuspensionType = "Front", FrameMaterial = "Steel", Terrain = "Trail", DailyRate = 19.99m, WeightKg = 14.5, IsAvailable = false },
            new MountainBike { Id = 104, ModelName = "Alpine Fury", Brand = "RockRider", GearCount = 24, SuspensionType = "Full", FrameMaterial = "Aluminum", Terrain = "Enduro", DailyRate = 34.99m, WeightKg = 13.9, IsAvailable = true },
            new MountainBike { Id = 105, ModelName = "Cliff Hanger SE", Brand = "VertX", GearCount = 30, SuspensionType = "Full", FrameMaterial = "Carbon Fiber", Terrain = "Downhill", DailyRate = 37.99m, WeightKg = 15.1, IsAvailable = true },
            new MountainBike { Id = 106, ModelName = "Forest Runner", Brand = "PeakVelo", GearCount = 21, SuspensionType = "Front", FrameMaterial = "Aluminum", Terrain = "Cross-country", DailyRate = 22.99m, WeightKg = 12.7, IsAvailable = false }
        );

        modelBuilder.Entity<Accessory>().HasData(
            new Accessory { Id = 1, Name = "Water Bottle", Category = "Hydration", Description = "BPA-free 750ml bottle with bike mount.", UnitPrice = 2.99m, StockCount = 15, CompatibleWith = "all" },
            new Accessory { Id = 2, Name = "Wicker Beach Basket", Category = "Storage", Description = "Classic wicker front basket for beach essentials.", UnitPrice = 4.99m, StockCount = 8, CompatibleWith = "beach" },
            new Accessory { Id = 3, Name = "Bike Light", Category = "Safety", Description = "LED front light, 3 brightness modes, USB rechargeable.", UnitPrice = 3.49m, StockCount = 20, CompatibleWith = "all" },
            new Accessory { Id = 4, Name = "Mountain Cargo Rack", Category = "Storage", Description = "Rear-mount aluminum cargo rack for trail gear.", UnitPrice = 5.99m, StockCount = 6, CompatibleWith = "mountain" }
        );
    }
}
