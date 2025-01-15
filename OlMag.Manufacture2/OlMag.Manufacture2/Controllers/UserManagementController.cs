using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlMag.Manufacture2.Models.Requests.Identity;
using OlMag.Manufacture2.Models.Responses.Identity;

namespace OlMag.Manufacture2.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "UserAdministrator")]
public class UserManagementController(
    ILogger<UserManagementController> logger,
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager)
    : ControllerBase
{
    [HttpGet("healthcheck")]
    [AllowAnonymous]
    public async Task<IActionResult> HealthCheck()
    {
        logger.LogInformation("Health check success");
        return Ok("Success");
    }

    [HttpGet("healthcheckauth")]
    public async Task<IActionResult> HealthCheckAuth()
    {
        logger.LogInformation("Health check with auth role UserAdministrator success");
        return Ok("Success");
    }

    [HttpPost("users/{userId:guid}/addRole")]
    public async Task<IActionResult> AddUserRole(Guid userId, RoleRequest request)
    {
        logger.LogInformation("Add user role {userId} {role}", userId, request.RoleName);
        var roleName = request.RoleName;
        if (string.IsNullOrWhiteSpace(roleName))
        {
            logger.LogError("Role name is empty {role}", request.RoleName);
            return BadRequest("Role name is empty");
        }

        if (!await roleManager.RoleExistsAsync(roleName).ConfigureAwait(false))
        {
            logger.LogError("Role {role} not found", request.RoleName);
            return BadRequest($"Role {request.RoleName} not found");
        }
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            logger.LogError("User {userId} not found", userId);
            return BadRequest($"User {userId} not found");
        }

        await userManager.AddToRoleAsync(user, roleName).ConfigureAwait(false);
        logger.LogTrace("Add user role success");
        return Ok();
    }

    [HttpDelete("users/{userId:guid}/deleteRole/{roleName}")]
    public async Task<IActionResult> DeleteUserRole(Guid userId, string roleName)
    {
        logger.LogInformation("Delete user role {userId} {role}", userId, roleName);
        if (string.IsNullOrWhiteSpace(roleName))
        {
            logger.LogError("Role name is empty {role}", roleName);
            return BadRequest("Role name is empty");
        }

        if (!await roleManager.RoleExistsAsync(roleName).ConfigureAwait(false))
        {
            logger.LogError("Role {role} not found", roleName);
            return BadRequest($"Role {roleName} not found");
        }
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            logger.LogError("User {userId} not found", userId);
            return BadRequest($"User {userId} not found");
        }

        await userManager.RemoveFromRoleAsync(user, roleName).ConfigureAwait(false);
        logger.LogTrace("Delete user role success");
        return Ok();
    }

    [HttpGet("users/all")]
    public async Task<IActionResult> GetAllUsers()
    {
        logger.LogInformation("Get all users");
        var identityUsers = await userManager.Users.ToListAsync();
        var users = (await Task.WhenAll(
            identityUsers.Select(GetUserInfo))).ToArray();
        logger.LogTrace("Get all users success: count {count}", users.Length);
        return Ok(users);
    }

    [HttpGet("users/byEmail")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        logger.LogInformation("Get user by email");
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == email.ToUpper());
        if (user == null)
        {
            logger.LogWarning("User not found by {email}", email);
            return BadRequest("Not found");
        }
        var userInfo = await GetUserInfo(user);
        logger.LogTrace("Get user by email success");
        return Ok(userInfo);
    }

    [HttpGet("roles/all")]
    public async Task<IActionResult> AllRoles()
    {
        logger.LogInformation("Get all roles");
        var roles = await roleManager.Roles.Select(o => o.Name).OrderBy(o => o).ToListAsync();
        return Ok(roles);
    }

    private async Task<UserWithRolesResponse> GetUserInfo(IdentityUser user)
    {
        var roles = await userManager.GetRolesAsync(user).ConfigureAwait(false);
        return new UserWithRolesResponse
        {
            Id = user.Id,
            Name = user.UserName,
            Email = user.Email,
            Roles = [.. roles]
        };
    }
}