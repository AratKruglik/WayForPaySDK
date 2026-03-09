using System.Text.Json.Serialization;

namespace WayForPaySDK.Responses;

public sealed class MerchantInfoResponse : MmsResponse
{
    [JsonPropertyName("merchantAccount")]
    public string? MerchantAccount { get; init; }

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

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("createDate")]
    public string? CreateDate { get; init; }
}
