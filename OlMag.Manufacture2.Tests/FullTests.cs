using AutoFixture;
using FluentAssertions;
using OlMag.Manufacture2.Data;
using OlMag.Manufacture2.Models.Entities.SalesManager;
using OlMag.Manufacture2.Models.Requests.SalesManager;
using OlMag.Manufacture2.Models.Responses.SalesManager;
using OlMag.Manufacture2.Tests.Maintenance;
using OlMag.Manufacture2.Tests.TestData;
using Xunit.Abstractions;

namespace OlMag.Manufacture2.Tests;

public class FullTests(WebAppFixture fixture, ITestOutputHelper outputHelper)
    : TestBase(fixture, outputHelper)
{
    [Fact]
    public async Task Success_test()
    {
        /* Минимальный успешный путь выполнения заказа:
        // Создать администратора пользователей
        // Создать менеджера по продажам
        // Создать заказчика
        // Создать контактное лицо заказчика
        // Создать заказ менеджера по продажам
        // Создать заказ на производство
        // ...
        */
        var dumpContent = false;

        // Создать администратора пользователей (на уровне инициализации)
        // Создать менеджера по продажам
        var salesManagerToken = await UserManagementControllerTests.CreateUserAndGetToken(client, EnRole.SalesManager);
        SetAuthorization(salesManagerToken);

        // Создать заказчика
        var customer = await CreateCustomer(fixture.Create<CustomerBodyRequest>(), dumpContent);
        // Создать контактное лицо заказчика
        var contactPerson = await CreateContactPerson(fixture.Create<ContactPersonBodyRequest>(), customer.Id, dumpContent);
        // Создать заказ менеджера по продажам

    }

    private async Task<CustomerResponse> CreateCustomer(CustomerBodyRequest request, bool dumpContent = false)
    {
        var response = await client.PostAsync("SalesManager/customer", CreateContent(request));

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

    private async Task<ContactPersonResponse> CreateContactPerson(ContactPersonBodyRequest request, Guid customerId,
        bool dumpContent = false)
    {
        var response =
            await client.PostAsync($"SalesManager/customer/{customerId}/ContactPerson", CreateContent(request));

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
}