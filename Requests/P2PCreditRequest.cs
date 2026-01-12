using System.Globalization;
using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

public sealed class P2PCreditRequest : ApiRequest
{
    public override string TransactionType => "P2P_CREDIT";

    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; set; }

    [JsonPropertyName("amount")]
    public required decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public required string Currency { get; set; }

    [JsonPropertyName("cardBeneficiary")]
    public required string CardBeneficiary { get; set; }

    public override IEnumerable<string> GetSignatureFields()
    {
        return new[]
        {
            MerchantAccount,
            OrderReference,
            Amount.ToString("0.##", CultureInfo.InvariantCulture),
            Currency,
            CardBeneficiary
        };
    }
}
