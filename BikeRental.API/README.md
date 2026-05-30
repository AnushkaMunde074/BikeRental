# BikeRental.API — Modern Backend

A .NET 9 Web API that replaces the legacy .NET Framework 4.8 ASP.NET Web Application backend.

## What Changed from Legacy

| Aspect | Legacy (.NET 4.8) | Modern (.NET 9) |
|--------|-------------------|-----------------|
| **Framework** | ASP.NET Web Forms + ASHX handlers | ASP.NET Core Minimal Hosting + Controllers |
| **Data Access** | Raw file I/O (XML/JSON) + BinaryFormatter cache | Entity Framework Core 9 + SQL Server |
| **DI Container** | Static `ApplicationServices` class | Built-in DI (`IServiceCollection`) |
| **Startup** | `Global.asax` + `Application_Start` | `Program.cs` (top-level statements) |
| **Background Work** | `FleetMonitor` with raw `Thread` polling | Removed (EF handles persistence) |
| **Caching** | `BinaryFormatterCache` (deprecated/insecure) | EF Core query caching + SQL Server |
| **Error Handling** | Unhandled / custom ASHX try-catch | Global exception middleware |
| **API Format** | Custom ASHX handlers writing JSON manually | REST controllers with model binding |
| **COM Dependency** | `ShellIntegration` (Shell.Application COM) | Removed entirely |
| **Data Storage** | Flat files (XML, JSON) on disk | SQL Server LocalDB with seed data |
| **Security** | BinaryFormatter deserialization risk | Parameterized EF queries, no deserialization |

## Tech Stack

- **.NET 9** (SDK 9.0.x)
- **ASP.NET Core** Web API with Controllers
- **Entity Framework Core 9** (Code-First, auto-migration via `EnsureCreated`)
- **SQL Server LocalDB** (development database)
- **Dependency Injection** (scoped services)
- **OpenAPI/Swagger** (development endpoint discovery)
- **CORS** configured for React dev server

## Project Structure

```
BikeRental.API/
├── Program.cs                  # App entry, DI registration, middleware pipeline
├── appsettings.json            # Connection string, logging config
├── Controllers/
│   ├── BikesController.cs      # GET /api/bikes/beach, GET /api/bikes/mountain, POST /api/bikes/rent, POST /api/bikes/reset
│   └── AccessoriesController.cs # GET /api/accessories, POST /api/accessories/order
├── Services/
│   ├── BikeService.cs          # IBikeService — rental logic, fleet reset
│   └── AccessoryService.cs     # IAccessoryService — orders, bundle discount logic
├── Data/
│   └── BikeRentalDbContext.cs  # EF Core context, model config, seed data
├── Models/
│   ├── BeachCruiser.cs
│   ├── MountainBike.cs
│   ├── Accessory.cs
│   ├── Rental.cs
│   └── Order.cs
├── DTOs/
│   └── Dtos.cs                 # Request/response DTOs (no entity exposure)
└── Middleware/
    └── GlobalExceptionMiddleware.cs  # Catches unhandled exceptions, returns JSON
```

## API Endpoints

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/bikes/beach` | List all beach cruisers |
| GET | `/api/bikes/mountain` | List all mountain bikes |
| POST | `/api/bikes/rent` | Rent a bike `{ bikeType, bikeId }` |
| POST | `/api/bikes/reset` | Reset fleet availability |
| GET | `/api/accessories?bikeType=` | List accessories (optional filter) |
| POST | `/api/accessories/order` | Place accessory order `[{ accessoryId, quantity }]` |

## Key Enhancements

1. **Proper Data Layer** — EF Core replaces raw file reads; data survives app restarts without file locking issues
2. **Dependency Injection** — Services are scoped per-request; testable via interfaces (`IBikeService`, `IAccessoryService`)
3. **Global Error Handling** — Middleware catches all exceptions, logs them, returns consistent JSON error responses
4. **Bundle Discount Logic** — Accessory orders with Water Bottle (ID 1) + Bike Light (ID 3) get 10% off
5. **Seed Data** — Database auto-creates and seeds 6 beach cruisers, 6 mountain bikes, and 4 accessories on first run
6. **No Legacy Baggage** — Removed `BinaryFormatterCache` (security risk), `FleetMonitor` (raw thread), `ShellIntegration` (COM dependency)

## Running

```bash
cd BikeRental.API
dotnet run
```

API starts on `http://localhost:5035`. Requires SQL Server LocalDB installed.

## Configuration

Connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BikeRentalDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```
