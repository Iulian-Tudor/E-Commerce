using Xunit;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Configurations;

namespace Commerce.Functions.Isolated.Tests.Integration;

public sealed class AzureFunctionsTestContainersFixture : IAsyncLifetime
{
    private IFutureDockerImage dockerImage;

    public IContainer AzureFunctionsContainerInstance { get; private set; }

    public async Task InitializeAsync()
    {
        TestcontainersSettings.Logger = new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider().CreateLogger("test");
        dockerImage = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
            .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D"))
            .Build();

        await dockerImage.CreateAsync().ConfigureAwait(false);

        AzureFunctionsContainerInstance = new ContainerBuilder()
            .WithEnvironment("IS_INTEGRATION_TEST_ENVIRONMENT", "true")
            .WithEnvironment("AZURE_FUNCTIONS_ENVIRONMENT", "Development")
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "DockerDev")
            .WithEnvironment("FUNCTIONS_WORKER_RUNTIME", "dotnet-isolated")
            .WithImage(dockerImage)
            .WithPortBinding(80, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(80)))
            .Build();
        await AzureFunctionsContainerInstance.StartAsync();

        await this.SeedDatabase();
    }

    public async Task DisposeAsync()
    {
        await AzureFunctionsContainerInstance.StopAsync();
        await AzureFunctionsContainerInstance.DisposeAsync();
        await dockerImage.DeleteAsync();
    }
}

public static class AzureFunctionsTestContainersFixtureExtensions
{
    public static string Route(this AzureFunctionsTestContainersFixture fixture, string path)
        => new UriBuilder(
            Uri.UriSchemeHttp,
            fixture.AzureFunctionsContainerInstance.Hostname,
            fixture.AzureFunctionsContainerInstance.GetMappedPublicPort(80),
            path
        ).ToString();
}