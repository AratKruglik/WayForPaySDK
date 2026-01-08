using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using WayForPaySDK.Exceptions;
using WayForPaySDK.Serialization;

namespace WayForPaySDK.Http;

/// <summary>
/// HTTP client for WayForPay API communication.
/// </summary>
public sealed class WayForPayHttpClient : IWayForPayHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="WayForPayHttpClient"/> class.
    /// </summary>
    /// <param name="httpClient">The configured HTTP client.</param>
    public WayForPayHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions(WayForPayJsonContext.Default.Options);
    }

    /// <inheritdoc />
    public async Task<TResponse> PostAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : class
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                string.Empty,
                request,
                _jsonOptions,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TResponse>(
                _jsonOptions,
                cancellationToken);

            return result ?? throw new JsonParseException("Response deserialized to null.");
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException($"HTTP request failed: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new JsonParseException($"Failed to parse response: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<TResponse> PostJsonAsync<TResponse>(
        string jsonContent,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        ArgumentException.ThrowIfNullOrEmpty(jsonContent);

        try
        {
            using var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(string.Empty, content, cancellationToken);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            var result = JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);

            return result ?? throw new JsonParseException("Response deserialized to null.", responseContent);
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException($"HTTP request failed: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new JsonParseException($"Failed to parse response: {ex.Message}", ex);
        }
    }
}
