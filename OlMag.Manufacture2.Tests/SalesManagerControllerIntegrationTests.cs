using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using Mapster;
using OlMag.Manufacture2.Data;
using OlMag.Manufacture2.Helpers.OperationResult;
using OlMag.Manufacture2.Models.Entities.SalesManager;
using OlMag.Manufacture2.Models.Requests.SalesManager;
using OlMag.Manufacture2.Models.Responses.SalesManager;
using OlMag.Manufacture2.Tests.Maintenance;
using OlMag.Manufacture2.Tests.TestData;
using Xunit.Abstractions;

namespace OlMag.Manufacture2.Tests;

public class SalesManagerControllerIntegrationTests(WebAppFixture appFixture, ITestOutputHelper outputHelper)
    : TestBase(appFixture, outputHelper)
{
    private const string Endpoint = "SalesManager";

    [Fact]
    public async Task Customer_full_test()
    {
        // Создать пользователя
        // Дать роль менеджера по продажам
        // * удалить все иницализированные данные из БД
        // Создать заказчиков
        // Создать контактные лица заказчика
        // Получить всех заказчиков
        // Получить одного заказчика
        // Обновить заказчика
        // Удалить заказчика
        // Получить заказчика - 400
        // todo Добавить заказ менеджера по продажам
        // todo Удалить заказчика
        // todo Получить заказчика - успешно
        // todo Получить всех заказчиков - этого получить не должны

        var dumpContent = true;

        var salesManagerToken = await UserManagementControllerTests.CreateUserAndGetToken(client, EnRole.SalesManager);
        SetAuthorization(salesManagerToken);
        await DeleteAllCustomers();

        var customersCreated = await CreateCustomersWithContactPersons(dumpContent);
        var customersGet = await GetAllCustomers(customersCreated.Select(o => o.Customer).ToArray(), dumpContent);
        var customerCheck = customersCreated[1];
        var customerInfo = await GetCustomer(customerCheck.Customer, customerCheck.ContactPersons, dumpContent);
        var customerUpdate = await UpdateCustomer(customerCheck.Customer, dumpContent);
        await DeleteCustomer(customerCheck.Customer);
        await GetDeletedCustomer(customerCheck.Customer);
        customersCreated = customersCreated.Where(c => c.Customer.Id != customerCheck.Customer.Id).ToList();
        var customersGetAfterDelete =
            await GetAllCustomers(customersCreated.Select(o => o.Customer).ToArray(), dumpContent);
    }

    [Fact]
    public async Task ContactPerson_full_test()
    {
        // Создать заказчика
        // Создать контактные лица заказчика
        // * удалить все иницализированные данные из БД
        // Получить все контактные лица одного заказчика
        // Получить одно контактное лицо
        // Обновить одно контактное лицо
        // Удалить одно контактное лицо
        // Получить одно контактное лицо - 400
        // todo Добавить заказ менеджера по продажам
        // todo Удалить одно контактное лицо
        // todo Получить одно контактное лицо - успешно
        // todo Получить все контактные лица одного заказчика - этого получить не должны

        var dumpContent = true;

        var salesManagerToken = await UserManagementControllerTests.CreateUserAndGetToken(client, EnRole.SalesManager);
        SetAuthorization(salesManagerToken);
        await DeleteAllCustomers();

        var customersCreated = await CreateCustomersWithContactPersons(dumpContent);
        var customersGet = await GetAllCustomers(customersCreated.Select(o => o.Customer).ToArray(), dumpContent);
        var customerCheck = customersCreated[1];
        var contactPersonsGet =
            await GetContactPersonsByCustomer(customerCheck.Customer.Id, customerCheck.ContactPersons, dumpContent);
        var contactPersonCheck = customerCheck.ContactPersons[2];
        var contactPersonGet = await GetContactPerson(contactPersonCheck, customerCheck.Customer, dumpContent);
        var contactPersonUpdate = await UpdateContactPerson(contactPersonCheck, dumpContent);

        await DeleteContactPerson(contactPersonCheck);
        await GetDeletedContactPerson(contactPersonCheck);
        customersCreated = customersCreated.Where(c => c.Customer.Id != customerCheck.Customer.Id).ToList();
        customerCheck.ContactPersons = customerCheck.ContactPersons.Where(c => c.Id != contactPersonCheck.Id).ToArray();
        var contactPersonsGetAfterDelete =
            await GetContactPersonsByCustomer(customerCheck.Customer.Id, customerCheck.ContactPersons, dumpContent);
    }

    #region Customer

    private async Task<CustomerResponse> CreateCustomer(CustomerBodyRequest request, bool dumpContent = false)
    {
        var response = await client.PostAsync($"{Endpoint}/customer", CreateContent(request));

        var result = await Assert<CustomerResponse>(response, TestResult.Successful, data =>
        {
            var entity = mapper.Map<CustomerEntity>(request);
            entity.Id = data.Data.Id;
            var correctResult = mapper.Map<CustomerResponse>(entity);
            data.Data.Should().BeEquivalentTo(correctResult);
            return ValueTask.CompletedTask;
        }, dumpContent);
        return result;
    }

    private async Task<List<(CustomerResponse Customer, ContactPersonResponse[] ContactPersons)>> CreateCustomersWithContactPersons(
        bool dumpContent)
    {
        var customers = new List<(CustomerResponse Customer, ContactPersonResponse[] ContactPersons)>();

        for (var i = 0; i < 3; i++)
        {
            var customer = await CreateCustomer(fixture.Create<CustomerBodyRequest>(), dumpContent);
            var contactPersons = new List<ContactPersonResponse>();
            for (var j = 0; j < 3; j++)
            {
                var contactPerson = await CreateContactPerson(fixture.Create<ContactPersonBodyRequest>(), customer.Id,
                    dumpContent);
                contactPersons.Add(contactPerson);
            }

            customers.Add((customer, contactPersons.ToArray()));
        }

        return customers;
    }

    private async Task<CustomerResponse[]> GetAllCustomers(CustomerResponse[] customers, bool dumpContent)
    {
        var response = await client.GetAsync($"{Endpoint}/customer/all");
        var result = await Assert<CustomerResponse[]>(response, TestResult.Successful, data =>
        {
            data.Data.Should().BeEquivalentTo(customers);
            return ValueTask.CompletedTask;
        }, dumpContent);
        return result;
    }

    private async Task<CustomerInfoResponse> GetCustomer(CustomerResponse customerCheck,
        ContactPersonResponse[] customerCheckContactPersons, bool dumpContent)
    {
        var response = await client.GetAsync($"{Endpoint}/customer/{customerCheck.Id}");
        var result = await Assert<CustomerInfoResponse>(response, TestResult.Successful, data =>
        {
            var customer = mapper.Map<CustomerInfoResponse>(customerCheck);
            customer.ContactPersons = customerCheckContactPersons;

            data.Data.Should().BeEquivalentTo(customer);
            return ValueTask.CompletedTask;
        }, dumpContent);
        return result;
    }

    private async Task<CustomerResponse> UpdateCustomer(CustomerResponse customer, bool dumpContent)
    {
        var request = fixture.Create<CustomerBodyRequest>();

        var response = await client.PutAsync($"{Endpoint}/customer/{customer.Id}", CreateContent(request));
        var result = await Assert<CustomerResponse>(response, TestResult.Successful, data =>
        {
            request.Adapt(customer, mapper.Config);
            data.Data.Should().BeEquivalentTo(customer);
            return ValueTask.CompletedTask;
        }, dumpContent);
        return result;
    }

    private async Task DeleteCustomer(CustomerResponse customer)
    {
        var response = await client.DeleteAsync($"{Endpoint}/customer/{customer.Id}");
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    private async Task GetDeletedCustomer(CustomerResponse customer)
    {
        var response = await client.GetAsync($"{Endpoint}/customer/{customer.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var data = await response.Content.ReadFromJsonAsync<OperationResult<CustomerInfoResponse>>(JsonTestExtensions
            .JsonSerializerOptions);
        data.Error.Detail.Should().Be("Заказчик не найден");
    }

    private async Task DeleteAllCustomers()
    {
        var customersResponse = await client.GetAsync($"{Endpoint}/customer/all");
        customersResponse.IsSuccessStatusCode.Should().BeTrue();
        var customersData = await customersResponse.Content.ReadFromJsonAsync<CustomerResponse[]>(JsonTestExtensions.JsonSerializerOptions);

        foreach (var customer in customersData)
            await client.DeleteAsync($"{Endpoint}/customer/{customer.Id}");
    }

    #endregion Customer

    #region Contact person

    private async Task<ContactPersonResponse> CreateContactPerson(ContactPersonBodyRequest request, Guid customerId,
        bool dumpContent = false)
    {
        var response =
            await client.PostAsync($"{Endpoint}/customer/{customerId}/ContactPerson", CreateContent(request));

        var result = await Assert<ContactPersonResponse>(response, TestResult.Successful, data =>
        {
            var entity = mapper.Map<ContactPersonEntity>(request);
            entity.Id = data.Data.Id;
            var correctResult = mapper.Map<ContactPersonResponse>(entity);
            data.Data.Should().BeEquivalentTo(correctResult);
            return ValueTask.CompletedTask;
        }, dumpContent);
        return result;
    }

    private async Task<ContactPersonResponse[]> GetContactPersonsByCustomer(Guid customerId,
    ContactPersonResponse[] contactPersons, bool dumpContent)
    {
        var response = await client.GetAsync($"{Endpoint}/customer/{customerId}/ContactPerson");
        var result = await Assert<ContactPersonResponse[]>(response, TestResult.Successful, data =>
        {
            data.Data.Should().BeEquivalentTo(contactPersons);
            return ValueTask.CompletedTask;
        }, dumpContent);
        return result;
    }

    private async Task<ContactPersonInfoResponse> GetContactPerson(ContactPersonResponse contactPersonCheck, CustomerResponse customer,
        bool dumpContent)
    {
        var response = await client.GetAsync($"{Endpoint}/ContactPerson/{contactPersonCheck.Id}");
        var result = await Assert<ContactPersonInfoResponse>(response, TestResult.Successful, data =>
        {
            var contactPerson = mapper.Map<ContactPersonInfoResponse>(contactPersonCheck);
            contactPerson.Customer = customer;

            data.Data.Should().BeEquivalentTo(contactPerson);
            return ValueTask.CompletedTask;
        }, dumpContent);
        return result;
    }

    private async Task<ContactPersonResponse> UpdateContactPerson(ContactPersonResponse contactPerson, bool dumpContent)
    {
        var request = fixture.Create<ContactPersonBodyRequest>();

        var response = await client.PutAsync($"{Endpoint}/ContactPerson/{contactPerson.Id}", CreateContent(request));
        var result = await Assert<ContactPersonResponse>(response, TestResult.Successful, data =>
        {
            request.Adapt(contactPerson, mapper.Config);
            data.Data.Should().BeEquivalentTo(contactPerson);
            return ValueTask.CompletedTask;
        }, dumpContent);
        return result;
    }

    private async Task DeleteContactPerson(ContactPersonResponse contactPersonCheck)
    {
        var response = await client.DeleteAsync($"{Endpoint}/ContactPerson/{contactPersonCheck.Id}");
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    private async Task GetDeletedContactPerson(ContactPersonResponse contactPersonCheck)
    {
        var response = await client.GetAsync($"{Endpoint}/ContactPerson/{contactPersonCheck.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var data = await response.Content.ReadFromJsonAsync<OperationResult<ContactPersonInfoResponse>>(JsonTestExtensions
            .JsonSerializerOptions);
        data.Error.Detail.Should().Be("Контактное лицо заказчика не найдено");
    }

    #endregion Contact person
}