using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

public sealed class UpdatePartnerRequest : CompensatedMmsRequest
{
    public override string MmsOperation => "updatePartner";

    [JsonPropertyName("partnerCode")]
    public required string PartnerCode { get; set; }

    [JsonPropertyName("site")]
    public string? Site { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    public override IEnumerable<string> GetSignatureFields()
    {
        return new[] { MerchantAccount, PartnerCode };
    }
}
