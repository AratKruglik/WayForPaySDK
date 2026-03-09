using System.Text.Json.Serialization;

namespace WayForPaySDK.Responses;

public abstract class MmsResponse
{
    [JsonPropertyName("reasonCode")]
    public required int ReasonCode { get; init; }

    [JsonPropertyName("reason")]
    public required string ReasonMessage { get; init; }

    [JsonIgnore]
    public bool IsSuccess => ReasonCode == 1100;
}
