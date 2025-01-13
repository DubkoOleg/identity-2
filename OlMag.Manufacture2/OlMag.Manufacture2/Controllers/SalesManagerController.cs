using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OlMag.Manufacture2.Interfaces;
using OlMag.Manufacture2.Models.Requests.SalesManager;
using OlMag.Manufacture2.Services;

namespace OlMag.Manufacture2.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class SalesManagerController(
    ISalesManagerService salesManagerService,
    ILogger<UserManagementController> logger)
    : ControllerBase
{
    [HttpGet("healthcheck")]
    public async Task<IActionResult> HealthCheck()
    {
        logger.LogInformation("Health check success");
        return Ok("Success");
    }

    [HttpGet("healthcheckfull")]
    [Authorize(Roles = "SalesManager")]
    public async Task<IActionResult> HealthCheckFull()
    {
        if(!salesManagerService.HealthCheck()) return BadRequest();
        logger.LogInformation("Health check role and service success");
        return Ok("Success");
    }

    #region Customer

    [HttpGet("customer/{id:guid}")]
    [Authorize(Roles = "SalesManager")]
    public async Task<IActionResult> GetCustomer(Guid id)
    {
        var result = await salesManagerService.GetCustomer(id).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpPost("customer")]
    [Authorize(Roles = "SalesManager")]
    public async Task<IActionResult> CreateCustomer(CustomerBodyRequest request)
    {
        var result = await salesManagerService.AddCustomer(request).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpPut("customer/{id:guid}")]
    [Authorize(Roles = "SalesManager")]
    public async Task<IActionResult> UpdateCustomer(Guid id, CustomerRequest request)
    {
        var result = await salesManagerService.UpdateCustomer(id, request).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpDelete("customer/{id:guid}")]
    [Authorize(Roles = "SalesManager")]
    public async Task<IActionResult> DeleteCustomer(Guid id)
    {
        var result = await salesManagerService.RemoveCustomer(id).ConfigureAwait(false);
        return Ok(result);
    }

    #endregion Customer
}