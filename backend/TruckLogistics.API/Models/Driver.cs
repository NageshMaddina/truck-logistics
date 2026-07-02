namespace TruckLogistics.API.Models;

public class Driver
{
    public int Id { get; set; }
    public int CarrierId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Carrier Carrier { get; set; } = null!;
    public ICollection<Load> Loads { get; set; } = new List<Load>();

    public string FullName => $"{FirstName} {LastName}";
}
