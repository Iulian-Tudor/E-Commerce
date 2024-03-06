using System.Net;
using Newtonsoft.Json;
using CSharpFunctionalExtensions;
using Microsoft.Azure.Functions.Worker.Http;

namespace Commerce.Functions.Isolated;

public static class HttpRequestExtensions
{
    public static T ParseQuery<T>(this HttpRequestData request) where T : new() => QueryString.Parse<T>(request.Url.Query);

    public static async Task<Result<T>> DeserializeBodyPayload<T>(this HttpRequestData request) where T : class
    {
        using var reader = new StreamReader(request.Body);
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings();
        var body = JsonConvert.DeserializeObject<T>(await reader.ReadToEndAsync());

        return Result.SuccessIf(body is not null, body!, "NULL_BODY_BAD_REQUEST");
    }

    public static async Task<T> DeserializeResponseBody<T>(this HttpResponseMessage response) where T : class
    {
        var body = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(body);
    }

    public static async Task<HttpResponseData> CreateJsonResponse<T>(this HttpRequestData httpRequest, T payload)
    {
        var response = httpRequest.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(payload);

        return response;
    }
}