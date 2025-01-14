using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OlMag.Manufacture2.Data;
using OlMag.Manufacture2.Models.Entities.SalesManager;
using OlMag.Manufacture2.Models.Requests.SalesManager;
using OlMag.Manufacture2.Models.Responses.SalesManager;
using Mapster;
using OlMag.Manufacture2.Helpers.OperationResult;

namespace OlMag.Manufacture2.Services.SalesManagerRepositories;

public class CustomerRepository(SalesManagementContext dbContext, IMapper mapper, ILogger<CustomerRepository> logger)
{
    internal const string Entity = "Заказчик";

    public async Task<OperationResult<CustomerInfoResponse>> GetCustomer(Guid customerId)
    {
        logger.LogInformation("Get customer {customerId}", customerId);
        var customer = await dbContext.Customers
            .Include(customerEntity => customerEntity.ContactPersons)
            .FirstOrDefaultAsync(c => c.Id == customerId && c.DateArchived == null);
        return customer != null
            ? mapper.Map<CustomerInfoResponse>(customer)
            : OperationResultExtensions.Failed<CustomerInfoResponse>($"{Entity} не найден");
    }

    public async Task<OperationResult<CustomerResponse[]>> GetCustomers()
    {
        logger.LogInformation("Get all customers");
        var customers = await dbContext.Customers
            .Where(c => c.DateArchived == null).OrderBy(c => c.Name).ToArrayAsync();

        return mapper.Map<CustomerResponse[]>(customers);
    }

    public async Task<OperationResult<CustomerResponse>> AddCustomer(CustomerBodyRequest request)
    {
        logger.LogInformation("Add customer {@customer}", request);
        var entity = mapper.Map<CustomerEntity>(request);
        var customer = await dbContext.Customers.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return mapper.Map<CustomerResponse>(customer.Entity);
    }

    public async Task<OperationResult<CustomerResponse>> UpdateCustomer(Guid customerId, CustomerBodyRequest request,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Update customer {customerId} {@customer}", customerId, request);
        var customerDb = await dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == customerId && c.DateArchived == null,
                cancellationToken: cancellationToken);
        if (customerDb == null)
        {
            logger.LogError("Customer not found {customerId}", customerId);
            return OperationResultExtensions.Failed<CustomerResponse>($"{Entity} не найден");
        }

        (customerId, request).Adapt(customerDb, mapper.Config);
        dbContext.Customers.Update(customerDb);

        var recCount = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        dbContext.Entry(customerDb).State = EntityState.Detached;
        if (recCount != 1)
        {
            if (recCount == 0)
            {
                logger.LogError("Error save customer {customerId} {recCount}", customerId, recCount);
                return OperationResultExtensions.Failed<CustomerResponse>($"{Entity} не обновлен");
            }

            logger.LogWarning("Save more info customer {customerId} {recCount}", customerId, recCount);
        }

        return mapper.Map<CustomerResponse>(customerDb);
    }
}