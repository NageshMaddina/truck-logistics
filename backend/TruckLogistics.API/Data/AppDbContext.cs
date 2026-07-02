using Microsoft.EntityFrameworkCore;
using TruckLogistics.API.Models;

namespace TruckLogistics.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Carrier> Carriers => Set<Carrier>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Load> Loads => Set<Load>();
    public DbSet<TrackingEvent> TrackingEvents => Set<TrackingEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Carrier
        modelBuilder.Entity<Carrier>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).IsRequired().HasMaxLength(200);
            e.Property(c => c.MCNumber).IsRequired().HasMaxLength(50);
            e.HasIndex(c => c.MCNumber).IsUnique();
            e.Property(c => c.Rating).HasPrecision(3, 2);
            e.Ignore(c => c.Drivers);
        });

        // Driver
        modelBuilder.Entity<Driver>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.FirstName).IsRequired().HasMaxLength(100);
            e.Property(d => d.LastName).IsRequired().HasMaxLength(100);
            e.Property(d => d.LicenseNumber).IsRequired().HasMaxLength(100);
            e.Ignore(d => d.FullName);
            e.HasOne(d => d.Carrier)
             .WithMany()
             .HasForeignKey(d => d.CarrierId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Load
        modelBuilder.Entity<Load>(e =>
        {
            e.HasKey(l => l.Id);
            e.Property(l => l.LoadNumber).IsRequired().HasMaxLength(50);
            e.HasIndex(l => l.LoadNumber).IsUnique();
            e.Property(l => l.Status).IsRequired().HasMaxLength(50);
            e.Property(l => l.EquipmentType).IsRequired().HasMaxLength(100);
            e.Property(l => l.Rate).HasPrecision(10, 2);
            e.Property(l => l.Miles).HasPrecision(10, 2);
            e.Property(l => l.Weight).HasPrecision(10, 2);
            e.HasOne(l => l.Carrier)
             .WithMany(c => c.Loads)
             .HasForeignKey(l => l.CarrierId)
             .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(l => l.Driver)
             .WithMany(d => d.Loads)
             .HasForeignKey(l => l.DriverId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // TrackingEvent
        modelBuilder.Entity<TrackingEvent>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.EventType).IsRequired().HasMaxLength(100);
            e.Property(t => t.Latitude).HasPrecision(10, 6);
            e.Property(t => t.Longitude).HasPrecision(10, 6);
            e.HasOne(t => t.Load)
             .WithMany(l => l.TrackingEvents)
             .HasForeignKey(t => t.LoadId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
