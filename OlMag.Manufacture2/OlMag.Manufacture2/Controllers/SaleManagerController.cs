using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OlMag.Manufacture2.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class SaleManagerController(
    ILogger<UserManagementController> logger)
    : ControllerBase
{
    
}