using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

public sealed class PartnerInfoRequest : MmsRequest
{
    public override string MmsOperation => "partnerInfo";

    [JsonPropertyName("partnerCode")]
    public required string PartnerCode { get; set; }

    public override IEnumerable<string> GetSignatureFields()
    {
        return new[] { MerchantAccount, PartnerCode };
    }
}
