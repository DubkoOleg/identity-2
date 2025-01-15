using System.Net;
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
    
    [Fact]
    public async Task UserManagement_full_test()
    {
        // Создать пользователя
        // Залогиниться
        // Проверить, что доступ к хелсчеку есть, к хелсчеку с авторизацией нет
        // Дать роль администратора пользователей
        // Проверить доступ к хелсчеку с авторизацией
        // Получить пользователя текущего краткую информацию
        // Получить пользователя текущего полную информацию
        // Получить пользователя по почте
        // Получить всех пользователей
        // Удалить роль
        // Проверить доступ к хелсчеку с авторизацией

        var userRequest = new
        {
            Email = "test@te.st",
            Password = "gEg3%gsetEW"
        };
        var registerResponse = await client.PostAsync("register", CreateContent(userRequest));
        registerResponse.IsSuccessStatusCode.Should().BeTrue();

        var loginData = await Login(userRequest.Email, userRequest.Password);
        client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {loginData.AccessToken}");

        var healthCheckResponse = await client.GetAsync($"{Endpoint}/healthcheck");
        healthCheckResponse.IsSuccessStatusCode.Should().BeTrue();

        var healthCheckAuthResponse = await client.GetAsync($"{Endpoint}/healthcheckauth");
        healthCheckAuthResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var userShortResponse = await client.GetAsync("User/shortInfo");
        userShortResponse.IsSuccessStatusCode.Should().BeTrue();
        var userShortData = await userShortResponse.Content.ReadFromJsonAsync<UserShortInfoResponse>(JsonTestExtensions.JsonSerializerOptions);
        userShortData.Email.Should().Be(userRequest.Email);

        var userAdministratorToken = await GetToken(client, EnRole.UserAdministrator, UserAdministratorPassword);
        client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {userAdministratorToken}");

        var addRoleResponse = await client.PostAsync($"{Endpoint}/users/{userShortData.Id}/addRole",
            CreateContent(new { roleName = EnRole.UserAdministrator.ToString() }));
        addRoleResponse.IsSuccessStatusCode.Should().BeTrue();

        client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {loginData.AccessToken}");
        
        var healthCheckAuthResponse2 = await client.GetAsync($"{Endpoint}/healthcheckauth");
        healthCheckAuthResponse2.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var loginRefreshData = await RefreshToken(loginData.RefreshToken);
        client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {loginRefreshData.AccessToken}");

        var healthCheckAuthResponse3 = await client.GetAsync($"{Endpoint}/healthcheckauth");
        healthCheckAuthResponse3.IsSuccessStatusCode.Should().BeTrue();

        var userCurrentResponse = await client.GetAsync("User/current");
        userCurrentResponse.IsSuccessStatusCode.Should().BeTrue();
        var userCurrentData = await userCurrentResponse.Content.ReadFromJsonAsync<UserWithRolesResponse>(JsonTestExtensions.JsonSerializerOptions);
        userCurrentData.Id.Should().Be(userShortData.Id);
        userCurrentData.Email.Should().Be(userShortData.Email);
        userCurrentData.Name.Should().Be(userShortData.Name);
        userCurrentData.Roles.Should().BeEquivalentTo([EnRole.UserAdministrator.ToString()]);

        var addRoleResponse2 = await client.PostAsync($"{Endpoint}/users/{userCurrentData.Id}/addRole",
            CreateContent(new { roleName = EnRole.MasterOfLaser.ToString() }));
        addRoleResponse2.IsSuccessStatusCode.Should().BeTrue();

        var userResponse = await client.GetAsync($"{Endpoint}/users/byEmail?email={userRequest.Email}");
        userResponse.IsSuccessStatusCode.Should().BeTrue();
        var userData = await userResponse.Content.ReadFromJsonAsync<UserWithRolesResponse>(JsonTestExtensions.JsonSerializerOptions);
        userData.Id.Should().Be(userShortData.Id);
        userData.Email.Should().Be(userShortData.Email);
        userData.Name.Should().Be(userShortData.Name);
        userData.Roles.Should().BeEquivalentTo([EnRole.UserAdministrator.ToString(), EnRole.MasterOfLaser.ToString()]);
        
        var usersResponse = await client.GetAsync($"{Endpoint}/users/all");
        usersResponse.IsSuccessStatusCode.Should().BeTrue();
        var usersData = await usersResponse.Content.ReadFromJsonAsync<UserWithRolesResponse[]>(JsonTestExtensions.JsonSerializerOptions);
        usersData.Length.Should().Be(2);

        var rolesResponse = await client.GetAsync($"{Endpoint}/roles/all");
        rolesResponse.IsSuccessStatusCode.Should().BeTrue();
        var rolesData = await rolesResponse.Content.ReadFromJsonAsync<string[]>(JsonTestExtensions.JsonSerializerOptions);
        rolesData.Length.Should().Be(Enum.GetValues<EnRole>().Length);

        var deleteRoleResponse = await client.DeleteAsync($"{Endpoint}/users/{userCurrentData.Id}/deleteRole/{EnRole.UserAdministrator}");
        deleteRoleResponse.IsSuccessStatusCode.Should().BeTrue();

        var healthCheckAuthResponse4 = await client.GetAsync($"{Endpoint}/healthcheckauth");
        healthCheckAuthResponse4.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginRefreshData2 = await RefreshToken(loginData.RefreshToken);
        client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {loginRefreshData2.AccessToken}");

        var healthCheckAuthResponse5 = await client.GetAsync($"{Endpoint}/healthcheckauth");
        healthCheckAuthResponse5.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    public async Task<LoginResponse> Login(string email, string password)
    {
        var loginResponse = await client.PostAsync("login", CreateContent(new
        {
            Email = email,
            Password = password
        }));
        loginResponse.IsSuccessStatusCode.Should().BeTrue();
        var loginData = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(JsonTestExtensions.JsonSerializerOptions);
        loginData.Should().NotBeNull();
        loginData!.TokenType.Should().Be("Bearer");
        loginData!.AccessToken.Should().NotBeEmpty();
        loginData!.ExpiresIn.Should().Be(3600);
        loginData!.RefreshToken.Should().NotBeEmpty();

        return loginData;
    }

    public async Task<LoginResponse> RefreshToken(string refreshToken)
    {
        var loginResponse = await client.PostAsync("refresh", CreateContent(new { refreshToken }));
        loginResponse.IsSuccessStatusCode.Should().BeTrue();
        var loginData = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(JsonTestExtensions.JsonSerializerOptions);
        loginData.Should().NotBeNull();
        loginData!.TokenType.Should().Be("Bearer");
        loginData!.AccessToken.Should().NotBeEmpty();
        loginData!.ExpiresIn.Should().Be(3600);
        loginData!.RefreshToken.Should().NotBeEmpty();

        return loginData;
    }
}