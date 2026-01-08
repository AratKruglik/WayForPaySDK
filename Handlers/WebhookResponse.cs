using System.Text.Json.Serialization;

namespace WayForPaySDK.Handlers;

/// <summary>
/// Response to WayForPay webhook callback.
/// Must be returned to confirm webhook receipt.
/// </summary>
public sealed record WebhookResponse
{
    /// <summary>
    /// Gets or sets the order reference from the webhook payload.
    /// </summary>
    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; init; }

    /// <summary>
    /// Gets or sets the processing status ("accept" or "decline").
    /// </summary>
    [JsonPropertyName("status")]
    public required string Status { get; init; }

    /// <summary>
    /// Gets or sets the processing time as Unix timestamp.
    /// </summary>
    [JsonPropertyName("time")]
    public required long Time { get; init; }

    /// <summary>
    /// Gets or sets the response signature (HMAC-MD5).
    /// Generated from: orderReference;status;time
    /// </summary>
    [JsonPropertyName("signature")]
    public required string Signature { get; init; }
}
