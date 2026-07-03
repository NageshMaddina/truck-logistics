using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TruckLogistics.API.Data;
using TruckLogistics.API.DTOs;
using TruckLogistics.API.Models;

namespace TruckLogistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarriersController : ControllerBase
{
    private readonly AppDbContext _db;

    public CarriersController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CarrierDto>>> GetCarriers([FromQuery] string? search)
    {
        var query = _db.Carriers.AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(c =>
                c.Name.Contains(search) ||
                c.MCNumber.Contains(search) ||
                (c.ContactName != null && c.ContactName.Contains(search)));

        var carriers = await query
            .OrderBy(c => c.Name)
            .Select(c => new CarrierDto
            {
                Id = c.Id,
                Name = c.Name,
                MCNumber = c.MCNumber,
                DOTNumber = c.DOTNumber,
                ContactName = c.ContactName,
                ContactEmail = c.ContactEmail,
                ContactPhone = c.ContactPhone,
                City = c.City,
                State = c.State,
                IsActive = c.IsActive,
                Rating = c.Rating,
                ActiveLoads = c.Loads.Count(l => l.Status == "InTransit" || l.Status == "Booked"),
                TotalDrivers = _db.Drivers.Count(d => d.CarrierId == c.Id)
            })
            .ToListAsync();

        return Ok(carriers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CarrierDto>> GetCarrier(int id)
    {
        var carrier = await _db.Carriers
            .Include(c => c.Loads)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (carrier == null) return NotFound();

        return Ok(new CarrierDto
        {
            Id = carrier.Id,
            Name = carrier.Name,
            MCNumber = carrier.MCNumber,
            DOTNumber = carrier.DOTNumber,
            ContactName = carrier.ContactName,
            ContactEmail = carrier.ContactEmail,
            ContactPhone = carrier.ContactPhone,
            City = carrier.City,
            State = carrier.State,
            IsActive = carrier.IsActive,
            Rating = carrier.Rating,
            ActiveLoads = carrier.Loads.Count(l => l.Status == "InTransit" || l.Status == "Booked"),
            TotalDrivers = await _db.Drivers.CountAsync(d => d.CarrierId == id)
        });
    }

    [HttpPost]
    public async Task<ActionResult<CarrierDto>> CreateCarrier(CreateCarrierDto dto)
    {
        var carrier = new Carrier
        {
            Name = dto.Name,
            MCNumber = dto.MCNumber,
            DOTNumber = dto.DOTNumber,
            ContactName = dto.ContactName,
            ContactEmail = dto.ContactEmail,
            ContactPhone = dto.ContactPhone,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            ZipCode = dto.ZipCode,
            Rating = dto.Rating
        };

        _db.Carriers.Add(carrier);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCarrier), new { id = carrier.Id }, new CarrierDto
        {
            Id = carrier.Id,
            Name = carrier.Name,
            MCNumber = carrier.MCNumber,
            IsActive = carrier.IsActive
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCarrier(int id, CreateCarrierDto dto)
    {
        var carrier = await _db.Carriers.FindAsync(id);
        if (carrier == null) return NotFound();

        carrier.Name = dto.Name;
        carrier.MCNumber = dto.MCNumber;
        carrier.DOTNumber = dto.DOTNumber;
        carrier.ContactName = dto.ContactName;
        carrier.ContactEmail = dto.ContactEmail;
        carrier.ContactPhone = dto.ContactPhone;
        carrier.Address = dto.Address;
        carrier.City = dto.City;
        carrier.State = dto.State;
        carrier.ZipCode = dto.ZipCode;
        carrier.Rating = dto.Rating;
        carrier.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCarrier(int id)
    {
        var carrier = await _db.Carriers.FindAsync(id);
        if (carrier == null) return NotFound();
        carrier.IsActive = false;
        carrier.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
