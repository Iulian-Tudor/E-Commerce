using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace Commerce.Infrastructure;

internal sealed class CorsOptionsMiddleware : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var httpRequest = await context.GetHttpRequestDataAsync();

        if (httpRequest.Method == "OPTIONS")
        {
            await next(context);
            return;
        }

        httpRequest.Headers.Add("Access-Control-Allow-Origin", "http://localhost:5167");
        httpRequest.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PATCH, OPTIONS, PUT, DELETE, OPTIONS");
        httpRequest.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

        await next(context);

        var httpResponse = context.GetHttpResponseData();
        httpResponse.Headers.Add("Access-Control-Allow-Origin", "http://localhost:5167");
        httpResponse.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PATCH, OPTIONS, PUT, DELETE, OPTIONS");
        httpResponse.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
    }
}