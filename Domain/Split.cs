using System.Text.Json.Serialization;

namespace WayForPaySDK.Domain;

public record Split
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("value")]
    public string? Value { get; init; }
}
