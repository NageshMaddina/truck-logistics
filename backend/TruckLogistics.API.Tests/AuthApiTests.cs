using System.Net;
using System.Net.Http.Json;
using TruckLogistics.API.DTOs;

namespace TruckLogistics.API.Tests;

public class AuthApiTests : IDisposable
{
    private const string UserPassword = "Test-User-Pass1!";
    private readonly ApiFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Theory]
    [InlineData("/api/loads")]
    [InlineData("/api/loads/stats")]
    [InlineData("/api/carriers")]
    [InlineData("/api/drivers")]
    [InlineData("/api/users")]
    [InlineData("/api/auth/me")]
    public async Task Endpoints_WithoutSigningIn_Return401(string url)
    {
        var response = await _factory.CreateClient().GetAsync(url);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        var response = await _factory.CreateClient().PostAsJsonAsync("/api/auth/login",
            new LoginRequest { Email = ApiFactory.AdminEmail, Password = "wrong-password" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task InitialAdmin_IsCreatedFromSettings()
    {
        var admin = await _factory.CreateSignedInClientAsync();

        var me = await admin.GetFromJsonAsync<CurrentUserDto>("/api/auth/me");

        Assert.Equal(ApiFactory.AdminEmail, me!.Email);
        Assert.True(me.IsAdmin);
    }

    [Fact]
    public async Task AdminCreatedUser_CanUseTheApp_ButNotManageUsers()
    {
        var admin = await _factory.CreateSignedInClientAsync();
        (await admin.PostAsJsonAsync("/api/users",
            new CreateUserRequest { Email = "dispatcher@example.com", Password = UserPassword })).EnsureSuccessStatusCode();

        var user = await _factory.CreateSignedInClientAsync("dispatcher@example.com", UserPassword);

        Assert.Equal(HttpStatusCode.OK, (await user.GetAsync("/api/loads")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await user.GetAsync("/api/users")).StatusCode);
        Assert.False((await user.GetFromJsonAsync<CurrentUserDto>("/api/auth/me"))!.IsAdmin);
    }

    [Fact]
    public async Task CreateUser_WithWeakPassword_Returns400WithReason()
    {
        var admin = await _factory.CreateSignedInClientAsync();

        var response = await admin.PostAsJsonAsync("/api/users",
            new CreateUserRequest { Email = "weak@example.com", Password = "short" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Passwords must", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task ChangePassword_ThenSignInWithNewPassword()
    {
        var admin = await _factory.CreateSignedInClientAsync();
        await admin.PostAsJsonAsync("/api/users", new CreateUserRequest { Email = "driver-desk@example.com", Password = UserPassword });
        var user = await _factory.CreateSignedInClientAsync("driver-desk@example.com", UserPassword);

        (await user.PostAsJsonAsync("/api/auth/change-password",
            new ChangePasswordRequest { CurrentPassword = UserPassword, NewPassword = "Changed-Pass2!" })).EnsureSuccessStatusCode();

        await _factory.CreateSignedInClientAsync("driver-desk@example.com", "Changed-Pass2!");
    }

    [Fact]
    public async Task ResetPassword_ByAdmin_LetsUserSignInWithNewPassword()
    {
        var admin = await _factory.CreateSignedInClientAsync();
        var created = await (await admin.PostAsJsonAsync("/api/users",
            new CreateUserRequest { Email = "forgetful@example.com", Password = UserPassword })).ReadAsync<UserDto>();

        (await admin.PostAsJsonAsync($"/api/users/{created.Id}/reset-password",
            new ResetPasswordRequest { NewPassword = "Reset-Pass3!" })).EnsureSuccessStatusCode();

        await _factory.CreateSignedInClientAsync("forgetful@example.com", "Reset-Pass3!");
    }

    [Fact]
    public async Task DeleteUser_RemovesAccess_ButAdminCannotDeleteSelf()
    {
        var admin = await _factory.CreateSignedInClientAsync();
        var created = await (await admin.PostAsJsonAsync("/api/users",
            new CreateUserRequest { Email = "leaver@example.com", Password = UserPassword })).ReadAsync<UserDto>();
        var users = await admin.GetFromJsonAsync<List<UserDto>>("/api/users");
        var adminId = users!.Single(u => u.Email == ApiFactory.AdminEmail).Id;

        (await admin.DeleteAsync($"/api/users/{created.Id}")).EnsureSuccessStatusCode();
        var selfDelete = await admin.DeleteAsync($"/api/users/{adminId}");

        Assert.Equal(HttpStatusCode.BadRequest, selfDelete.StatusCode);
        var login = await _factory.CreateClient().PostAsJsonAsync("/api/auth/login",
            new LoginRequest { Email = "leaver@example.com", Password = UserPassword });
        Assert.Equal(HttpStatusCode.Unauthorized, login.StatusCode);
    }
}
