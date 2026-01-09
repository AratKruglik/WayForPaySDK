using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

public sealed class Complete3DSRequest : ApiRequest
{
    public override string TransactionType => "COMPLETE_3DS";

    [JsonPropertyName("merchantDomainName")]
    public required string MerchantDomainName { get; set; }

    [JsonPropertyName("d3Md")]
    public required string D3Md { get; set; }

    [JsonPropertyName("d3Pares")]
    public required string D3Pares { get; set; }

    public override IEnumerable<string> GetSignatureFields()
    {
        return new[] { MerchantAccount, D3Md, D3Pares };
    }
}
