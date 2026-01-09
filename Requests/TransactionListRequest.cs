using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

public sealed class TransactionListRequest : ApiRequest
{
    public override string TransactionType => "TRANSACTION_LIST";

    [JsonPropertyName("merchantDomainName")]
    public required string MerchantDomainName { get; set; }

    [JsonPropertyName("dateBegin")]
    public required long DateBegin { get; set; }

    [JsonPropertyName("dateEnd")]
    public required long DateEnd { get; set; }

    public override IEnumerable<string> GetSignatureFields()
    {
        return new[]
        {
            MerchantAccount,
            DateBegin.ToString(),
            DateEnd.ToString()
        };
    }
}
