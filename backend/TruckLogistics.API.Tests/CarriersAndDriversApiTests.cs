using System.Net.Http.Json;
using TruckLogistics.API.DTOs;

namespace TruckLogistics.API.Tests;

public class CarriersAndDriversApiTests : IAsyncLifetime
{
    private readonly ApiFactory _factory = new();
    private HttpClient _client = null!;

    public async Task InitializeAsync() => _client = await _factory.CreateSignedInClientAsync();

    public Task DisposeAsync()
    {
        _factory.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetCarrier_CountsItsDrivers()
    {
        var carrier = await _client.CreateCarrierAsync();
        await _client.CreateDriverAsync(carrier.Id);
        await _client.CreateDriverAsync(carrier.Id);

        var fetched = await _client.GetFromJsonAsync<CarrierDto>($"/api/carriers/{carrier.Id}");

        Assert.Equal(2, fetched!.TotalDrivers);
        Assert.True(fetched.IsActive);
    }

    [Fact]
    public async Task GetCarriers_SearchMatchesName()
    {
        await _client.CreateCarrierAsync("Swift Haul Transport");
        await _client.CreateCarrierAsync("Lone Star Freight");

        var results = await _client.GetFromJsonAsync<List<CarrierDto>>("/api/carriers?search=Lone");

        Assert.Equal("Lone Star Freight", Assert.Single(results!).Name);
    }

    [Fact]
    public async Task DeleteCarrier_DeactivatesInsteadOfRemoving()
    {
        var carrier = await _client.CreateCarrierAsync();

        (await _client.DeleteAsync($"/api/carriers/{carrier.Id}")).EnsureSuccessStatusCode();
        var fetched = await _client.GetFromJsonAsync<CarrierDto>($"/api/carriers/{carrier.Id}");

        Assert.False(fetched!.IsActive);
    }

    [Fact]
    public async Task SetAvailability_UpdatesDriverAndAvailableFilter()
    {
        var carrier = await _client.CreateCarrierAsync();
        var busy = await _client.CreateDriverAsync(carrier.Id);
        var free = await _client.CreateDriverAsync(carrier.Id);

        (await _client.PatchAsJsonAsync($"/api/drivers/{busy.Id}/availability", false)).EnsureSuccessStatusCode();
        var available = await _client.GetFromJsonAsync<List<DriverDto>>("/api/drivers?available=true");

        Assert.Equal(free.Id, Assert.Single(available!).Id);
    }
}
