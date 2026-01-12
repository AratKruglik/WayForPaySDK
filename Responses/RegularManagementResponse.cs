using System.Text.Json.Serialization;

namespace WayForPaySDK.Responses;

public sealed class RegularManagementResponse
{
    [JsonPropertyName("orderReference")]
    public string? OrderReference { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("time")]
    public long? Time { get; init; }

    [JsonPropertyName("reasonCode")]
    public int? ReasonCode { get; init; }

    [JsonPropertyName("reason")]
    public string? Reason { get; init; }
    
    [JsonIgnore]
    public bool IsSuccess => Status == "accept" || ReasonCode == 1100; // Check standard success indicators
}
