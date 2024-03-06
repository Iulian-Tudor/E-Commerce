using System.Net.Http.Json;
using System.Net.Http.Headers;
using Blazored.Toast.Services;

namespace Commerce.Client;

public sealed class HttpClientWrapper(HttpClient http, IAuthService authService, IToastService toastService) : IHttpClient
{
    public async Task<TResponse?> Get<TResponse>(string url) where TResponse : class
    {
        await http.WithAuthorization(authService);

        try
        {
            var response = await http.GetAsync(url);
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
        catch (Exception)
        {
            toastService.ShowError("An error occurred while processing the request.");
            return null;
        }
    }

    public async Task<Stream> GetStream(string url)
    {
        Console.WriteLine(url);

        var response = await http.GetAsync(url);
        return await response.Content.ReadAsStreamAsync();
    }

    public async Task<ApiResult> Post<TRequest>(string url, TRequest? model = null) where TRequest : class
    {
        await http.WithAuthorization(authService);

        var response = await http.PostAsJsonAsync(url, model);
        var result = await ApiResult.FromResponse(response);
        result.ShowNotification(toastService);

        return result;
    }

    public async Task<ApiResult<TResponse>> Post<TRequest, TResponse>(string url, TRequest? model = null) where TRequest : class where TResponse : class
    {
        await http.WithAuthorization(authService);

        var response = await http.PostAsJsonAsync(url, model);
        var result = await ApiResult<TResponse>.FromResponse(response);
        result.ShowNotification(toastService);

        return result;
    }

    public async Task<ApiResult> PostStream(string url, Stream stream, string fileName, string extension)
    {
        await http.WithAuthorization(authService);

        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
        var content = new MultipartFormDataContent();
        content.Add(new StreamContent(stream), "file", $"{fileName}.{extension}");

        var response = await http.PostAsync(url, content);
        var result = await ApiResult.FromResponse(response);
        result.ShowNotification(toastService);

        return result;
    }

    public async Task<ApiResult> Put<T>(string url, T? model = null) where T : class
    {
        await http.WithAuthorization(authService);

        var response = await http.PutAsJsonAsync(url, model);
        var result = await ApiResult.FromResponse(response);
        result.ShowNotification(toastService);

        return result;
    }

    public async Task<ApiResult<TResponse>> Put<TRequest, TResponse>(string url, TRequest? model = null) where TRequest : class where TResponse : class
    {
        await http.WithAuthorization(authService);

        var response = await http.PutAsJsonAsync(url, model);
        var result = await ApiResult<TResponse>.FromResponse(response);
        result.ShowNotification(toastService);

        return result;
    }

    public async Task<ApiResult> Patch(string url)
    {
        await http.WithAuthorization(authService);

        var response = await http.PatchAsync(url, null);
        var result = await ApiResult.FromResponse(response);
        result.ShowNotification(toastService);

        return result;
    }

    public async Task<ApiResult> Patch<T>(string url, T? model = null) where T : class
    {
        await http.WithAuthorization(authService);

        var response = await http.PatchAsJsonAsync(url, model);
        var result = await ApiResult.FromResponse(response);
        result.ShowNotification(toastService);

        return result;
    }

    public async Task<ApiResult<TResponse>> Patch<TRequest, TResponse>(string url, TRequest? model = null) where TRequest : class where TResponse : class
    {
        await http.WithAuthorization(authService);

        var response = await http.PatchAsJsonAsync(url, model);
        var result = await ApiResult<TResponse>.FromResponse(response);
        result.ShowNotification(toastService);

        return result;
    }

    public async Task<ApiResult> Delete(string url)
    {
        await http.WithAuthorization(authService);

        var response = await http.DeleteAsync(url);
        var result = await ApiResult.FromResponse(response);
        result.ShowNotification(toastService);

        return result;
    }

    public async Task<ApiResult<TResponse>> Delete<TResponse>(string url) where TResponse : class
    {
        await http.WithAuthorization(authService);

        var response = await http.DeleteAsync(url);
        var result = await ApiResult<TResponse>.FromResponse(response);
        result.ShowNotification(toastService);

        return result;
    }
}

public static class ApiResultExtensions
{
    public static void ShowNotification(this ApiResult? result, IToastService toastService)
    {
        if (result is null)
        {
            toastService.ShowError("An error occurred while processing the request.");
            return;
        }
        if (result.IsSuccess)
        {
            toastService.ShowSuccess("The operation has been completed successfully!");
        }
        else
        {
            toastService.ShowError(result.ErrorCode);
        }
    }
}

public static class HttpClientExtensions
{
    public static async Task WithAuthorization(this HttpClient http, IAuthService authService)
    {
        var token = await authService.GetToken();
        if (token.HasValue)
        {
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
        }
    }
}