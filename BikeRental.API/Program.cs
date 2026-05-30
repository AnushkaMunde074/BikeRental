using BikeRental.API.Data;
using BikeRental.API.Middleware;
using BikeRental.API.Services;
using BikeRental.API.Settings;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<BikeRentalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Strongly-typed configuration
builder.Services.Configure<FleetSettings>(builder.Configuration.GetSection("FleetSettings"));

// Services (Dependency Injection)
builder.Services.AddScoped<IBikeService, BikeService>();
builder.Services.AddScoped<IAccessoryService, AccessoryService>();

// Controllers + JSON options
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

// Swagger/OpenAPI
builder.Services.AddOpenApi();

// Health checks
builder.Services.AddHealthChecks();

// CORS — origins are environment-specific via appsettings
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
    {
        var origins = allowedOrigins.Where(o => !string.IsNullOrWhiteSpace(o)).ToArray();
        if (origins.Length == 0)
        {
            origins = ["http://localhost:5173", "http://localhost:3000"];
        }

        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Apply migrations and seed on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BikeRentalDbContext>();
    db.Database.EnsureCreated();
}

// Middleware pipeline
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors("ReactApp");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
