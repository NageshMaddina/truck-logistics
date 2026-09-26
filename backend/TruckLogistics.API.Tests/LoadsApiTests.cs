using System.Net;
using System.Net.Http.Json;
using TruckLogistics.API.DTOs;

namespace TruckLogistics.API.Tests;

public class LoadsApiTests : IAsyncLifetime
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
    public async Task Stats_OnEmptyDatabase_ReturnsZeros()
    {
        var stats = await _client.GetStatsAsync();

        Assert.Equal(new StatsResponse(0, 0, 0, 0, 0, 0m), stats);
    }

    [Fact]
    public async Task Stats_SumsRevenueOfDeliveredLoadsOnly()
    {
        // Regression: SQLite can't SUM decimals server-side, so this used to return 500
        var delivered1 = await _client.CreateLoadAsync(ApiClient.NewLoad(rate: 1250.50m));
        var delivered2 = await _client.CreateLoadAsync(ApiClient.NewLoad(rate: 2000m));
        await _client.CreateLoadAsync(ApiClient.NewLoad(rate: 9999m)); // stays Available
        await _client.TrackAsync(delivered1.Id, "Delivered");
        await _client.TrackAsync(delivered2.Id, "Delivered");

        var stats = await _client.GetStatsAsync();

        Assert.Equal(3, stats.Total);
        Assert.Equal(1, stats.Available);
        Assert.Equal(2, stats.Delivered);
        Assert.Equal(3250.50m, stats.TotalRevenue);
    }

    [Fact]
    public async Task CreateLoad_AssignsSequentialNumbersForTheCurrentYear()
    {
        var first = await _client.CreateLoadAsync();
        var second = await _client.CreateLoadAsync();

        var prefix = $"TL-{DateTime.UtcNow.Year}-";
        Assert.Equal(prefix + "0001", first.LoadNumber);
        Assert.Equal(prefix + "0002", second.LoadNumber);
        Assert.Equal("Available", first.Status);
    }

    [Fact]
    public async Task CreateLoad_AfterDeletingALoad_DoesNotReuseANumber()
    {
        // Regression: numbers came from Count() + 1, so this used to collide and return 500
        var first = await _client.CreateLoadAsync();
        var second = await _client.CreateLoadAsync();
        (await _client.DeleteAsync($"/api/loads/{first.Id}")).EnsureSuccessStatusCode();

        var third = await _client.CreateLoadAsync();

        Assert.NotEqual(second.LoadNumber, third.LoadNumber);
        Assert.EndsWith("-0003", third.LoadNumber);
    }

    [Fact]
    public async Task Load_MovesThroughBookPickupAndDelivery()
    {
        var carrier = await _client.CreateCarrierAsync();
        var driver = await _client.CreateDriverAsync(carrier.Id);
        var newLoad = ApiClient.NewLoad();
        var load = await _client.CreateLoadAsync(newLoad);

        await _client.BookAsync(load.Id, newLoad, carrier.Id, driver.Id);
        var booked = await _client.GetFromJsonAsync<LoadDetailDto>($"/api/loads/{load.Id}");
        var bookedDriver = await _client.GetFromJsonAsync<DriverDto>($"/api/drivers/{driver.Id}");
        Assert.Equal("Booked", booked!.Status);
        Assert.Equal(carrier.Id, booked.CarrierId);
        Assert.False(bookedDriver!.IsAvailable);

        await _client.TrackAsync(load.Id, "PickedUp");
        var inTransit = await _client.GetFromJsonAsync<LoadDetailDto>($"/api/loads/{load.Id}");
        Assert.Equal("InTransit", inTransit!.Status);

        await _client.TrackAsync(load.Id, "Delivered");
        var delivered = await _client.GetFromJsonAsync<LoadDetailDto>($"/api/loads/{load.Id}");
        Assert.Equal("Delivered", delivered!.Status);
        Assert.Equal(new[] { "Delivered", "PickedUp" }, delivered.TrackingEvents.Select(e => e.EventType).Order());
    }

    [Fact]
    public async Task GetLoads_FiltersByStatusAndSearch()
    {
        var columbus = await _client.CreateLoadAsync(ApiClient.NewLoad(pickupCity: "Columbus", commodity: "Paper"));
        var dallas = await _client.CreateLoadAsync(ApiClient.NewLoad(pickupCity: "Dallas", commodity: "Steel"));
        await _client.TrackAsync(dallas.Id, "Delivered");

        var delivered = await _client.GetFromJsonAsync<List<LoadDto>>("/api/loads?status=Delivered");
        var steel = await _client.GetFromJsonAsync<List<LoadDto>>("/api/loads?search=Steel");
        var columbusOnly = await _client.GetFromJsonAsync<List<LoadDto>>("/api/loads?search=Columbus");

        Assert.Equal(dallas.Id, Assert.Single(delivered!).Id);
        Assert.Equal(dallas.Id, Assert.Single(steel!).Id);
        Assert.Equal(columbus.Id, Assert.Single(columbusOnly!).Id);
    }

    [Fact]
    public async Task GetLoad_UnknownId_Returns404()
    {
        var response = await _client.GetAsync("/api/loads/12345");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
