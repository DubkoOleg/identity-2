using OlMag.Manufacture2.Interfaces;
using OlMag.Manufacture2.Models.Requests.SalesManager;
using OlMag.Manufacture2.Models.Responses.SalesManager;
using OlMag.Manufacture2.Services.SalesManagerRepositories;

namespace OlMag.Manufacture2.Services;

public class SalesManagerService(
    CustomerRepository customerRepository,
    ContactPersonRepository contactPersonRepository,
    ILogger<SalesManagerService> logger)
    : ISalesManagerService
{
    public bool HealthCheck() => true;

    #region Customer

    public Task<CustomerInfoResponse> GetCustomer(Guid customerId) => customerRepository.GetCustomer(customerId);
    public Task<CustomerResponse[]> GetCustomers() => customerRepository.GetCustomers();

    public Task<CustomerResponse> AddCustomer(CustomerBodyRequest request) => customerRepository.AddCustomer(request);

    public Task<CustomerResponse> UpdateCustomer(Guid customerId, CustomerBodyRequest request) =>
        customerRepository.UpdateCustomer(customerId, request);

    public Task<bool> RemoveCustomer(Guid customerId)
    {
        //todo если нет контактного лица
        return customerRepository.RemoveCustomer(customerId);
        //todo если есть контактные лица, но нет заказов
        //Удалить контактные лица и заказчика
        //todo если есть контактные лица и хоть один заказ
        //Заархивировать контактные лица и заказчика
    }

    #endregion Customer

    #region ContactPerson

    public Task<ContactPersonInfoResponse> GetContactPerson(Guid contactPersonId) =>
        contactPersonRepository.GetContactPerson(contactPersonId);

    public Task<ContactPersonResponse[]> GetContactPersonsByCustomer(Guid customerId) =>
        contactPersonRepository.GetContactPersonsByCustomer(customerId);

    public Task<ContactPersonResponse> AddContactPerson(ContactPersonBodyRequest request, Guid customerId) =>
        contactPersonRepository.AddContactPerson(request, customerId);

    public Task<ContactPersonResponse> UpdateContactPerson(Guid contactPersonId, ContactPersonBodyRequest request) =>
        contactPersonRepository.UpdateContactPerson(contactPersonId, request);

    public Task<bool> RemoveContactPerson(Guid contactPersonId)
    {
        //todo если нет заказов
        return contactPersonRepository.RemoveContactPerson(contactPersonId);
        //todo если есть хоть один заказ
        return contactPersonRepository.ArchivedContactPerson(contactPersonId);
    }

    #endregion ContactPerson
}