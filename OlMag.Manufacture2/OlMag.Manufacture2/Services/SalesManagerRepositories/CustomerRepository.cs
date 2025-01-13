using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OlMag.Manufacture2.Data;
using OlMag.Manufacture2.Models.Entities.SalesManager;
using OlMag.Manufacture2.Models.Requests.SalesManager;
using OlMag.Manufacture2.Models.Responses.SalesManager;
using Mapster;

namespace OlMag.Manufacture2.Services.SalesManagerRepositories;

public class CustomerRepository(SalesManagementContext dbContext, IMapper mapper, ILogger<CustomerRepository> logger)
{
    public async Task<CustomerInfoResponse> GetCustomer(Guid customerId)
    {
        logger.LogInformation("Get customer {customerId}", customerId);
        var customer = await dbContext.Customers
            .Include(customerEntity => customerEntity.ContactPersons)
            .FirstOrDefaultAsync(c => c.Id == customerId && c.DateArchived == null);
        if (customer == null)
            throw new Exception("Customer not found");
        return mapper.Map<CustomerInfoResponse>(customer);
    }

    public async Task<CustomerResponse[]> GetCustomers()
    {
        logger.LogInformation("Get all customers");
        var customers = await dbContext.Customers
            .Where(c => c.DateArchived == null).OrderBy(c => c.Name).ToArrayAsync();

        return mapper.Map<CustomerResponse[]>(customers);
    }

    public async Task<CustomerResponse> AddCustomer(CustomerBodyRequest request)
    {
        logger.LogInformation("Add customer {@customer}", request);
        var entity = mapper.Map<CustomerEntity>(request);
        var customer = await dbContext.Customers.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return mapper.Map<CustomerResponse>(customer.Entity);
    }

    public async Task<CustomerResponse> UpdateCustomer(Guid customerId, CustomerBodyRequest request,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Update customer {customerId} {@customer}", customerId, request);
        var customerDb = await dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == customerId && c.DateArchived == null,
                cancellationToken: cancellationToken);
        if (customerDb == null)
        {
            logger.LogError("Customer not found {customerId}", customerId);
            //todo change Exception to OperationResult<T>
            throw new Exception("Customer not found");
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
                //todo change Exception to OperationResult<T>
                throw new Exception("Customer not found");
            }

            logger.LogWarning("Save more info customer {customerId} {recCount}", customerId, recCount);
        }

        return mapper.Map<CustomerResponse>(customerDb);
    }

    public async Task<bool> RemoveCustomer(Guid customerId)
    {
        logger.LogInformation("Remove customer {customerId}", customerId);

        var result = await dbContext.Customers.Where(c => c.Id == customerId)
            .ExecuteDeleteAsync();
        return result == 1;
    }

    public async Task<bool> ArchivedCustomer(Guid customerId)
    {
        logger.LogInformation("Archived customer {customerId}", customerId);

        var result = await dbContext.Customers.Where(c => c.Id == customerId)
            .ExecuteUpdateAsync(o => o
                .SetProperty(c => c.DateArchived, DateTime.UtcNow));
        return result == 1;
    }
}