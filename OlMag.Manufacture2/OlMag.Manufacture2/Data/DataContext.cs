using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OlMag.Manufacture2.Data
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var roles = Enum.GetValues<EnRole>().Select(role =>            
                new IdentityRole
                {
                    Id = ((int)role).ToString(),
                    Name = role.ToString(),
                    NormalizedName = role.ToString().ToUpper(),
                }
            );
            modelBuilder.Entity<IdentityRole>().HasData(roles);

            base.OnModelCreating(modelBuilder);
        }
    }
}
