using Microsoft.AspNetCore.Identity;
using OlMag.Manufacture2.Data;

namespace OlMag.Manufacture2.Tests.TestData;

public class IdentityData
{
    internal const string UserAdministratorPassword = "OmI}Kh2r";

    public static void InitDb(DataContext context)
    {
        context.Users.Add(new IdentityUser
        {
            Id = "37956879-4676-4f43-8b9a-03f14ca0cc54",
            UserName = "UserAdministrator@te.st",
            NormalizedUserName = "USERADMINISTRATOR@TE.ST",
            Email = "UserAdministrator@te.st",
            NormalizedEmail = "USERADMINISTRATOR@TE.ST",
            EmailConfirmed = false,
            PasswordHash = "AQAAAAIAAYagAAAAEF3mZFX5ZITFjtav0uFehEEkYhylz3ciUf8BT2FA0P6SjP1UXFi1Kfpvh8GZC/fGdQ==",
            SecurityStamp = "7EHZIGXIC2JKAUUCQT6X2YTWMI2OFX7L",
            ConcurrencyStamp = "4ea46425-494d-4699-a411-7f0ab6996b6e",
            PhoneNumber = null,
            PhoneNumberConfirmed = false,
            LockoutEnd = null,
            LockoutEnabled = true,
            AccessFailedCount = 0
        });
        context.UserRoles.Add(new IdentityUserRole<string>()
        {
            UserId = "37956879-4676-4f43-8b9a-03f14ca0cc54",
            RoleId = ((int)EnRole.UserAdministrator).ToString()
        });

        context.SaveChanges();
    }

    public class LoginResponse
    {
        public string TokenType { get; set; }
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
    }
}