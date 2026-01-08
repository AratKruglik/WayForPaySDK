using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

/// <summary>
/// Request to refund a transaction.
/// </summary>
public sealed class RefundRequest : ApiRequest
{
    /// <inheritdoc />
    public override string TransactionType => "REFUND";

    /// <summary>
    /// Gets or sets the original order reference.
    /// </summary>
    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; set; }

    /// <summary>
    /// Gets or sets the refund amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public required decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the currency code.
    /// </summary>
    [JsonPropertyName("currency")]
    public required string Currency { get; set; }

    /// <summary>
    /// Gets or sets the refund comment/reason.
    /// </summary>
    [JsonPropertyName("comment")]
    public required string Comment { get; set; }

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
