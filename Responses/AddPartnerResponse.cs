using System.Text.Json.Serialization;

namespace WayForPaySDK.Responses;

public sealed class AddPartnerResponse : MmsResponse
{
    [JsonPropertyName("partnerCode")]
    public string? PartnerCode { get; init; }
}
