using OlMag.Manufacture2.Models.Requests.SalesManager;
using OlMag.Manufacture2.Models.Responses.SalesManager;

namespace OlMag.Manufacture2.Interfaces;

public interface ISalesManagerService
{
    bool HealthCheck();

    Task<CustomerInfoResponse> GetCustomer(Guid customerId);
    Task<CustomerResponse[]> GetCustomers();
    Task<CustomerResponse> AddCustomer(CustomerBodyRequest request);
    Task<CustomerResponse> UpdateCustomer(Guid customerId, CustomerBodyRequest request);
    Task<bool> RemoveCustomer(Guid customerId);

    Task<ContactPersonInfoResponse> GetContactPerson(Guid contactPersonId);
    Task<ContactPersonResponse[]> GetContactPersonsByCustomer(Guid customerId);
    Task<ContactPersonResponse> AddContactPerson(ContactPersonBodyRequest request, Guid customerId);
    Task<ContactPersonResponse> UpdateContactPerson(Guid contactPersonId, ContactPersonBodyRequest request);
    Task<bool> RemoveContactPerson(Guid contactPersonId);
}