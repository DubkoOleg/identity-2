using OlMag.Manufacture2.Interfaces;
using OlMag.Manufacture2.Models.Requests.SalesManager;
using OlMag.Manufacture2.Models.Responses.SalesManager;
using OlMag.Manufacture2.Services.SalesManagerRepositories;

namespace OlMag.Manufacture2.Services;

public class SalesManagerService(CustomerRepository customerRepository, ILogger<SalesManagerService> logger)
    : ISalesManagerService
{
    public bool HealthCheck() => true;

    public Task<CustomerResponse> GetCustomer(Guid customerId)
    {
        //todo возвращать с контактными лицами
        return customerRepository.GetCustomer(customerId);
    }

    public Task<CustomerResponse> AddCustomer(CustomerBodyRequest request) => customerRepository.AddCustomer(request);

    public Task<CustomerResponse> UpdateCustomer(Guid customerId, CustomerBodyRequest request) => customerRepository.UpdateCustomer(customerId, request);

    public Task<bool> RemoveCustomer(Guid customerId)
    {
        //todo если нет контактного лица
        return customerRepository.RemoveCustomer(customerId);
        //todo если есть контактные лица, но нет заказов
        //Удалить контактные лица и заказчика
        //todo если есть контактные лица и хоть один заказ
        //Заархивировать контактные лица и заказчика
    }
}