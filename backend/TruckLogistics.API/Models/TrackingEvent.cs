namespace TruckLogistics.API.Models;

public class TrackingEvent
{
    public int Id { get; set; }
    public int LoadId { get; set; }
    public string EventType { get; set; } = string.Empty; // PickedUp, InTransit, Delayed, Delivered
    public string? Location { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Notes { get; set; }
    public DateTime EventTime { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Load Load { get; set; } = null!;
}
