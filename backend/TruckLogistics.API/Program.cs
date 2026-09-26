using Microsoft.EntityFrameworkCore;
using TruckLogistics.API.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Truck Logistics API", Version = "v1" });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Create or upgrade the database schema on startup, in every environment
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Databases created by the old EnsureCreated call have the tables but no
    // migration history, so Migrate() would fail trying to recreate them.
    var hasTables = db.Database
        .SqlQueryRaw<int>("SELECT COUNT(*) AS Value FROM sqlite_master WHERE type = 'table' AND name = 'Carriers'")
        .AsEnumerable().Single() > 0;
    if (hasTables && !db.Database.GetAppliedMigrations().Any())
        throw new InvalidOperationException(
            "This database was created before migrations were introduced. Delete trucklogistics.db " +
            "(and its -shm/-wal files), restart the API, then reseed with scripts/seed-sample-data.ps1.");

    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReact");
app.UseAuthorization();
app.MapControllers();

app.Run();

// Exposes Program to WebApplicationFactory in the integration tests
public partial class Program { }
