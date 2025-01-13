using OlMag.Manufacture2.Interfaces;
using OlMag.Manufacture2.Services.SalesManagerRepositories;
using OlMag.Manufacture2.Services;

namespace OlMag.Manufacture2.Helpers;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add API BL services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddMemoryCache();

        services.AddTransient<ISalesManagerService, SalesManagerService>();
        services.AddTransient<CustomerRepository>();
        services.AddTransient<ContactPersonRepository>();

        return services;
    }
}