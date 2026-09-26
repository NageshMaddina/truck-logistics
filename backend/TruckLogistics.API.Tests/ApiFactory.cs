using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TruckLogistics.API.Data;

namespace TruckLogistics.API.Tests;

/// <summary>
/// Hosts the real API against a private in-memory SQLite database, so each
/// factory instance starts empty and never touches trucklogistics.db.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>
{
    public const string AdminEmail = "test-admin@example.com";
    public const string AdminPassword = "Test-Admin-Pass1!";

    // An in-memory SQLite database lives only as long as its connection is open
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public ApiFactory() => _connection.Open();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Development mode makes Program.cs call EnsureCreated on startup
        builder.UseEnvironment("Development");
        builder.UseSetting("AdminAccount:Email", AdminEmail);
        builder.UseSetting("AdminAccount:Password", AdminPassword);

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));
        });
    }

    /// <summary>Signs in and returns a client that sends the bearer token.</summary>
    public async Task<HttpClient> CreateSignedInClientAsync(string email = AdminEmail, string password = AdminPassword)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();
        var token = (await response.Content.ReadFromJsonAsync<TokenResponse>())!;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        return client;
    }

    private record TokenResponse(string AccessToken);

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _connection.Dispose();
    }
}
