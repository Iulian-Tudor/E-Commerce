namespace Commerce.Client;

public interface IHttpClient
{
    Task<T?> Get<T>(string url) where T : class;

    Task<Stream> GetStream(string url);

    Task<ApiResult> Post<TRequest>(string url, TRequest? model = null) where TRequest : class;

    Task<ApiResult<TResponse>> Post<TRequest, TResponse>(string url, TRequest? model = null) where TRequest : class where TResponse : class;

    Task<ApiResult> PostStream(string url, Stream stream, string fileName, string extension);
    
    Task<ApiResult> Put<T>(string url, T? model = null) where T : class;

    Task<ApiResult<TResponse>> Put<TRequest, TResponse>(string url, TRequest? model = null) where TRequest : class where TResponse : class;

    Task<ApiResult> Patch(string url);

    Task<ApiResult> Patch<T>(string url, T? model = null) where T : class;

    Task<ApiResult<TResponse>> Patch<TRequest, TResponse>(string url, TRequest? model = null) where TRequest : class where TResponse : class;

    Task<ApiResult> Delete(string url);

    Task<ApiResult<TResponse>> Delete<TResponse>(string url) where TResponse : class;
}