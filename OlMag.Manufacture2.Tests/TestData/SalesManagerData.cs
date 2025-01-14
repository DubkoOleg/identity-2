using AutoFixture;
using Mapster;
using OlMag.Manufacture2.Data;
using OlMag.Manufacture2.Helpers;
using OlMag.Manufacture2.Models.Entities.SalesManager;
using OlMag.Manufacture2.Models.Requests.SalesManager;

namespace OlMag.Manufacture2.Tests.TestData;

public static class SalesManagerData
{
    static SalesManagerData()
    {
        var fixture = new Fixture();
        var mapperConfig = MapsterConfig.GetMapsterConfig();

        Customers = new int[3].Select(_ =>
        {
            var request = fixture.Create<CustomerBodyRequest>();
            return request.Adapt<CustomerEntity>(mapperConfig);
        }).ToArray();
    }

    public static CustomerEntity[] Customers { get; set; } 

    public static IEnumerable<object?[]> CustomersForReadTest => Customers.Select(customer => new object?[] { customer });

    public static void InitDb(SalesManagementContext context)
    {
        foreach (var customer in Customers)
            customer.Id = context.Customers.Add(customer).Entity.Id;

        context.SaveChanges();
    }
}