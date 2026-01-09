using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

/// Cancels a previously authorized transaction (AUTH) without capturing funds.
public sealed class VoidRequest : ApiRequest
{
    public override string TransactionType => "VOID";

    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; set; }

    [JsonPropertyName("amount")]
    public required decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public required string Currency { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    public override IEnumerable<string> GetSignatureFields()
    {
        return new[]
        {
            MerchantAccount,
            OrderReference,
            Amount.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture),
            Currency
        };
    }
}
