using Newtonsoft.Json;
using Commerce.Business;
using Azure.Storage.Blobs;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Configuration;
using SendGrid.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Commerce.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services
            .AddNewtonsoftSerializationOptions()
            .AddRepositories()
            .AddAzureBlobStorage()
            .AddSendGridEmailService()
            .AddJwtConfiguration()
            .AddJwtUser();

        return services;
    }

    public static IServiceCollection AddEfDbContext(this IServiceCollection services)
    {
        services.AddDbContext<EfDbContext>();

        var isIntegrationTestEnvironment = Environment.GetEnvironmentVariable("IS_INTEGRATION_TEST_ENVIRONMENT");

        if (isIntegrationTestEnvironment is null or "" or "false")
        {
            return services;
        }

        //var sp = services.BuildServiceProvider();
        //var dbContext = sp.GetRequiredService<EfDbContext>();
        //dbContext.Database.Migrate();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IOrderRepository, OrderRepository>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IOrderedProductRepository, OrderedProductRepository>()
            .AddScoped<IFavoriteProductSnapshotRepository, FavoriteProductSnapshotRepository>()
            .AddScoped<ICategoryRepository, CategoryRepository>()
            .AddScoped<IUserGateRepository, UserGateRepository>();

        return services;
    }

    private static IServiceCollection AddNewtonsoftSerializationOptions(this IServiceCollection services)
    {
        services
            .AddOptions<JsonSerializerSettings>()
            .Configure(options => options.Defaults());

        return services;
    }

    public static IServiceCollection AddAzureBlobStorage(this IServiceCollection services)
    {
        return services
            .AddSingleton(x => new BlobServiceClient(CommerceEnvironment.IsDevDockerEnvironment ? "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://host.docker.internal:10000/devstoreaccount1;QueueEndpoint=http://host.docker.internal:10001/devstoreaccount1;TableEndpoint=http://host.docker.internal:10002/devstoreaccount1;" : "UseDevelopmentStorage=true"))
            .AddScoped<IBlobStorage, AzureBlobStorage>();
    }

    public static IServiceCollection AddSendGridEmailService(this IServiceCollection services)
    {
        return services
            .AddSingletonConfiguration<SendGridConfiguration>(SendGridConfiguration.SectionName)
            .AddSendGrid((_, sgo) => sgo.ApiKey = "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")
            .Services
            .AddScoped<IEmailService, SendGridEmailService>();
    }

    public static IServiceCollection AddJwtConfiguration(this IServiceCollection services)
    {
        return services
            .AddScoped<IJwtTokenRepository, JwtTokenRepository>()
            .AddScoped<ITokenService, JwtTokenService>()
            .AddSingleton(p =>
            {
                var config = p.GetRequiredService<IConfiguration>();
                return config
                    .GetSection(JwtConfiguration.SectionName)
                    .Get<JwtConfiguration>(o => o.BindNonPublicProperties = true);
            });
    }

    public static IServiceCollection AddJwtUser(this IServiceCollection services)
    {
        return services.AddScoped<IIdentifiedUserAccessor, IdentifiedUserAccessor>()
            .AddScoped(sp =>
            {
                var accessor = sp.GetRequiredService<IIdentifiedUserAccessor>();
                var user = accessor.User;

                return user.GetValueOrThrow("No user set on IdentifiedUserAccessor");
            });
    }

    private static JsonSerializerSettings Defaults(this JsonSerializerSettings settings)
    {
        return settings.EnumAsStrings().CamelCase();
    }

    private static JsonSerializerSettings EnumAsStrings(this JsonSerializerSettings settings)
    {
        settings.Converters.Add(new StringEnumConverter());
        return settings;
    }

    private static JsonSerializerSettings CamelCase(this JsonSerializerSettings settings)
    {
        settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        return settings;
    }

    private static IServiceCollection AddSingletonConfiguration<T>(this IServiceCollection services, string sectionKey) where T : class
    {
        return services.AddSingleton(p => p.GetService<IConfiguration>().GetConfiguration<T>(sectionKey));
    }

    private static T GetConfiguration<T>(this IConfiguration configuration, string sectionKey = null)
    {
        sectionKey ??= typeof(T).Name;
        return configuration
            .GetSection(sectionKey)
            .Get<T>(o => o.BindNonPublicProperties = true);
    }
}