using System.Text.Json.Serialization;
using WayForPaySDK.Domain;

namespace WayForPaySDK.Responses;

public abstract class ApiResponse
{
    [JsonPropertyName("reasonCode")]
    public required int ReasonCode { get; init; }

    [JsonPropertyName("reason")]
    public required string ReasonMessage { get; init; }

    [JsonPropertyName("merchantSignature")]
    public string? MerchantSignature { get; init; }

    [JsonIgnore]
    public Reason Reason => new() { Code = ReasonCode, Message = ReasonMessage };

    [JsonIgnore]
    public bool IsSuccess => Reason.IsSuccess;

    public abstract IEnumerable<string> GetSignatureFields();
}
