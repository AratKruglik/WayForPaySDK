namespace WayForPaySDK.Http;

public interface IWayForPayHttpClient
{
    Task<TResponse> PostAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class
        where TResponse : class;

    Task<TResponse> PostJsonAsync<TResponse>(string jsonContent, CancellationToken cancellationToken = default)
        where TResponse : class;
}
