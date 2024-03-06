using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace Commerce.Infrastructure;

public static class HostEnvironmentExtensions
{
    public static bool IsCommerceDevelopment(this IHostEnvironment env) => env.IsDevelopment() || CommerceEnvironment.IsDevDockerEnvironment;
}

public static class CommerceEnvironment
{
    public static bool IsDevDockerEnvironment => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "DockerDev";
}

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureCommerceAppConfiguration(this IHostBuilder builder)
    {
        return builder.ConfigureAppConfiguration(b =>
        {
            b.AddCommerceAppSettings().AddEnvironmentVariables();
        });
    }

    private static IConfigurationBuilder AddCommerceAppSettings(this IConfigurationBuilder builder)
    {
        builder.AddJsonFile("appsettings.json", false);

        if (CommerceEnvironment.IsDevDockerEnvironment)
        {
            builder.AddJsonFile("appsettings.DockerDev.json", true);
        }

        builder.AddJsonFile("appsettings.Development.json", true);
        builder.AddJsonFile("appsettings.private.json", true);

        return builder;
    }
}