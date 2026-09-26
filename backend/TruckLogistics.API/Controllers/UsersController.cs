using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TruckLogistics.API.Data;
using TruckLogistics.API.DTOs;

namespace TruckLogistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = IdentitySeeder.AdminRole)]
public class UsersController : ControllerBase
{
    private readonly UserManager<IdentityUser> _users;

    public UsersController(UserManager<IdentityUser> users) => _users = users;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var adminIds = (await _users.GetUsersInRoleAsync(IdentitySeeder.AdminRole)).Select(u => u.Id).ToHashSet();
        var users = await _users.Users.OrderBy(u => u.Email).ToListAsync();

        return Ok(users.Select(u => ToDto(u, adminIds.Contains(u.Id))));
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserRequest request)
    {
        var user = new IdentityUser { UserName = request.Email, Email = request.Email, EmailConfirmed = true };
        var result = await _users.CreateAsync(user, request.Password);
        if (!result.Succeeded) return this.IdentityProblem(result);

        if (request.IsAdmin) await _users.AddToRoleAsync(user, IdentitySeeder.AdminRole);

        return Created($"/api/users/{user.Id}", ToDto(user, request.IsAdmin));
    }

    [HttpPost("{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(string id, ResetPasswordRequest request)
    {
        var user = await _users.FindByIdAsync(id);
        if (user == null) return NotFound();

        // Validate the new password before removing the old one
        foreach (var validator in _users.PasswordValidators)
        {
            var check = await validator.ValidateAsync(_users, user, request.NewPassword);
            if (!check.Succeeded) return this.IdentityProblem(check);
        }

        await _users.RemovePasswordAsync(user);
        await _users.AddPasswordAsync(user, request.NewPassword);
        await _users.SetLockoutEndDateAsync(user, null);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _users.FindByIdAsync(id);
        if (user == null) return NotFound();
        if (user.Id == _users.GetUserId(User))
            return Problem("You can't delete your own account.", statusCode: StatusCodes.Status400BadRequest);

        await _users.DeleteAsync(user);
        return NoContent();
    }

    private static UserDto ToDto(IdentityUser u, bool isAdmin) => new()
    {
        Id = u.Id,
        Email = u.Email ?? u.UserName ?? string.Empty,
        IsAdmin = isAdmin,
        IsLockedOut = u.LockoutEnd > DateTimeOffset.UtcNow
    };
}
