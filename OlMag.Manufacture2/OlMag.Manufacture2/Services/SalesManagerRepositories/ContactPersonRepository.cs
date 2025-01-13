using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OlMag.Manufacture2.Data;
using OlMag.Manufacture2.Models.Entities.SalesManager;
using OlMag.Manufacture2.Models.Requests.SalesManager;
using OlMag.Manufacture2.Models.Responses.SalesManager;

namespace OlMag.Manufacture2.Services.SalesManagerRepositories;

public class ContactPersonRepository(SalesManagementContext dbContext, IMapper mapper, ILogger<ContactPersonRepository> logger)
{
    public async Task<ContactPersonInfoResponse> GetContactPerson(Guid contactPersonId)
    {
        logger.LogInformation("Get contact person {contactPersonId}", contactPersonId);
        var contactPerson = await dbContext.ContactPersons
            .Include(contactPersonEntity => contactPersonEntity.Customer)
            .FirstOrDefaultAsync(c => c.Id == contactPersonId && c.DateArchived == null);
        if (contactPerson == null)
            throw new Exception("Contact person not found");
        return mapper.Map<ContactPersonInfoResponse>(contactPerson);
    }

    public async Task<ContactPersonResponse[]> GetContactPersonsByCustomer(Guid customerId)
    {
        logger.LogInformation("Get contact persons by customer {customer}", customerId);
        var contactPersons = await dbContext.ContactPersons
            .Where(c => c.CustomerId == customerId && c.DateArchived == null)
            .OrderBy(contactPerson => contactPerson.Name).ToArrayAsync();

        return mapper.Map<ContactPersonResponse[]>(contactPersons);
    }

    public async Task<ContactPersonResponse> AddContactPerson(ContactPersonBodyRequest request, Guid customerId)
    {
        logger.LogInformation("Add contact person {@contactPerson}", request);
        var entity = mapper.Map<ContactPersonEntity>(request);
        var contactPerson = await dbContext.ContactPersons.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return mapper.Map<ContactPersonResponse>(contactPerson.Entity);
    }

    public async Task<ContactPersonResponse> UpdateContactPerson(Guid contactPersonId, ContactPersonBodyRequest request,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Update contact person {contactPersonId} {@contactPerson}", contactPersonId, request);
        var contactPersonDb = await dbContext.ContactPersons
            .FirstOrDefaultAsync(c => c.Id == contactPersonId && c.DateArchived == null,
                cancellationToken: cancellationToken);
        if (contactPersonDb == null)
        {
            logger.LogError("Contact person not found {contactPersonId}", contactPersonId);
            //todo change Exception to OperationResult<T>
            throw new Exception("Contact person not found");
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
                //todo change Exception to OperationResult<T>
                throw new Exception("Contact person not found");
            }

            logger.LogWarning("Save more info contact person {contactPersonId} {recCount}", contactPersonId, recCount);
        }

        return mapper.Map<ContactPersonResponse>(contactPersonDb);
    }

    public async Task<bool> RemoveContactPerson(Guid contactPersonId)
    {
        logger.LogInformation("Remove contact person {contactPersonId}", contactPersonId);

        var result = await dbContext.ContactPersons.Where(c => c.Id == contactPersonId)
            .ExecuteDeleteAsync();
        return result == 1;
    }

    public async Task<bool> ArchivedContactPerson(Guid contactPersonId)
    {
        logger.LogInformation("Archived contact person {contactPersonId}", contactPersonId);

        var result = await dbContext.ContactPersons.Where(c => c.Id == contactPersonId)
            .ExecuteUpdateAsync(o => o
                .SetProperty(c => c.DateArchived, DateTime.UtcNow));
        return result == 1;
    }
}