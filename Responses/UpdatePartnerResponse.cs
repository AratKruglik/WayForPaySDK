using System.Text.Json.Serialization;

namespace WayForPaySDK.Responses;

public sealed class UpdatePartnerResponse : MmsResponse
{
    [JsonPropertyName("partnerCode")]
    public string? PartnerCode { get; init; }

    [JsonPropertyName("secretKey")]
    public string? SecretKey { get; init; }
}
