using OlMag.Manufacture2.Data;
using OlMag.Manufacture2.Models.Entities.SalesManager;

namespace OlMag.Manufacture2.Tests.TestData;

public static class SalesManagerData
{
    public static List<CustomerEntity> Customers { get; set; } = [];
    public static CustomerEntity[] Customers2 { get; set; } = new CustomerEntity[]
    {
        new()
        {
            Name = "345",
            Information = new CustomerInformationEntity()
            {
                Phone = "123",
                Email = "234",
                Note = "456",
            }
        }
    };
    public static IEnumerable<object?[]> CustomersForReadTest => Customers2.Select(customer => new object?[] { customer });

    public static void InitDb(SalesManagementContext context)
    {
        var customers = new CustomerEntity[]
        {
            new()
            {
                Name = "345",
                Information = new CustomerInformationEntity()
                {
                    Phone = "123",
                    Email = "234",
                    Note = "456",
                }
            }
        };
        foreach (var customer in Customers2)
        {
            //var entity = mapper.Map<CustomerEntity>(customer);
            //Customers.Add(context.Customers.Add(customer).Entity);
            customer.Id = context.Customers.Add(customer).Entity.Id;
        }

        context.SaveChanges();
    }
}