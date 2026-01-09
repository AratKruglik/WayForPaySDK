using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

public sealed class CheckStatusRequest : ApiRequest
{
    public override string TransactionType => "CHECK_STATUS";

    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; set; }

    public override IEnumerable<string> GetSignatureFields()
    {
        return new[]
        {
            MerchantAccount,
            OrderReference
        };
    }
}
