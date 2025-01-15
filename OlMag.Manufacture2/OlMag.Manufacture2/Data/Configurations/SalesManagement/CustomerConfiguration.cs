using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OlMag.Manufacture2.Models.Entities.SalesManager;

namespace OlMag.Manufacture2.Data.Configurations.SalesManagement;

internal class CustomerConfiguration : IEntityTypeConfiguration<CustomerEntity>
{
    public void Configure(EntityTypeBuilder<CustomerEntity> builder)
    {
        builder
            .Property(b => b.Created)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.OwnsOne(entity => entity.Information, modelBuilder => { modelBuilder.ToJson(); });
    }
}