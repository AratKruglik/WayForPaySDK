namespace WayForPaySDK.Handlers;

/// <summary>
/// Handler for processing WayForPay webhook callbacks.
/// Provides methods for parsing, validating, and responding to payment notifications.
/// </summary>
public interface IWebhookHandler
{
    /// <summary>
    /// Asynchronously parses and validates webhook payload from a Stream.
    /// </summary>
    /// <param name="body">Stream containing the webhook JSON payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Validated webhook payload.</returns>
    /// <exception cref="WayForPaySDK.Exceptions.SignatureException">Thrown when the webhook signature is invalid.</exception>
    /// <exception cref="System.Text.Json.JsonException">Thrown when the JSON is malformed.</exception>
    Task<WebhookPayload> ParseAsync(Stream body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Parses and validates webhook payload from a JSON string.
    /// </summary>
    /// <param name="json">JSON string containing the webhook payload.</param>
    /// <returns>Validated webhook payload.</returns>
    /// <exception cref="WayForPaySDK.Exceptions.SignatureException">Thrown when the webhook signature is invalid.</exception>
    /// <exception cref="System.Text.Json.JsonException">Thrown when the JSON is malformed.</exception>
    /// <exception cref="System.ArgumentException">Thrown when json is null or whitespace.</exception>
    WebhookPayload Parse(string json);

    /// <summary>
    /// Creates a signed response to acknowledge webhook receipt.
    /// </summary>
    /// <param name="payload">The received webhook payload.</param>
    /// <param name="status">Processing status (Accept or Decline). Defaults to Accept.</param>
    /// <returns>Signed webhook response ready to be returned to WayForPay.</returns>
    WebhookResponse CreateResponse(WebhookPayload payload, WebhookStatus status = WebhookStatus.Accept);

    /// <summary>
    /// Serializes webhook response to JSON string.
    /// </summary>
    /// <param name="response">The webhook response to serialize.</param>
    /// <returns>JSON string ready to be sent to WayForPay.</returns>
    string SerializeResponse(WebhookResponse response);
}
