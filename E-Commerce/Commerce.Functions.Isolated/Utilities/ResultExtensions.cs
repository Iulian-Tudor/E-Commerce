using System.Net;
using CSharpFunctionalExtensions;
using Microsoft.Azure.Functions.Worker.Http;

namespace Commerce.Functions.Isolated;

public static class ResultExtensions
{
    public static async Task<HttpResponseData> ToResponseData<T>(this Task<Result<T>> resultTask, HttpRequestData request, Action<HttpResponseData, ApiResult> onOk)
    {
        var response = request.CreateResponse();

        var apiResult = ApiResult.From(await resultTask);
        if (apiResult.IsSuccess)
        {
            onOk(response, apiResult);
        }
        else
        {
            await response.WriteAsJsonAsync(apiResult, HttpStatusCode.BadRequest);
        }
        return response;
    }

    public static async Task<HttpResponseData> ToResponseData(this Result result, HttpRequestData request, Action<HttpResponseData, ApiResult> onOk = null)
    {
        var response = request.CreateResponse(HttpStatusCode.NoContent);

        var apiResult = ApiResult.From(result);
        if (apiResult.IsSuccess)
        {
            onOk?.Invoke(response, apiResult);
        }
        else
        {
            await response.WriteAsJsonAsync(apiResult, HttpStatusCode.BadRequest);
        }
        return response;
    }

    public static async Task<HttpResponseData> ToResponseData(this Task<Result> resultTask, HttpRequestData request, Action<HttpResponseData, ApiResult> onOk = null)
        => await (await resultTask).ToResponseData(request, onOk);

    public static async Task<HttpResponseData> ToResponseData<T>(this Task<Maybe<T>> maybeTask, HttpRequestData request, Func<T, object> mapper = null)
    {
        var maybe = await maybeTask;
        return await maybe.ToResponseData(request, mapper);
    }

    public static async Task<HttpResponseData> ToResponseData<T>(this Maybe<T> maybe, HttpRequestData request, Func<T, object> mapper = null)
    {
        if (maybe.HasNoValue)
        {
            return request.CreateResponse(HttpStatusCode.NotFound);
        }

        object payload = maybe.Value;
        if (mapper != null)
        {
            payload = mapper(maybe.Value);
        }

        return await request.CreateJsonResponse(payload);
    }
}

public sealed class ApiResult
{
    private ApiResult() { }

    private ApiResult(bool isFailure, bool isSuccess, string errorCode, object value = null) : this()
    {
        IsFailure = isFailure;
        IsSuccess = isSuccess;
        ErrorCode = errorCode;
        Value = value;
    }

    internal static ApiResult From(Result result) => new(result.IsFailure, result.IsSuccess, result.IsFailure ? result.Error : string.Empty);

    internal static ApiResult From<T>(Result<T> result) => new(result.IsFailure, result.IsSuccess, result.IsFailure ? result.Error : string.Empty, result.IsSuccess ? (object)result.Value : null);

    public bool IsFailure { get; set; }

    public bool IsSuccess { get; set; }

    public string ErrorCode { get; set; }

    public object Value { get; set; }
}