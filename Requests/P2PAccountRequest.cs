using System.Globalization;
using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

public sealed class P2PAccountRequest : ApiRequest
{
    public override string TransactionType => "P2P_ACCOUNT";

    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; set; }

    [JsonPropertyName("amount")]
    public required decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public required string Currency { get; set; }

    [JsonPropertyName("iban")]
    public required string Iban { get; set; }

    [JsonPropertyName("okpo")]
    public required string Okpo { get; set; }

    [JsonPropertyName("accountName")]
    public required string AccountName { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    public override IEnumerable<string> GetSignatureFields()
    {
        return new[]
        {
            MerchantAccount,
            OrderReference,
            Amount.ToString("0.##", CultureInfo.InvariantCulture),
            Currency,
            Iban,
            Okpo,
            AccountName,
            Description
        };
    }
}
