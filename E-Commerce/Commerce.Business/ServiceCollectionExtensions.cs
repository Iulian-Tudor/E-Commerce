using Microsoft.Extensions.DependencyInjection;

namespace Commerce.Business;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusiness(this IServiceCollection services)
    {
        return services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(BusinessAssembly).Assembly));
    }

    private static class BusinessAssembly { }
}