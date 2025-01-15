using Mapster;
using MapsterMapper;
using OlMag.Manufacture2.Models.Entities.SalesManager;
using OlMag.Manufacture2.Models.Requests.SalesManager;
using OlMag.Manufacture2.Models.Responses.SalesManager;

namespace OlMag.Manufacture2.Helpers;

public static class MapsterConfig
{
    public static IServiceCollection RegisterMapsterConfiguration(this IServiceCollection services)
    {
        var config = GetMapsterConfig();
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        return services;
    }

    public static TypeAdapterConfig GetMapsterConfig()
    {
        var config = new TypeAdapterConfig();
        config.Compile();

        CustomerConfig(config);
        ContactPersonConfig(config);
        return config;
    }

    private static void CustomerConfig(TypeAdapterConfig config)
    {
        config
            .NewConfig<CustomerBodyRequest, CustomerEntity>()
            .Map(dest => dest, src => src)
            .Map(dest => dest.Information, src => src)
            ;

        config
            .NewConfig<(Guid id, CustomerBodyRequest body), CustomerEntity>()
            .Map(dest => dest.Id, src => src.id)
            .Map(dest => dest, src => src.body)
            .Map(dest => dest.Information, src => src.body)
            ;

        config.NewConfig<CustomerEntity, CustomerResponse>()
            .Map(dest => dest, src => src)
            .Map(dest => dest, src => src.Information)
            ;

        config.NewConfig<CustomerEntity, CustomerInfoResponse>()
            .Map(dest => dest, src => src)
            .Map(dest => dest, src => src.Information)
            ;
    }

    private static void ContactPersonConfig(TypeAdapterConfig config)
    {
        config
            .NewConfig<ContactPersonBodyRequest, ContactPersonEntity>()
            .Map(dest => dest, src => src)
            .Map(dest => dest.Information, src => src)
            ;

        config
            .NewConfig<(Guid id, ContactPersonBodyRequest body), ContactPersonEntity>()
            .Map(dest => dest.Id, src => src.id)
            .Map(dest => dest, src => src.body)
            .Map(dest => dest.Information, src => src.body)
            ;

        config.NewConfig<ContactPersonEntity, ContactPersonResponse>()
            .Map(dest => dest, src => src)
            .Map(dest => dest, src => src.Information)
            ;

        config.NewConfig<ContactPersonEntity, ContactPersonInfoResponse>()
            .Map(dest => dest, src => src)
            .Map(dest => dest, src => src.Information)
            ;
    }
}