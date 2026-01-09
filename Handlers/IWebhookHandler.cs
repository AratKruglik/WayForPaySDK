namespace WayForPaySDK.Handlers;

/// <summary>
/// Handler for processing WayForPay webhook callbacks.
/// Provides methods for parsing, validating, and responding to payment notifications.
/// </summary>
public interface IWebhookHandler
{
    /// <summary>
    /// Parses and validates webhook payload from a Stream.
    /// </summary>
    /// <exception cref="WayForPaySDK.Exceptions.SignatureException">Thrown when the webhook signature is invalid.</exception>
    /// <exception cref="System.Text.Json.JsonException">Thrown when the JSON is malformed.</exception>
    Task<WebhookPayload> ParseAsync(Stream body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Parses and validates webhook payload from a JSON string.
    /// </summary>
    /// <exception cref="WayForPaySDK.Exceptions.SignatureException">Thrown when the webhook signature is invalid.</exception>
    /// <exception cref="System.Text.Json.JsonException">Thrown when the JSON is malformed.</exception>
    /// <exception cref="System.ArgumentException">Thrown when json is null or whitespace.</exception>
    WebhookPayload Parse(string json);

    /// <summary>
    /// Creates a signed response to acknowledge webhook receipt.
    /// </summary>
    WebhookResponse CreateResponse(WebhookPayload payload, WebhookStatus status = WebhookStatus.Accept);

    /// <summary>
    /// Serializes webhook response to JSON string.
    /// </summary>
    string SerializeResponse(WebhookResponse response);
}
