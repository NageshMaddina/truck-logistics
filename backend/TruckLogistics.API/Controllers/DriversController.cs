using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TruckLogistics.API.Data;
using TruckLogistics.API.DTOs;
using TruckLogistics.API.Models;

namespace TruckLogistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DriversController : ControllerBase
{
    private readonly AppDbContext _db;

    public DriversController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DriverDto>>> GetDrivers(
        [FromQuery] int? carrierId,
        [FromQuery] bool? available)
    {
        var query = _db.Drivers.Include(d => d.Carrier).AsQueryable();

        if (carrierId.HasValue)
            query = query.Where(d => d.CarrierId == carrierId.Value);

        if (available.HasValue)
            query = query.Where(d => d.IsAvailable == available.Value);

        var drivers = await query
            .OrderBy(d => d.LastName)
            .Select(d => new DriverDto
            {
                Id = d.Id,
                CarrierId = d.CarrierId,
                CarrierName = d.Carrier != null ? d.Carrier.Name : null,
                FirstName = d.FirstName,
                LastName = d.LastName,
                LicenseNumber = d.LicenseNumber,
                Phone = d.Phone,
                Email = d.Email,
                IsAvailable = d.IsAvailable
            })
            .ToListAsync();

        return Ok(drivers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DriverDto>> GetDriver(int id)
    {
        var driver = await _db.Drivers
            .Include(d => d.Carrier)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (driver == null) return NotFound();

        return Ok(new DriverDto
        {
            Id = driver.Id,
            CarrierId = driver.CarrierId,
            CarrierName = driver.Carrier != null ? driver.Carrier.Name : null,
            FirstName = driver.FirstName,
            LastName = driver.LastName,
            LicenseNumber = driver.LicenseNumber,
            Phone = driver.Phone,
            Email = driver.Email,
            IsAvailable = driver.IsAvailable
        });
    }

    [HttpPost]
    public async Task<ActionResult<DriverDto>> CreateDriver(CreateDriverDto dto)
    {
        var driver = new Driver
        {
            CarrierId = dto.CarrierId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            LicenseNumber = dto.LicenseNumber,
            Phone = dto.Phone,
            Email = dto.Email
        };

        _db.Drivers.Add(driver);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDriver), new { id = driver.Id }, new DriverDto
        {
            Id = driver.Id,
            CarrierId = driver.CarrierId,
            FirstName = driver.FirstName,
            LastName = driver.LastName,
            LicenseNumber = driver.LicenseNumber,
            IsAvailable = driver.IsAvailable
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDriver(int id, CreateDriverDto dto)
    {
        var driver = await _db.Drivers.FindAsync(id);
        if (driver == null) return NotFound();

        driver.CarrierId = dto.CarrierId;
        driver.FirstName = dto.FirstName;
        driver.LastName = dto.LastName;
        driver.LicenseNumber = dto.LicenseNumber;
        driver.Phone = dto.Phone;
        driver.Email = dto.Email;
        driver.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}/availability")]
    public async Task<IActionResult> SetAvailability(int id, [FromBody] bool available)
    {
        var driver = await _db.Drivers.FindAsync(id);
        if (driver == null) return NotFound();
        driver.IsAvailable = available;
        driver.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
