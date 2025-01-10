using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OlMag.Manufacture2.Models.Requests;
using OlMag.Manufacture2.Models.Responses;

namespace OlMag.Manufacture2.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "UserAdministrator")]
public class UserManagementController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<UserManagementController> _logger;

    public UserManagementController(ILogger<UserManagementController> logger,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet("healthcheck")]
    [AllowAnonymous]
    public async Task<IActionResult> HealthCheck()
    {
        return Ok("success");
    }

    [HttpGet("healthcheckauth")]
    public async Task<IActionResult> HealthCheckAuth()
    {
        return Ok("success");
    }

    [HttpPost("users/{userId:guid}/addRole")]
    public async Task<IActionResult> AddUserRole(string userId, RoleRequest request)
    {
        var roleName = request.RoleName;
        if (string.IsNullOrWhiteSpace(roleName)) return BadRequest("Role name is empty");
        if (!await _roleManager.RoleExistsAsync(roleName).ConfigureAwait(false)) return BadRequest($"Role {request.RoleName} not found");
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return BadRequest($"User {userId} not found");

        await _userManager.AddToRoleAsync(user, roleName).ConfigureAwait(false);
        return Ok();
    }

    [HttpDelete("users/{userId:guid}/deleteRole")]
    public async Task<IActionResult> DeleteUserRole(string userId, RoleRequest request)
    {
        var roleName = request.RoleName;
        if (string.IsNullOrWhiteSpace(roleName)) return BadRequest("Role name is empty");
        if (!await _roleManager.RoleExistsAsync(roleName).ConfigureAwait(false)) return BadRequest($"Role {request.RoleName} not found");
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return BadRequest($"User {userId} not found");

        await _userManager.RemoveFromRoleAsync(user, roleName).ConfigureAwait(false);
        return Ok();
    }

    [HttpGet("users/all")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = (await Task.WhenAll(
                _userManager.Users.ToList().Select(GetUserInfo)).ConfigureAwait(false))
            .Where(result => result != null).ToList();
        return Ok(users);
    }

    private async Task<UserWithRolesResponse> GetUserInfo(IdentityUser user)
    {
        var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        return new UserWithRolesResponse
        {
            Id = user.Id,
            Name = user.UserName,
            Email = user.Email,
            Roles = roles.ToArray()
        };
    }
}