using BikeRental.API.Data;
using BikeRental.API.Middleware;
using BikeRental.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<BikeRentalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services (Dependency Injection — replaces the old static ApplicationServices class)
builder.Services.AddScoped<IBikeService, BikeService>();
builder.Services.AddScoped<IAccessoryService, AccessoryService>();

// Controllers + JSON options
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

// Swagger/OpenAPI
builder.Services.AddOpenApi();

// CORS for React frontend (dev on different port)
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactDev", policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("ReactDev");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
