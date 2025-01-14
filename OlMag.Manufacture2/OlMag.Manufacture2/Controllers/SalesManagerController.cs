using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OlMag.Manufacture2.Controllers.Base;
using OlMag.Manufacture2.Interfaces;
using OlMag.Manufacture2.Models.Requests.SalesManager;
using OlMag.Manufacture2.Models.Responses.SalesManager;

namespace OlMag.Manufacture2.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class SalesManagerController(
    ISalesManagerService salesManagerService,
    IMapper mapper,
    ILogger<UserManagementController> logger)
    : BaseApiController(mapper, logger)
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
    public async Task<ActionResult<CustomerInfoResponse>> GetCustomer(Guid id)
    {
        var result = await salesManagerService.GetCustomer(id).ConfigureAwait(false);
        return Result(result);
    }

    //todo add paging, filters, sorting
    /// <summary>
    /// Получить всех заказчиков
    /// </summary>
    [HttpGet("customer/all")]
    public async Task<ActionResult<CustomerResponse[]>> GetCustomers()
    {
        var result = await salesManagerService.GetCustomers().ConfigureAwait(false);
        return Result(result);
    }

    /// <summary>
    /// Добавить заказчика
    /// </summary>
    [HttpPost("customer")]
    [Authorize(Roles = "SalesManager")]
    public async Task<ActionResult<CustomerResponse>> CreateCustomer(CustomerBodyRequest request)
    {
        var result = await salesManagerService.AddCustomer(request).ConfigureAwait(false);
        return Result(result);
    }

    /// <summary>
    /// Обновить заказчика
    /// </summary>
    /// <param name="id">Id заказчика</param>
    [HttpPut("customer/{id:guid}")]
    [Authorize(Roles = "SalesManager")]
    public async Task<ActionResult<CustomerResponse>> UpdateCustomer(Guid id, CustomerRequest request)
    {
        var result = await salesManagerService.UpdateCustomer(id, request).ConfigureAwait(false);
        return Result(result);
    }

    /// <summary>
    /// Удалить или заархивировать заказчика
    /// </summary>
    /// <param name="id">Id заказчика</param>
    [HttpDelete("customer/{id:guid}")]
    [Authorize(Roles = "SalesManager")]
    public async Task<ActionResult> DeleteCustomer(Guid id)
    {
        var result = await salesManagerService.RemoveCustomer(id).ConfigureAwait(false);
        return Result(result);
    }

    #endregion Customer

    #region ContactPerson

    /// <summary>
    /// Получить контактное лицое и заказчика
    /// </summary>
    /// <param name="id">Id контактного лица</param>
    [HttpGet("ContactPerson/{id:guid}")]
    public async Task<ActionResult<ContactPersonInfoResponse>> GetContactPerson(Guid id)
    {
        var result = await salesManagerService.GetContactPerson(id).ConfigureAwait(false);
        return Result(result);
    }

    /// <summary>
    /// Получить контактные лица заказчика
    /// </summary>
    /// <param name="customerId">Id заказчика</param>
    [HttpGet("customer/{customerId:guid}/ContactPerson")]
    public async Task<ActionResult<ContactPersonResponse[]>> GetContactPersons(Guid customerId)
    {
        var result = await salesManagerService.GetContactPersonsByCustomer(customerId).ConfigureAwait(false);
        return Result(result);
    }

    /// <summary>
    /// Добавить контактное лицое
    /// </summary>
    [HttpPost("customer/{customerId:guid}/ContactPerson")]
    [Authorize(Roles = "SalesManager")]
    public async Task<ActionResult<ContactPersonResponse>> CreateContactPerson(Guid customerId, ContactPersonBodyRequest request)
    {
        var result = await salesManagerService.AddContactPerson(request, customerId).ConfigureAwait(false);
        return Result(result);
    }

    /// <summary>
    /// Обновить контактное лицое
    /// </summary>
    /// <param name="id">Id контактного лица</param>
    [HttpPut("ContactPerson/{id:guid}")]
    [Authorize(Roles = "SalesManager")]
    public async Task<ActionResult<ContactPersonResponse>> UpdateContactPerson(Guid id, ContactPersonRequest request)
    {
        var result = await salesManagerService.UpdateContactPerson(id, request).ConfigureAwait(false);
        return Result(result);
    }

    /// <summary>
    /// Удалить или заархивировать контактное лицое
    /// </summary>
    /// <param name="id">Id контактного лица</param>
    [HttpDelete("ContactPerson/{id:guid}")]
    [Authorize(Roles = "SalesManager")]
    public async Task<ActionResult> DeleteContactPerson(Guid id)
    {
        var result = await salesManagerService.RemoveContactPerson(id).ConfigureAwait(false);
        return Result(result);
    }

    #endregion ContactPerson
}