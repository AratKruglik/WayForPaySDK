using System.Net.Http.Json;
using System.Text.Json;
using WayForPaySDK.Exceptions;

namespace WayForPaySDK.Http;

internal static class ApiRequestSender
{
    internal static async Task<TResponse> SendAsync<TRequest, TResponse>(
        HttpClient httpClient,
        Uri? url,
        TRequest request,
        JsonSerializerOptions jsonOptions,
        CancellationToken cancellationToken)
        where TRequest : class
        where TResponse : class
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(
                url,
                request,
                jsonOptions,
                cancellationToken);

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(
                    $"HTTP request failed with status {(int)response.StatusCode}.");
            }

            var result = JsonSerializer.Deserialize<TResponse>(content, jsonOptions);

            if (result is null)
            {
                throw new JsonParseException("Response deserialized to null.");
            }

            return result;
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
