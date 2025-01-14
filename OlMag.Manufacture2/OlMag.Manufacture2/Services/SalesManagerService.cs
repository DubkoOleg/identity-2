using Microsoft.EntityFrameworkCore;
using OlMag.Manufacture2.Data;
using OlMag.Manufacture2.Helpers.OperationResult;
using OlMag.Manufacture2.Interfaces;
using OlMag.Manufacture2.Models.Requests.SalesManager;
using OlMag.Manufacture2.Models.Responses.SalesManager;
using OlMag.Manufacture2.Services.SalesManagerRepositories;

namespace OlMag.Manufacture2.Services;

public class SalesManagerService(
    SalesManagementContext dbContext,
    CustomerRepository customerRepository,
    ContactPersonRepository contactPersonRepository,
    ILogger<SalesManagerService> logger)
    : ISalesManagerService
{
    public bool HealthCheck() => true;

    #region Customer

    public Task<OperationResult<CustomerInfoResponse>> GetCustomer(Guid customerId) => customerRepository.GetCustomer(customerId);
    public Task<OperationResult<CustomerResponse[]>> GetCustomers() => customerRepository.GetCustomers();

    public Task<OperationResult<CustomerResponse>> AddCustomer(CustomerBodyRequest request) => customerRepository.AddCustomer(request);

    public Task<OperationResult<CustomerResponse>> UpdateCustomer(Guid customerId, CustomerBodyRequest request) =>
        customerRepository.UpdateCustomer(customerId, request);

    public async Task<OperationResult> RemoveCustomer(Guid customerId)
    {
        logger.LogInformation("Remove customer {customerId}", customerId);
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            //todo если есть контактные лица и хоть один заказ
            //Заархивировать контактные лица и заказчика

            await dbContext.ContactPersons.Where(c => c.CustomerId == customerId).ExecuteDeleteAsync();
            await dbContext.Customers.Where(c => c.Id == customerId).ExecuteDeleteAsync();

            await transaction.CommitAsync();
            return OperationResultExtensions.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error remove customer");
            await transaction.RollbackAsync();
            return OperationResultExtensions.Failed("Ошибка удаления заказчика");
        }
    }

    #endregion Customer

    #region ContactPerson

    public Task<OperationResult<ContactPersonInfoResponse>> GetContactPerson(Guid contactPersonId) =>
        contactPersonRepository.GetContactPerson(contactPersonId);

    public Task<OperationResult<ContactPersonResponse[]>> GetContactPersonsByCustomer(Guid customerId) =>
        contactPersonRepository.GetContactPersonsByCustomer(customerId);

    public Task<OperationResult<ContactPersonResponse>> AddContactPerson(ContactPersonBodyRequest request, Guid customerId) =>
        contactPersonRepository.AddContactPerson(request, customerId);

    public Task<OperationResult<ContactPersonResponse>> UpdateContactPerson(Guid contactPersonId, ContactPersonBodyRequest request) =>
        contactPersonRepository.UpdateContactPerson(contactPersonId, request);

    public async Task<OperationResult> RemoveContactPerson(Guid contactPersonId)
    {
        logger.LogInformation("Remove contact person {contactPersonId}", contactPersonId);
        try
        {
            //todo если есть контактные лица и хоть один заказ
            //Заархивировать контактные лица и заказчика

            await dbContext.ContactPersons.Where(c => c.Id == contactPersonId).ExecuteDeleteAsync();

            return OperationResultExtensions.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error remove contact person");
            return OperationResultExtensions.Failed("Ошибка удаления контактного лица заказчика");
        }
    }

    #endregion ContactPerson
}