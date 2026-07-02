namespace TruckLogistics.API.DTOs;

public class CarrierDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MCNumber { get; set; } = string.Empty;
    public string? DOTNumber { get; set; }
    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public bool IsActive { get; set; }
    public decimal? Rating { get; set; }
    public int ActiveLoads { get; set; }
    public int TotalDrivers { get; set; }
}

public class CreateCarrierDto
{
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
    public decimal? Rating { get; set; }
}

public class DriverDto
{
    public int Id { get; set; }
    public int CarrierId { get; set; }
    public string? CarrierName { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string LicenseNumber { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsAvailable { get; set; }
}

public class CreateDriverDto
{
    public int CarrierId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
}
