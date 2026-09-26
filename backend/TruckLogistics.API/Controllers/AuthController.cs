using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TruckLogistics.API.Data;
using TruckLogistics.API.DTOs;

namespace TruckLogistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signIn;
    private readonly UserManager<IdentityUser> _users;

    public AuthController(SignInManager<IdentityUser> signIn, UserManager<IdentityUser> users)
    {
        _signIn = signIn;
        _users = users;
    }

    /// <summary>Returns { tokenType, accessToken, expiresIn, refreshToken } on success.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        // With the bearer scheme, a successful sign-in writes the token response itself
        _signIn.AuthenticationScheme = IdentityConstants.BearerScheme;
        var result = await _signIn.PasswordSignInAsync(request.Email, request.Password, isPersistent: false, lockoutOnFailure: true);

        if (result.Succeeded) return new EmptyResult();

        var detail = result.IsLockedOut
            ? "Too many failed attempts. The account is locked for a few minutes."
            : "Invalid email or password.";
        return Problem(detail, statusCode: StatusCodes.Status401Unauthorized);
    }

    [HttpGet("me")]
    public async Task<ActionResult<CurrentUserDto>> Me()
    {
        var user = await _users.GetUserAsync(User);
        if (user == null) return Unauthorized();

        return new CurrentUserDto
        {
            Email = user.Email ?? user.UserName ?? string.Empty,
            IsAdmin = await _users.IsInRoleAsync(user, IdentitySeeder.AdminRole)
        };
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var user = await _users.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var result = await _users.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        return result.Succeeded ? NoContent() : this.IdentityProblem(result);
    }
}

internal static class IdentityResultExtensions
{
    /// <summary>Turns Identity errors (e.g. password rules) into a 400 with a readable message.</summary>
    public static ObjectResult IdentityProblem(this ControllerBase controller, IdentityResult result) =>
        controller.Problem(string.Join(" ", result.Errors.Select(e => e.Description)), statusCode: StatusCodes.Status400BadRequest);
}
