using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OlMag.Manufacture2.Interfaces;
using OlMag.Manufacture2.Models.Requests.SalesManager;

namespace OlMag.Manufacture2.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class SalesManagerController(
    ISalesManagerService salesManagerService,
    ILogger<UserManagementController> logger)
    : ControllerBase
{
    #region healthcheck

    /// <summary>
    /// Проверка работоспособности контроллера
    /// </summary>
    [HttpGet("healthcheck")]
    public async Task<IActionResult> HealthCheck()
    {
        logger.LogInformation("Health check success");
        return Ok("Success");
    }

    /// <summary>
    /// Проверка работоспособности контроллера с авторизацией
    /// </summary>
    [HttpGet("healthcheckfull")]
    [Authorize(Roles = "SalesManager")]
    public async Task<IActionResult> HealthCheckFull()
    {
        if (!salesManagerService.HealthCheck()) return BadRequest();
        logger.LogInformation("Health check role and service success");
        return Ok("Success");
    }

    #endregion healthcheck

    #region Customer

    /// <summary>
    /// Получить заказчика со всеми контактными лицами
    /// </summary>
    /// <param name="id">Id заказчика</param>
    [HttpGet("customer/{id:guid}")]
    public async Task<IActionResult> GetCustomer(Guid id)
    {
        var result = await salesManagerService.GetCustomer(id).ConfigureAwait(false);
        return Ok(result);
    }

    //todo add paging, filters, sorting
    /// <summary>
    /// Получить всех заказчиков
    /// </summary>
    [HttpGet("customer/all")]
    public async Task<IActionResult> GetCustomers()
    {
        var result = await salesManagerService.GetCustomers().ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Добавить заказчика
    /// </summary>
    [HttpPost("customer")]
    [Authorize(Roles = "SalesManager")]
    public async Task<IActionResult> CreateCustomer(CustomerBodyRequest request)
    {
        var result = await salesManagerService.AddCustomer(request).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Обновить заказчика
    /// </summary>
    /// <param name="id">Id заказчика</param>
    [HttpPut("customer/{id:guid}")]
    [Authorize(Roles = "SalesManager")]
    public async Task<IActionResult> UpdateCustomer(Guid id, CustomerRequest request)
    {
        var result = await salesManagerService.UpdateCustomer(id, request).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Удалить или заархивировать заказчика
    /// </summary>
    /// <param name="id">Id заказчика</param>
    [HttpDelete("customer/{id:guid}")]
    [Authorize(Roles = "SalesManager")]
    public async Task<IActionResult> DeleteCustomer(Guid id)
    {
        var result = await salesManagerService.RemoveCustomer(id).ConfigureAwait(false);
        return result ? Ok() : BadRequest();
    }

    #endregion Customer

    #region ContactPerson

    /// <summary>
    /// Получить контактное лицое и заказчика
    /// </summary>
    /// <param name="id">Id контактного лица</param>
    [HttpGet("ContactPerson/{id:guid}")]
    public async Task<IActionResult> GetContactPerson(Guid id)
    {
        var result = await salesManagerService.GetContactPerson(id).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Получить контактные лица заказчика
    /// </summary>
    /// <param name="customerId">Id заказчика</param>
    [HttpGet("customer/{customerId:guid}/ContactPerson")]
    public async Task<IActionResult> GetContactPersons(Guid customerId)
    {
        var result = await salesManagerService.GetContactPersonsByCustomer(customerId).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Добавить контактное лицое
    /// </summary>
    [HttpPost("customer/{customerId:guid}/ContactPerson")]
    [Authorize(Roles = "SalesManager")]
    public async Task<IActionResult> CreateContactPerson(Guid customerId, ContactPersonBodyRequest request)
    {
        var result = await salesManagerService.AddContactPerson(request, customerId).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Обновить контактное лицое
    /// </summary>
    /// <param name="id">Id контактного лица</param>
    [HttpPut("ContactPerson/{id:guid}")]
    [Authorize(Roles = "SalesManager")]
    public async Task<IActionResult> UpdateContactPerson(Guid id, ContactPersonRequest request)
    {
        var result = await salesManagerService.UpdateContactPerson(id, request).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Удалить или заархивировать контактное лицое
    /// </summary>
    /// <param name="id">Id контактного лица</param>
    [HttpDelete("ContactPerson/{id:guid}")]
    [Authorize(Roles = "SalesManager")]
    public async Task<IActionResult> DeleteContactPerson(Guid id)
    {
        var result = await salesManagerService.RemoveContactPerson(id).ConfigureAwait(false);
        return result ? Ok() : BadRequest();
    }

    #endregion ContactPerson
}