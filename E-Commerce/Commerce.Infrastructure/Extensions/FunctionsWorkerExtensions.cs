using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Commerce.Infrastructure;

public static class FunctionsWorkerExtensions
{
    public static IFunctionsWorkerApplicationBuilder UseCorsOptionsMiddleware(this IFunctionsWorkerApplicationBuilder builder)
    {
        return builder.UseHttpMiddleware<CorsOptionsMiddleware>();
    }

    public static IFunctionsWorkerApplicationBuilder UseJwtAuthMiddleware(this IFunctionsWorkerApplicationBuilder builder)
    {
        return builder.UseHttpMiddleware<HttpJwtAuthMiddleware>();
    }

    public static IFunctionsWorkerApplicationBuilder UseExecutingFunctionMiddleware<T>(this IFunctionsWorkerApplicationBuilder builder)
    {
        builder.Services.TryAddScoped<ExecutingFunctionContext>();
        builder.Services.TryAddScoped<IExecutingFunctionContext>(sp => sp.GetRequiredService<ExecutingFunctionContext>());

        return builder.UseMiddleware<ExecutingFunctionContextMiddleware<T>>();
    }

    private static IFunctionsWorkerApplicationBuilder UseHttpMiddleware<T>(this IFunctionsWorkerApplicationBuilder builder) where T : class, IFunctionsWorkerMiddleware
    {
        return builder.UseWhen<T>(context => context.FunctionDefinition.InputBindings.Values.First(a => a.Type.EndsWith("Trigger")).Type == "httpTrigger");
    }
}