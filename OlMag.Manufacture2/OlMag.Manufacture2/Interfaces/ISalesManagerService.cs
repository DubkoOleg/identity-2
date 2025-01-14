using OlMag.Manufacture2.Helpers.OperationResult;
using OlMag.Manufacture2.Models.Requests.SalesManager;
using OlMag.Manufacture2.Models.Responses.SalesManager;

namespace OlMag.Manufacture2.Interfaces;

public interface ISalesManagerService
{
    bool HealthCheck();

    Task<OperationResult<CustomerInfoResponse>> GetCustomer(Guid customerId);
    Task<OperationResult<CustomerResponse[]>> GetCustomers();
    Task<OperationResult<CustomerResponse>> AddCustomer(CustomerBodyRequest request);
    Task<OperationResult<CustomerResponse>> UpdateCustomer(Guid customerId, CustomerBodyRequest request);
    Task<OperationResult> RemoveCustomer(Guid customerId);

    Task<OperationResult<ContactPersonInfoResponse>> GetContactPerson(Guid contactPersonId);
    Task<OperationResult<ContactPersonResponse[]>> GetContactPersonsByCustomer(Guid customerId);
    Task<OperationResult<ContactPersonResponse>> AddContactPerson(ContactPersonBodyRequest request, Guid customerId);
    Task<OperationResult<ContactPersonResponse>> UpdateContactPerson(Guid contactPersonId, ContactPersonBodyRequest request);
    Task<OperationResult> RemoveContactPerson(Guid contactPersonId);
}