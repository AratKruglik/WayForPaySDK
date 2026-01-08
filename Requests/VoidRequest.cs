using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

/// <summary>
/// Request to void (cancel) a previously authorized transaction.
/// Used when MerchantTransactionType is AUTH to release the hold without capturing funds.
/// </summary>
public sealed class VoidRequest : ApiRequest
{
    /// <inheritdoc />
    public override string TransactionType => "VOID";

    /// <summary>
    /// Gets or sets the original order reference from the authorization.
    /// </summary>
    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; set; }

    /// <summary>
    /// Gets or sets the amount to void.
    /// </summary>
    [JsonPropertyName("amount")]
    public required decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the currency code.
    /// </summary>
    [JsonPropertyName("currency")]
    public required string Currency { get; set; }

    /// <summary>
    /// Gets or sets the reason/comment for voiding.
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

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
