using Commerce.Business;
using Commerce.Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(wa =>
    {
        wa.UseCorsOptionsMiddleware();
        wa.UseExecutingFunctionMiddleware<Program>();
        wa.UseJwtAuthMiddleware();
    })
    .ConfigureCommerceAppConfiguration()
    .ConfigureServices()
    .Build();

host.Run();

static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureServices(this IHostBuilder builder)
    {
        return builder.ConfigureServices((_, services) => services
            .AddLogging(b => b.AddSimpleConsole())
            .AddEfDbContext()
            .AddInfrastructure()
            .AddBusiness()
        );
    }
}
