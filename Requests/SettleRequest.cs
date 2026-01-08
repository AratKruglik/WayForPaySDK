using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

/// <summary>
/// Request to settle (capture) a previously authorized transaction.
/// Used when MerchantTransactionType is AUTH to complete the payment.
/// </summary>
public sealed class SettleRequest : ApiRequest
{
    /// <inheritdoc />
    public override string TransactionType => "SETTLE";

    /// <summary>
    /// Gets or sets the original order reference from the authorization.
    /// </summary>
    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; set; }

    /// <summary>
    /// Gets or sets the amount to settle.
    /// Must be less than or equal to the authorized amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public required decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the currency code.
    /// </summary>
    [JsonPropertyName("currency")]
    public required string Currency { get; set; }

    /// <inheritdoc />
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
