using FluentAssertions;
using OlMag.Manufacture2.Models.Entities.SalesManager;
using OlMag.Manufacture2.Tests.Maintenance;
using OlMag.Manufacture2.Tests.TestData;
using Xunit.Abstractions;

namespace OlMag.Manufacture2.Tests;

public class SalesManagerControllerTests(WebAppFixture fixture, ITestOutputHelper outputHelper)
    : TestBase(fixture, outputHelper)
{
    private const string Endpoint = "SalesManager";

    [Theory]
    [MemberData(nameof(SalesManagerData.CustomersForReadTest), MemberType = typeof(SalesManagerData))]
    public async Task get_by_id_test(CustomerEntity customer)
    {
        var response = await client.GetAsync($"{Endpoint}/customer/{customer.Id}");
        response.IsSuccessStatusCode.Should().BeTrue();
        var content = await response.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(500);
    }
}