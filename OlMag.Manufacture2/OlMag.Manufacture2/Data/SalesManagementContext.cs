using Microsoft.EntityFrameworkCore;
using OlMag.Manufacture2.Data.Configurations.SalesManagement;
using OlMag.Manufacture2.Models.Entities.SalesManager;

namespace OlMag.Manufacture2.Data;

//https://www.milanjovanovic.tech/blog/using-multiple-ef-core-dbcontext-in-single-application
public class SalesManagementContext(DbContextOptions options) : DbContext(options)
{
    public const string Schema = "SalesManagement";
    public DbSet<CustomerEntity> Customers { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}