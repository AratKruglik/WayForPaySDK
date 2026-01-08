namespace WayForPaySDK.Http;

/// <summary>
/// Interface for WayForPay HTTP client.
/// </summary>
public interface IWayForPayHttpClient
{
    /// <summary>
    /// Sends a POST request to the WayForPay API.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body.</typeparam>
    /// <typeparam name="TResponse">The type of the response body.</typeparam>
    /// <param name="request">The request body.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response.</returns>
    Task<TResponse> PostAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class
        where TResponse : class;

    /// <summary>
    /// Sends a POST request with raw JSON content to the WayForPay API.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response body.</typeparam>
    /// <param name="jsonContent">The JSON content to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response.</returns>
    Task<TResponse> PostJsonAsync<TResponse>(string jsonContent, CancellationToken cancellationToken = default)
        where TResponse : class;
}
