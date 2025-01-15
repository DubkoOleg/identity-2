using FluentAssertions;
using OlMag.Manufacture2.Models.Entities.SalesManager;
using OlMag.Manufacture2.Models.Responses.SalesManager;
using OlMag.Manufacture2.Tests.Maintenance;
using OlMag.Manufacture2.Tests.TestData;
using Xunit.Abstractions;

namespace OlMag.Manufacture2.Tests;

public class SalesManagerControllerTests(WebAppFixture appFixture, ITestOutputHelper outputHelper)
    : TestBase(appFixture, outputHelper)
{
    private const string Endpoint = "SalesManager";

    [Theory]
    [MemberData(nameof(SalesManagerData.CustomersForReadTest), MemberType = typeof(SalesManagerData))]
    internal async Task Customer_GetCustomer_success_test(CustomerEntity customer)
    {
        var response = await client.GetAsync($"{Endpoint}/customer/{customer.Id}");

        await Assert<CustomerResponse>(response, TestResult.Successful, data =>
        {
            var response = mapper.Map<CustomerResponse>(customer);
            data.Data.Should().BeEquivalentTo(response);
            return ValueTask.CompletedTask;
        }, true);
    }
}