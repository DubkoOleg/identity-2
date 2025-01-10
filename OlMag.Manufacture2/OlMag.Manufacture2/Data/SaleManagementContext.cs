using Microsoft.EntityFrameworkCore;
using OlMag.Manufacture2.Models.Entities.SaleManager;

namespace OlMag.Manufacture2.Data;

//https://www.milanjovanovic.tech/blog/using-multiple-ef-core-dbcontext-in-single-application
public class SaleManagementContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<CustomerEntity> Customers { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("SaleManagement");

        base.OnModelCreating(modelBuilder);
    }
}