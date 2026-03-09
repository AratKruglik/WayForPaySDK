using System.Text.Json.Serialization;

namespace WayForPaySDK.Responses;

public sealed class PartnerInfoResponse : MmsResponse
{
    [JsonPropertyName("merchantAccount")]
    public string? MerchantAccount { get; init; }

    [JsonPropertyName("partnerCode")]
    public string? PartnerCode { get; init; }

    [JsonPropertyName("site")]
    public string? Site { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("phone")]
    public string? Phone { get; init; }

    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("compensation")]
    public string? Compensation { get; init; }

    [JsonPropertyName("partnerStatus")]
    public string? PartnerStatus { get; init; }

    [JsonPropertyName("createDate")]
    public string? CreateDate { get; init; }
}
