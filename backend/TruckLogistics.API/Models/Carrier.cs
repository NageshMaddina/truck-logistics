namespace TruckLogistics.API.Models;

public class Carrier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MCNumber { get; set; } = string.Empty;
    public string? DOTNumber { get; set; }
    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal? Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
    public ICollection<Load> Loads { get; set; } = new List<Load>();
}
