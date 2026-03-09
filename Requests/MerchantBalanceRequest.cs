using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

public sealed class MerchantBalanceRequest : MmsRequest
{
    public override string MmsOperation => "merchantBalance";

    [JsonPropertyName("toDate")]
    public string? ToDate { get; set; }

    public override IEnumerable<string> GetSignatureFields()
    {
        return new[] { MerchantAccount };
    }
}
