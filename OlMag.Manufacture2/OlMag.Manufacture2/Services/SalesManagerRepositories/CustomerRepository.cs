using Microsoft.EntityFrameworkCore;
using OlMag.Manufacture2.Data;
using OlMag.Manufacture2.Models.Entities.SalesManager;
using OlMag.Manufacture2.Models.Requests.SalesManager;
using OlMag.Manufacture2.Models.Responses.SalesManager;

namespace OlMag.Manufacture2.Services.SalesManagerRepositories;

public class CustomerRepository(SalesManagementContext dbContext, ILogger<SalesManagerService> logger)
{
    public async Task<CustomerResponse> GetCustomer(Guid customerId)
    {
        logger.LogInformation("Get customer {customerId}", customerId);
        var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == customerId && c.DateArchived == null);
        if (customer == null)
            throw new Exception("Customer not found");
        return new CustomerResponse
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Information.Email,
            Phone = customer.Information.Phone,
        };
    }

    public async Task<CustomerResponse> AddCustomer(CustomerBodyRequest request)
    {
        logger.LogInformation("Add customer {@customer}", request);
        var customer = await dbContext.Customers.AddAsync(
            new CustomerEntity
            {
                Name = request.Name,
                Information = new CustomerInformationEntity()
                {
                    Email = request.Email,
                    Phone = request.Phone
                }
            });
        await dbContext.SaveChangesAsync();
        return new CustomerResponse
        {
            Id = customer.Entity.Id,
            Name = customer.Entity.Name,
            Email = customer.Entity.Information.Email,
            Phone = customer.Entity.Information.Phone,
        };
    }

    public async Task<CustomerResponse> UpdateCustomer(Guid customerId, CustomerBodyRequest request)
    {
        logger.LogInformation("Update customer {customerId} {@customer}", customerId, request);
        var result = await dbContext.Customers.Where(c => c.Id == customerId)
            .ExecuteUpdateAsync(o => o
                .SetProperty(c => c.Name, request.Name)
                .SetProperty(c => c.Information, new CustomerInformationEntity()
                {
                    Email = request.Email,
                    Phone = request.Phone
                }));
        if (result == 1)
            throw new Exception("Customer not found");

        return await GetCustomer(customerId);
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