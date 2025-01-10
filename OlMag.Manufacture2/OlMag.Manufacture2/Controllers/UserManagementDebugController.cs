using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OlMag.Manufacture2.Models.Requests;
using OlMag.Manufacture2.Models.Responses;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace OlMag.Manufacture2.Controllers
{
    #if DEBUG

    [ApiController]
    [Route("[controller]")]
    public class UserManagementDebugController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserManagementDebugController> _logger;

        public UserManagementDebugController(ILogger<UserManagementDebugController> logger, 
            UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("getData")]
        [Authorize]
        [Obsolete]
        public async Task<IActionResult> WithAuth()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
            var userName = User.FindFirstValue(ClaimTypes.Name); // will give the user's userName

            // For ASP.NET Core <= 3.1
            IdentityUser applicationUser = await _userManager.GetUserAsync(User);
            string userEmail = applicationUser?.Email; // will give the user's Email

            // For ASP.NET Core >= 5.0
            var userEmail2 = User.FindFirstValue(ClaimTypes.Email); // will give the user's Email
            
            return Ok("success");
        }

        [HttpGet("users/current")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            var userInfo = new UserWithRolesResponse
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                Roles = roles.ToArray()
            };
            return Ok(userInfo);
        }

        [HttpPost("role/create")]
        [Authorize]
        public async Task<IActionResult> CreateRole(CreateRoleRequest request)
        {
            var roleName = request.RoleName;
            if (string.IsNullOrWhiteSpace(roleName)) return BadRequest("Role name is empty");
            if (await _roleManager.RoleExistsAsync(roleName)) return Conflict("Role already created");

            await _roleManager.CreateAsync(new IdentityRole
            {
                 Name = roleName,
                 NormalizedName = roleName.ToUpper()
            });

            return Ok("success");
        }

        [HttpPost("role/addUser")]
        [HttpPost("user/addRole")]
        [Authorize]
        public async Task<IActionResult> AddUserRole(AddRoleRequest request)
        {
            var roleName = request.RoleName;
            if (string.IsNullOrWhiteSpace(roleName)) return BadRequest("Role name is empty");
            if (!await _roleManager.RoleExistsAsync(roleName)) return BadRequest($"Role {request.RoleName} not found");
            var user = await _userManager.FindByEmailAsync(request.UserEmail);
            if (user == null) return BadRequest($"User with email {request.UserEmail} not found");

            await _userManager.AddToRoleAsync(user, roleName);

            return Ok("success");
        }

        [HttpDelete("role/deleteUser")]
        [HttpDelete("user/deleteRole")]
        [Authorize]
        public async Task<IActionResult> DeleteUserRole(AddRoleRequest request)
        {
            var roleName = request.RoleName;
            if (string.IsNullOrWhiteSpace(roleName)) return BadRequest("Role name is empty");
            if (!await _roleManager.RoleExistsAsync(roleName)) return BadRequest($"Role {request.RoleName} not found");
            var user = await _userManager.FindByEmailAsync(request.UserEmail);
            if (user == null) return BadRequest($"User with email {request.UserEmail} not found");

            await _userManager.RemoveFromRoleAsync(user, roleName);

            return Ok("success");
        }

        [HttpGet("resetPassword/getResetToken")]
        public async Task<IActionResult> Reset(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null) return BadRequest($"User with email {userEmail} not found");
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            return Ok(code);
        }
    }

    public class AddRoleRequest
    {
        public string RoleName { get; set; }
        public string UserEmail { get; set; }
    }
    public class CreateRoleRequest
    {
        [Required]
        public string RoleName { get; set; }
    }

#endif
}
