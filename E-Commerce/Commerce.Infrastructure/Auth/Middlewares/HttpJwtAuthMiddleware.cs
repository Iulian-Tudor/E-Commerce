using System.Net;
using System.Reflection;
using Commerce.Business;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace Commerce.Infrastructure;

internal sealed class HttpJwtAuthMiddleware : IFunctionsWorkerMiddleware
{
    private const string BearerScheme = "Bearer";
    private const string AuthorizationHeaderKey = "Authorization";

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var httpRequest = await context.GetHttpRequestDataAsync();
        var executingFunction = context.InstanceServices.GetRequiredService<IExecutingFunctionContext>();
        var authRequirements = executingFunction.FunctionMethod.GetCustomAttribute<FunctionRequiresAuth>();

        var logger = context.InstanceServices.GetRequiredService<ILogger<HttpJwtAuthMiddleware>>();

        if (authRequirements is null)
        {
            logger.LogInformation("No auth requirements found for function {FunctionName}", executingFunction.FunctionMethod.Name);
            await next(context);
            return;
        }

        if (!httpRequest.Headers.Contains(AuthorizationHeaderKey))
        {
            logger.LogWarning("Expected request to contain Authorization header");
            WriteUnauthorizedResponse(httpRequest, context, false);
            return;
        }

        var authorizationValue = httpRequest.Headers.GetValues(AuthorizationHeaderKey).First();
        if (!authorizationValue.StartsWith(BearerScheme))
        {
            logger.LogWarning("Invalid Authorization scheme. Expected Bearer");
            WriteUnauthorizedResponse(httpRequest, context, false);
            return;
        }

        var token = authorizationValue.Split(BearerScheme).LastOrDefault()?.Trim();

        var jwtSettings = context.InstanceServices.GetRequiredService<JwtConfiguration>();
        var jwtHandler = new JwtSecurityTokenHandler();

        var validationParams = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = jwtSettings.SigningKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(10)
        };

        try
        {
            jwtHandler.ValidateToken(token, validationParams, out _);
            var userAccessor = context.InstanceServices.GetRequiredService<IIdentifiedUserAccessor>();
            userAccessor.Set(JwtTokenIdentifiedUser.From(jwtHandler.ReadJwtToken(token)));
        }
        catch (Exception e)
        {
            if (e is not SecurityTokenException)
            {
                logger.LogError(e, "Bad token detected. Exception type: {exceptionType}. Message: {exceptionMessage}", e.GetType().Name, e.Message);
            }

            var extraHeaders = new Dictionary<string, string>
            {
                {"X-Token-Validation-Error", e.GetType().Name}
            };
            WriteUnauthorizedResponse(httpRequest, context, true, extraHeaders);
            return;
        }

        await next(context);
    }

    private static void WriteUnauthorizedResponse(HttpRequestData httpRequest, FunctionContext context, bool refreshable, IDictionary<string, string> extraHeaders = null)
    {
        var response = httpRequest.CreateResponse(HttpStatusCode.Unauthorized);
        response.Headers.Add("Access-Control-Expose-Headers", "X-Token-Refreshable");
        response.Headers.Add("X-Token-Refreshable", refreshable.ToString().ToLower());

        if (extraHeaders != null)
        {
            foreach (var (key, value) in extraHeaders)
            {
                response.Headers.TryAddWithoutValidation(key, value);
            }
        }

        var keyValuePair = context.Features.SingleOrDefault(f => f.Key.Name == "IFunctionBindingsFeature");
        var functionBindingsFeature = keyValuePair.Value;
        var type = functionBindingsFeature.GetType();
        var result = type.GetProperties().Single(p => p.Name == "InvocationResult");
        result.SetValue(functionBindingsFeature, response);
    }
}