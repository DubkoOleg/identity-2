using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OlMag.Manufacture2.Data;
using OlMag.Manufacture2.Helpers.OperationResult;
using OlMag.Manufacture2.Models.Entities.SalesManager;
using OlMag.Manufacture2.Models.Requests.SalesManager;
using OlMag.Manufacture2.Models.Responses.SalesManager;

namespace OlMag.Manufacture2.Services.SalesManagerRepositories;

public class ContactPersonRepository(SalesManagementContext dbContext, IMapper mapper, ILogger<ContactPersonRepository> logger)
{
    internal const string Entity = "Контактное лицо заказчика";

    public async Task<OperationResult<ContactPersonInfoResponse>> GetContactPerson(Guid contactPersonId)
    {
        logger.LogInformation("Get contact person {contactPersonId}", contactPersonId);
        var contactPerson = await dbContext.ContactPersons
            .Include(contactPersonEntity => contactPersonEntity.Customer)
            .FirstOrDefaultAsync(c => c.Id == contactPersonId && c.DateArchived == null);
        return contactPerson != null
            ? mapper.Map<ContactPersonInfoResponse>(contactPerson)
            : OperationResultExtensions.Failed<ContactPersonInfoResponse>($"{Entity} не найдено");
    }

    public async Task<OperationResult<ContactPersonResponse[]>> GetContactPersonsByCustomer(Guid customerId)
    {
        logger.LogInformation("Get contact persons by customer {customer}", customerId);
        var contactPersons = await dbContext.ContactPersons
            .Where(c => c.CustomerId == customerId && c.DateArchived == null)
            .OrderBy(contactPerson => contactPerson.Name).ToArrayAsync();

        return mapper.Map<ContactPersonResponse[]>(contactPersons);
    }

    public async Task<OperationResult<ContactPersonResponse>> AddContactPerson(ContactPersonBodyRequest request, Guid customerId)
    {
        logger.LogInformation("Add contact person {@contactPerson}", request);
        var entity = mapper.Map<ContactPersonEntity>(request);
        entity.CustomerId = customerId;
        var contactPerson = await dbContext.ContactPersons.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return mapper.Map<ContactPersonResponse>(contactPerson.Entity);
    }

    public async Task<OperationResult<ContactPersonResponse>> UpdateContactPerson(Guid contactPersonId, ContactPersonBodyRequest request,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Update contact person {contactPersonId} {@contactPerson}", contactPersonId, request);
        var contactPersonDb = await dbContext.ContactPersons
            .FirstOrDefaultAsync(c => c.Id == contactPersonId && c.DateArchived == null,
                cancellationToken: cancellationToken);
        if (contactPersonDb == null)
        {
            logger.LogError("Contact person not found {contactPersonId}", contactPersonId);
            return OperationResultExtensions.Failed<ContactPersonResponse>($"{Entity} не найдено");
        }

        (contactPersonId, request).Adapt(contactPersonDb, mapper.Config);
        dbContext.ContactPersons.Update(contactPersonDb);

        var recCount = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        dbContext.Entry(contactPersonDb).State = EntityState.Detached;
        if (recCount != 1)
        {
            if (recCount == 0)
            {
                logger.LogError("Error save contact person {contactPersonId} {recCount}", contactPersonId, recCount);
                return OperationResultExtensions.Failed<ContactPersonResponse>($"{Entity} не обновлено");
            }

            logger.LogWarning("Save more info contact person {contactPersonId} {recCount}", contactPersonId, recCount);
        }

        return mapper.Map<ContactPersonResponse>(contactPersonDb);
    }
}