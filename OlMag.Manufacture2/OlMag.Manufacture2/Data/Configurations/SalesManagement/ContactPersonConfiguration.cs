using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OlMag.Manufacture2.Models.Entities.SalesManager;

namespace OlMag.Manufacture2.Data.Configurations.SalesManagement;

public class ContactPersonConfiguration : IEntityTypeConfiguration<ContactPersonEntity>
{
    public void Configure(EntityTypeBuilder<ContactPersonEntity> builder)
    {
        builder
            .Property(b => b.Created)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.OwnsOne(entity => entity.Information, modelBuilder => { modelBuilder.ToJson(); });
    }
}