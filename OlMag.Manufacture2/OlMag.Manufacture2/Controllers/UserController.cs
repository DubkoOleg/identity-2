using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OlMag.Manufacture2.Models.Responses.Identity;
using System.Security.Claims;

namespace OlMag.Manufacture2.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(
    ILogger<UserController> logger,
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager)
    : ControllerBase
{
    [HttpGet("shortInfo")]
    [Authorize]
    public async Task<IActionResult> WithAuth()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.FindFirstValue(ClaimTypes.Name);
        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        return Ok(new UserShortInfoResponse
        {
            Id = userId,
            Name = userName,
            Email = userEmail,
        });
    }

    [HttpGet("current")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        logger.LogInformation("Get current user info");
        var user = await userManager.GetUserAsync(User);
        var roles = await userManager.GetRolesAsync(user).ConfigureAwait(false);
        var userInfo = new UserWithRolesResponse
        {
            Id = user.Id,
            Name = user.UserName,
            Email = user.Email,
            Roles = [.. roles]
        };
        return Ok(userInfo);
    }

}