using System.Text.Json;
using Commerce.Client.Extensions;

namespace Commerce.Client;

public sealed class ApiResult<T> : ApiResult where T : class
{
    public new static async Task<ApiResult<T>?> FromResponse(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode switch
        {
            true when responseContent.IsNullOrEmpty() => new()
            {
                ErrorCode = null, IsSuccess = true, IsFailure = false, Value = null
            },
            false when responseContent.IsNullOrEmpty() => new()
            {
                ErrorCode = response.StatusCode.ToString(), IsSuccess = false, IsFailure = true, Value = null
            },
            _ => JsonSerializer.Deserialize<ApiResult<T>>(responseContent)
        };
    }

    public new T? Value { get; set; }
}

public class ApiResult
{
    public static async Task<ApiResult?> FromResponse(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode switch
        {
            true when responseContent.IsNullOrEmpty() => new()
            {
                ErrorCode = null, IsSuccess = true, IsFailure = false, Value = null
            },
            false when responseContent.IsNullOrEmpty() => new()
            {
                ErrorCode = response.StatusCode.ToString(), IsSuccess = false, IsFailure = true, Value = null
            },
            _ => JsonSerializer.Deserialize<ApiResult>(responseContent)
        };
    }

    public string? ErrorCode { get; set; }

    public bool IsSuccess { get; set; }

    public bool IsFailure { get; set; }

    public object? Value { get; set; }
}