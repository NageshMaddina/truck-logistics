using Microsoft.AspNetCore.Identity;

namespace TruckLogistics.API.Data;

public static class IdentitySeeder
{
    public const string AdminRole = "Admin";

    /// <summary>
    /// Ensures the Admin role exists and, when the database has no users yet,
    /// creates the first admin from the AdminAccount:Email / AdminAccount:Password settings.
    /// </summary>
    public static async Task EnsureAdminAsync(IServiceProvider services, IConfiguration config, ILogger logger)
    {
        var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
        var users = services.GetRequiredService<UserManager<IdentityUser>>();

        if (!await roles.RoleExistsAsync(AdminRole))
            await roles.CreateAsync(new IdentityRole(AdminRole));

        if (users.Users.Any()) return;

        var email = config["AdminAccount:Email"];
        var password = config["AdminAccount:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("No user accounts exist and AdminAccount:Email / AdminAccount:Password are not set, so nobody can sign in. " +
                              "Set them (e.g. environment variables AdminAccount__Email and AdminAccount__Password) and restart.");
            return;
        }

        var admin = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
        var result = await users.CreateAsync(admin, password);
        if (!result.Succeeded)
            throw new InvalidOperationException("Could not create the initial admin account: " +
                                                string.Join(" ", result.Errors.Select(e => e.Description)));

        await users.AddToRoleAsync(admin, AdminRole);
        logger.LogInformation("Created initial admin account {Email}", email);
    }
}
