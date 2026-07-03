using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TruckLogistics.API.Data;
using TruckLogistics.API.DTOs;
using TruckLogistics.API.Models;

namespace TruckLogistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoadsController : ControllerBase
{
    private readonly AppDbContext _db;

    public LoadsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoadDto>>> GetLoads(
        [FromQuery] string? status,
        [FromQuery] string? search)
    {
        var query = _db.Loads
            .Include(l => l.Carrier)
            .Include(l => l.Driver)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(l => l.Status == status);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(l =>
                l.LoadNumber.Contains(search) ||
                l.PickupCity.Contains(search) ||
                l.DeliveryCity.Contains(search) ||
                (l.Commodity != null && l.Commodity.Contains(search)));

        var loads = await query
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new LoadDto
            {
                Id = l.Id,
                LoadNumber = l.LoadNumber,
                CarrierId = l.CarrierId,
                CarrierName = l.Carrier != null ? l.Carrier.Name : null,
                DriverId = l.DriverId,
                DriverName = l.Driver != null ? l.Driver.FirstName + " " + l.Driver.LastName : null,
                Status = l.Status,
                EquipmentType = l.EquipmentType,
                Weight = l.Weight,
                Commodity = l.Commodity,
                PickupCity = l.PickupCity,
                PickupState = l.PickupState,
                PickupDate = l.PickupDate,
                DeliveryCity = l.DeliveryCity,
                DeliveryState = l.DeliveryState,
                DeliveryDate = l.DeliveryDate,
                Miles = l.Miles,
                Rate = l.Rate,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync();

        return Ok(loads);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LoadDetailDto>> GetLoad(int id)
    {
        var load = await _db.Loads
            .Include(l => l.Carrier)
            .Include(l => l.Driver)
            .Include(l => l.TrackingEvents)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (load == null) return NotFound();

        return Ok(new LoadDetailDto
        {
            Id = load.Id,
            LoadNumber = load.LoadNumber,
            CarrierId = load.CarrierId,
            CarrierName = load.Carrier != null ? load.Carrier.Name : null,
            DriverId = load.DriverId,
            DriverName = load.Driver != null ? load.Driver.FirstName + " " + load.Driver.LastName : null,
            Status = load.Status,
            EquipmentType = load.EquipmentType,
            Weight = load.Weight,
            Commodity = load.Commodity,
            PickupAddress = load.PickupAddress,
            PickupCity = load.PickupCity,
            PickupState = load.PickupState,
            PickupZip = load.PickupZip,
            PickupDate = load.PickupDate,
            DeliveryAddress = load.DeliveryAddress,
            DeliveryCity = load.DeliveryCity,
            DeliveryState = load.DeliveryState,
            DeliveryZip = load.DeliveryZip,
            DeliveryDate = load.DeliveryDate,
            Miles = load.Miles,
            Rate = load.Rate,
            Notes = load.Notes,
            CreatedAt = load.CreatedAt,
            TrackingEvents = load.TrackingEvents
                .OrderByDescending(t => t.EventTime)
                .Select(t => new TrackingEventDto
                {
                    Id = t.Id,
                    LoadId = t.LoadId,
                    EventType = t.EventType,
                    Location = t.Location,
                    City = t.City,
                    State = t.State,
                    Notes = t.Notes,
                    EventTime = t.EventTime
                }).ToList()
        });
    }

    [HttpPost]
    public async Task<ActionResult<LoadDto>> CreateLoad(CreateLoadDto dto)
    {
        var loadNumber = "TL-" + DateTime.UtcNow.Year + "-" + (_db.Loads.Count() + 1).ToString("D4");
        var load = new Load
        {
            LoadNumber = loadNumber,
            EquipmentType = dto.EquipmentType,
            Weight = dto.Weight,
            Commodity = dto.Commodity,
            PickupAddress = dto.PickupAddress,
            PickupCity = dto.PickupCity,
            PickupState = dto.PickupState,
            PickupZip = dto.PickupZip,
            PickupDate = dto.PickupDate,
            DeliveryAddress = dto.DeliveryAddress,
            DeliveryCity = dto.DeliveryCity,
            DeliveryState = dto.DeliveryState,
            DeliveryZip = dto.DeliveryZip,
            DeliveryDate = dto.DeliveryDate,
            Miles = dto.Miles,
            Rate = dto.Rate,
            Notes = dto.Notes,
            Status = "Available"
        };

        _db.Loads.Add(load);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetLoad), new { id = load.Id }, new LoadDto
        {
            Id = load.Id,
            LoadNumber = load.LoadNumber,
            Status = load.Status,
            EquipmentType = load.EquipmentType,
            PickupCity = load.PickupCity,
            PickupState = load.PickupState,
            PickupDate = load.PickupDate,
            DeliveryCity = load.DeliveryCity,
            DeliveryState = load.DeliveryState,
            DeliveryDate = load.DeliveryDate
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLoad(int id, UpdateLoadDto dto)
    {
        var load = await _db.Loads.FindAsync(id);
        if (load == null) return NotFound();

        load.Status = dto.Status;
        load.CarrierId = dto.CarrierId;
        load.DriverId = dto.DriverId;
        load.EquipmentType = dto.EquipmentType;
        load.Weight = dto.Weight;
        load.Commodity = dto.Commodity;
        load.PickupAddress = dto.PickupAddress;
        load.PickupCity = dto.PickupCity;
        load.PickupState = dto.PickupState;
        load.PickupZip = dto.PickupZip;
        load.PickupDate = dto.PickupDate;
        load.DeliveryAddress = dto.DeliveryAddress;
        load.DeliveryCity = dto.DeliveryCity;
        load.DeliveryState = dto.DeliveryState;
        load.DeliveryZip = dto.DeliveryZip;
        load.DeliveryDate = dto.DeliveryDate;
        load.Miles = dto.Miles;
        load.Rate = dto.Rate;
        load.Notes = dto.Notes;
        load.UpdatedAt = DateTime.UtcNow;

        if (dto.DriverId.HasValue && load.Status == "Booked")
        {
            var driver = await _db.Drivers.FindAsync(dto.DriverId.Value);
            if (driver != null) driver.IsAvailable = false;
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLoad(int id)
    {
        var load = await _db.Loads.FindAsync(id);
        if (load == null) return NotFound();
        _db.Loads.Remove(load);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/tracking")]
    public async Task<ActionResult<TrackingEventDto>> AddTracking(int id, CreateTrackingEventDto dto)
    {
        var load = await _db.Loads.FindAsync(id);
        if (load == null) return NotFound();

        var evt = new TrackingEvent
        {
            LoadId = id,
            EventType = dto.EventType,
            Location = dto.Location,
            City = dto.City,
            State = dto.State,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Notes = dto.Notes,
            EventTime = dto.EventTime
        };

        if (dto.EventType == "PickedUp") load.Status = "InTransit";
        else if (dto.EventType == "Delivered") load.Status = "Delivered";
        load.UpdatedAt = DateTime.UtcNow;

        _db.TrackingEvents.Add(evt);
        await _db.SaveChangesAsync();

        return Ok(new TrackingEventDto
        {
            Id = evt.Id,
            LoadId = evt.LoadId,
            EventType = evt.EventType,
            Location = evt.Location,
            City = evt.City,
            State = evt.State,
            Notes = evt.Notes,
            EventTime = evt.EventTime
        });
    }

    [HttpGet("stats")]
    public async Task<ActionResult<object>> GetStats()
    {
        var stats = new
        {
            Total = await _db.Loads.CountAsync(),
            Available = await _db.Loads.CountAsync(l => l.Status == "Available"),
            Booked = await _db.Loads.CountAsync(l => l.Status == "Booked"),
            InTransit = await _db.Loads.CountAsync(l => l.Status == "InTransit"),
            Delivered = await _db.Loads.CountAsync(l => l.Status == "Delivered"),
            TotalRevenue = await _db.Loads
                .Where(l => l.Status == "Delivered")
                .SumAsync(l => l.Rate ?? 0)
        };
        return Ok(stats);
    }
}
