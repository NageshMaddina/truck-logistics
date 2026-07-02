namespace TruckLogistics.API.DTOs;

public class LoadDto
{
    public int Id { get; set; }
    public string LoadNumber { get; set; } = string.Empty;
    public int? CarrierId { get; set; }
    public string? CarrierName { get; set; }
    public int? DriverId { get; set; }
    public string? DriverName { get; set; }
    public string Status { get; set; } = string.Empty;
    public string EquipmentType { get; set; } = string.Empty;
    public decimal? Weight { get; set; }
    public string? Commodity { get; set; }
    public string PickupCity { get; set; } = string.Empty;
    public string PickupState { get; set; } = string.Empty;
    public DateTime PickupDate { get; set; }
    public string DeliveryCity { get; set; } = string.Empty;
    public string DeliveryState { get; set; } = string.Empty;
    public DateTime DeliveryDate { get; set; }
    public decimal? Miles { get; set; }
    public decimal? Rate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LoadDetailDto : LoadDto
{
    public string PickupAddress { get; set; } = string.Empty;
    public string? PickupZip { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string? DeliveryZip { get; set; }
    public string? Notes { get; set; }
    public List<TrackingEventDto> TrackingEvents { get; set; } = new();
}

public class CreateLoadDto
{
    public string EquipmentType { get; set; } = string.Empty;
    public decimal? Weight { get; set; }
    public string? Commodity { get; set; }
    public string PickupAddress { get; set; } = string.Empty;
    public string PickupCity { get; set; } = string.Empty;
    public string PickupState { get; set; } = string.Empty;
    public string? PickupZip { get; set; }
    public DateTime PickupDate { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string DeliveryCity { get; set; } = string.Empty;
    public string DeliveryState { get; set; } = string.Empty;
    public string? DeliveryZip { get; set; }
    public DateTime DeliveryDate { get; set; }
    public decimal? Miles { get; set; }
    public decimal? Rate { get; set; }
    public string? Notes { get; set; }
}

public class UpdateLoadDto : CreateLoadDto
{
    public string Status { get; set; } = string.Empty;
    public int? CarrierId { get; set; }
    public int? DriverId { get; set; }
}

public class TrackingEventDto
{
    public int Id { get; set; }
    public int LoadId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Notes { get; set; }
    public DateTime EventTime { get; set; }
}

public class CreateTrackingEventDto
{
    public string EventType { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Notes { get; set; }
    public DateTime EventTime { get; set; } = DateTime.UtcNow;
}
