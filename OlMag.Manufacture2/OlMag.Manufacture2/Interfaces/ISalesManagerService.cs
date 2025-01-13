using OlMag.Manufacture2.Models.Requests.SalesManager;
using OlMag.Manufacture2.Models.Responses.SalesManager;

namespace OlMag.Manufacture2.Interfaces;

public interface ISalesManagerService
{
    bool HealthCheck();
    Task<CustomerResponse> GetCustomer(Guid customerId);
    Task<CustomerResponse> AddCustomer(CustomerBodyRequest request);
    Task<CustomerResponse> UpdateCustomer(Guid customerId, CustomerBodyRequest request);
    Task<bool> RemoveCustomer(Guid customerId);
}