using System.Net.Http.Headers;
using OlMag.Manufacture2.Data;
using OlMag.Manufacture2.Models.Responses.Identity;
using OlMag.Manufacture2.Tests.Maintenance;
using System.Net.Http.Json;
using Xunit.Abstractions;
using static OlMag.Manufacture2.Tests.TestData.IdentityData;
using FluentAssertions;
using OlMag.Manufacture2.Tests.TestData;

namespace OlMag.Manufacture2.Tests;

public class UserManagementControllerTests(WebAppFixture fixture, ITestOutputHelper outputHelper)
    : TestBase(fixture, outputHelper)
{
    private const string Endpoint = "UserManagement";
    private const string Password = IdentityData.UserAdministratorPassword;

    public static async Task<string> CreateUserAndGetToken(HttpClient client, EnRole role)
    {
        var userRequest = new
        {
            Email = $"{role}@te.st",
            Password = Password
        };
        var registerResponse = await client.PostAsync("register", CreateContent(userRequest));
        registerResponse.IsSuccessStatusCode.Should().BeTrue();

        var userAdministratorToken = await GetToken(client, EnRole.UserAdministrator, UserAdministratorPassword);
        client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {userAdministratorToken}");

        var userResponse = await client.GetAsync($"{Endpoint}/users/byEmail?email={userRequest.Email}");
        userResponse.IsSuccessStatusCode.Should().BeTrue();
        var userData = await userResponse.Content.ReadFromJsonAsync<UserWithRolesResponse>(JsonTestExtensions.JsonSerializerOptions);

        var addRoleResponse = await client.PostAsync($"{Endpoint}/users/{userData.Id}/addRole", CreateContent(new { roleName = role.ToString() }));
        addRoleResponse.IsSuccessStatusCode.Should().BeTrue();

        return await GetToken(client, role);
    }

    public static async Task<string> GetToken(HttpClient client, EnRole role, string password = Password)
    {
        var loginResponse = await client.PostAsync("login", CreateContent(new
        {
            Email = $"{role}@te.st",
            Password = password
        }));
        loginResponse.IsSuccessStatusCode.Should().BeTrue();
        var loginData = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(JsonTestExtensions.JsonSerializerOptions);

        return loginData.AccessToken;
    }
}