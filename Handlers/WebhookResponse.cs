using System.Text.Json.Serialization;

namespace WayForPaySDK.Handlers;

public sealed record WebhookResponse
{
    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; init; }

    [JsonPropertyName("status")]
    public required string Status { get; init; }

    [JsonPropertyName("time")]
    public required long Time { get; init; }

    [JsonPropertyName("signature")]
    public required string Signature { get; init; }
}
