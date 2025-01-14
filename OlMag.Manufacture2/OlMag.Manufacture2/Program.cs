using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.OpenApi.Models;
using OlMag.Manufacture2.Data;
using OlMag.Manufacture2.Helpers;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;
using Swashbuckle.AspNetCore.Filters;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up!");

try
{
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;

    // Add services to the container.

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("metrics"))));

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });

        options.OperationFilter<SecurityRequirementsOperationFilter>();
    });

    builder.Services.AddDb(configuration.GetConnectionString("IdentityConnection")!);

    builder.Services.AddAuthentication();

    builder.Services.AddIdentityApiEndpoints<IdentityUser>()
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<DataContext>();
    /*builder.Services.AddIdentityApiEndpoints<IdentityUser>()
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<DataContext>()
        .AddSignInManager()
        .AddRoleManager<RoleManager<IdentityRole>>()
        .AddDefaultTokenProviders();*/

    builder.Services.AddApiServices(builder.Configuration);

    builder.Services.RegisterMapsterConfiguration();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapIdentityApi<IdentityUser>();

    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.UseDb();
    app.Run();
    return 0;
}
catch (Exception ex) when (ex is not HostAbortedException && ex.Source != "Microsoft.EntityFrameworkCore.Design")
{
    Log.Fatal(ex, "An unhandled exception occurred during bootstrapping");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program;