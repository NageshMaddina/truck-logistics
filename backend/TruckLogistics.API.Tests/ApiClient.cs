using System.Net.Http.Json;
using TruckLogistics.API.DTOs;

namespace TruckLogistics.API.Tests;

public record StatsResponse(int Total, int Available, int Booked, int InTransit, int Delivered, decimal TotalRevenue);

/// <summary>Small helpers for building test data through the public API.</summary>
public static class ApiClient
{
    private static int _mcCounter;

    public static async Task<T> ReadAsync<T>(this HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    public static async Task<CarrierDto> CreateCarrierAsync(this HttpClient client, string name = "Test Carrier")
    {
        var mc = $"MC-{Interlocked.Increment(ref _mcCounter):D6}";
        var response = await client.PostAsJsonAsync("/api/carriers", new CreateCarrierDto { Name = name, MCNumber = mc });
        return await response.ReadAsync<CarrierDto>();
    }

    public static async Task<DriverDto> CreateDriverAsync(this HttpClient client, int carrierId)
    {
        var response = await client.PostAsJsonAsync("/api/drivers", new CreateDriverDto
        {
            CarrierId = carrierId, FirstName = "Test", LastName = "Driver", LicenseNumber = "CDL-TEST"
        });
        return await response.ReadAsync<DriverDto>();
    }

    public static CreateLoadDto NewLoad(decimal rate = 1000m, string pickupCity = "Columbus", string commodity = "Paper") => new()
    {
        EquipmentType = "Dry Van",
        Commodity = commodity,
        PickupAddress = "1 Pickup St", PickupCity = pickupCity, PickupState = "OH",
        PickupDate = new DateTime(2026, 10, 1, 8, 0, 0),
        DeliveryAddress = "2 Delivery Ave", DeliveryCity = "Chicago", DeliveryState = "IL",
        DeliveryDate = new DateTime(2026, 10, 2, 14, 0, 0),
        Miles = 355, Rate = rate
    };

    public static async Task<LoadDto> CreateLoadAsync(this HttpClient client, CreateLoadDto? load = null)
    {
        var response = await client.PostAsJsonAsync("/api/loads", load ?? NewLoad());
        return await response.ReadAsync<LoadDto>();
    }

    public static async Task BookAsync(this HttpClient client, int loadId, CreateLoadDto load, int carrierId, int driverId)
    {
        var update = new UpdateLoadDto
        {
            Status = "Booked", CarrierId = carrierId, DriverId = driverId,
            EquipmentType = load.EquipmentType, Commodity = load.Commodity,
            PickupAddress = load.PickupAddress, PickupCity = load.PickupCity, PickupState = load.PickupState, PickupDate = load.PickupDate,
            DeliveryAddress = load.DeliveryAddress, DeliveryCity = load.DeliveryCity, DeliveryState = load.DeliveryState, DeliveryDate = load.DeliveryDate,
            Miles = load.Miles, Rate = load.Rate
        };
        (await client.PutAsJsonAsync($"/api/loads/{loadId}", update)).EnsureSuccessStatusCode();
    }

    public static async Task TrackAsync(this HttpClient client, int loadId, string eventType)
    {
        var response = await client.PostAsJsonAsync($"/api/loads/{loadId}/tracking",
            new CreateTrackingEventDto { EventType = eventType, City = "Chicago", State = "IL" });
        response.EnsureSuccessStatusCode();
    }

    public static async Task<StatsResponse> GetStatsAsync(this HttpClient client) =>
        (await client.GetFromJsonAsync<StatsResponse>("/api/loads/stats"))!;
}
