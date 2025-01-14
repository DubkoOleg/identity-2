using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OlMag.Manufacture2.Data;
using OlMag.Manufacture2.Tests.TestData;

namespace OlMag.Manufacture2.Tests.Maintenance;

public class TestDatabaseFixture
{
    private static readonly object Lock = new();
    private static bool _databaseInitialized;

    public TestDatabaseFixture()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Testing.json")
            .Build();
        var dbConnectionStringTesting = configuration.GetConnectionString("IdentityConnection")!;
        lock (Lock)
        {
            if (_databaseInitialized) return;
            using (var context = CreateDataContext(dbConnectionStringTesting))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                //DbInit(context);
            }

            using (var context = CreateSalesManagementContext(dbConnectionStringTesting))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                DbInit(context);
            }
            _databaseInitialized = true;
        }
    }

    private void DbInit(SalesManagementContext context)
    {
        SalesManagerData.InitDb(context);
    }

    public static DataContext CreateDataContext(string connectionString)
        => new(new DbContextOptionsBuilder<DataContext>()
            .UseNpgsql(connectionString)
            .Options);
    public static SalesManagementContext CreateSalesManagementContext(string connectionString)
        => new (new DbContextOptionsBuilder<SalesManagementContext>()
            .UseNpgsql(connectionString)
            .Options);
}