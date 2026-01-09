using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

/// Captures a previously authorized transaction (when MerchantTransactionType is AUTH).
public sealed class SettleRequest : ApiRequest
{
    public override string TransactionType => "SETTLE";

    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; set; }

    /// Amount to settle (must be ≤ authorized amount)
    [JsonPropertyName("amount")]
    public required decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public required string Currency { get; set; }

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
