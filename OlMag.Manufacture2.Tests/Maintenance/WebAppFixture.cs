using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace OlMag.Manufacture2.Tests.Maintenance;

public class WebAppFixture : IAsyncLifetime
{
    private WebApplicationFactory<Program> application = default!;
    internal TestDatabaseFixture? DatabaseFixture { get; private set; }
    internal WebApplicationFactory<Program> Application => application;

    public Task InitializeAsync()
    {
        application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(_ =>
            {
                DatabaseFixture = new TestDatabaseFixture();
            });
            builder.ConfigureTestServices(services =>
            {
                services
                    .AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthHandler.AuthSchemeName;
                        options.DefaultScheme = TestAuthHandler.AuthSchemeName;
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AuthSchemeName,
                        _ => { });
            });
        });
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await application.DisposeAsync();
    }
}