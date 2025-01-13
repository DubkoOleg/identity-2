using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OlMag.Manufacture2.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        var roles = Enum.GetValues<EnRole>().Select(role =>
            new IdentityRole
            {
                Id = ((int)role).ToString(),
                Name = role.ToString(),
                NormalizedName = role.ToString().ToUpper(),
            }
        );
        builder.HasData(roles);
    }
}