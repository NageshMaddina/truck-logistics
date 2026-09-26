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
    // An in-memory SQLite database lives only as long as its connection is open
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public ApiFactory() => _connection.Open();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Development mode makes Program.cs call EnsureCreated on startup
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _connection.Dispose();
    }
}
