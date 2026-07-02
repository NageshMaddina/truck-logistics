namespace TruckLogistics.API.Models;

public class Load
{
    public int Id { get; set; }
    public string LoadNumber { get; set; } = string.Empty;
    public int? CarrierId { get; set; }
    public int? DriverId { get; set; }
    public string Status { get; set; } = "Available"; // Available, Booked, InTransit, Delivered, Cancelled
    public string EquipmentType { get; set; } = string.Empty;
    public decimal? Weight { get; set; }
    public string? Commodity { get; set; }

    // Pickup
    public string PickupAddress { get; set; } = string.Empty;
    public string PickupCity { get; set; } = string.Empty;
    public string PickupState { get; set; } = string.Empty;
    public string? PickupZip { get; set; }
    public DateTime PickupDate { get; set; }

    // Delivery
    public string DeliveryAddress { get; set; } = string.Empty;
    public string DeliveryCity { get; set; } = string.Empty;
    public string DeliveryState { get; set; } = string.Empty;
    public string? DeliveryZip { get; set; }
    public DateTime DeliveryDate { get; set; }

    public decimal? Miles { get; set; }
    public decimal? Rate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Carrier? Carrier { get; set; }
    public Driver? Driver { get; set; }
    public ICollection<TrackingEvent> TrackingEvents { get; set; } = new List<TrackingEvent>();
}
