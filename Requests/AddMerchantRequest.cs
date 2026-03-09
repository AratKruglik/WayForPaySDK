using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

public sealed class AddMerchantRequest : CompensatedMmsRequest
{
    public override string MmsOperation => "addMerchant";

    [JsonPropertyName("site")]
    public required string Site { get; set; }

    [JsonPropertyName("phone")]
    public required string Phone { get; set; }

    [JsonPropertyName("email")]
    public required string Email { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    public override IEnumerable<string> GetSignatureFields()
    {
        return new[] { MerchantAccount, Site, Phone, Email };
    }
}
