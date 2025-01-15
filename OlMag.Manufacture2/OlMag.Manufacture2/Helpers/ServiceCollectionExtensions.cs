using OlMag.Manufacture2.Interfaces;
using OlMag.Manufacture2.Services.SalesManagerRepositories;
using OlMag.Manufacture2.Services;
using Microsoft.EntityFrameworkCore;
using OlMag.Manufacture2.Data;
using Microsoft.EntityFrameworkCore.Migrations;
using OlMag.Manufacture2.Controllers;

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

    public static IServiceCollection AddDb(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<SalesManagementContext>(opts => { opts.UseNpgsql(connectionString); });


        services.AddDbContext<DataContext>(options =>
        {
            options.UseNpgsql(connectionString,
                o => o.MigrationsHistoryTable(
                    tableName: HistoryRepository.DefaultTableName, schema: "public"));
        });
        services.AddDbContext<SalesManagementContext>(options =>
        {
            options.UseNpgsql(connectionString,
                o => o.MigrationsHistoryTable(
                    tableName: HistoryRepository.DefaultTableName, schema: SalesManagementContext.Schema));
        });

        return services;
    }

    public static IHost UseDb(this IHost host)
    {
        try
        {
            using var scope = host.Services.CreateScope();
            var salesManagementContext = scope.ServiceProvider.GetRequiredService<SalesManagementContext>();
            salesManagementContext.Database.Migrate();
            /*var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            dataContext.Database.Migrate();*/
        }
        catch (Exception ex)
        {
            var logger = host.Services.GetRequiredService<ILogger<UserManagementController>>();
            logger.LogError(ex, "Failed run db migration. Error: {error}", ex.Message);
        }

        return host;
    }
}