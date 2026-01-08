using System.Text.Json.Serialization;

namespace WayForPaySDK.Responses;

/// <summary>
/// Response from a refund request.
/// </summary>
public sealed class RefundResponse : ApiResponse
{
    /// <summary>
    /// Gets or sets the merchant account.
    /// </summary>
    [JsonPropertyName("merchantAccount")]
    public string? MerchantAccount { get; init; }

    /// <summary>
    /// Gets or sets the order reference.
    /// </summary>
    [JsonPropertyName("orderReference")]
    public string? OrderReference { get; init; }

    /// <summary>
    /// Gets or sets the transaction status.
    /// </summary>
    [JsonPropertyName("transactionStatus")]
    public string? TransactionStatus { get; init; }

    /// <summary>
    /// Gets or sets the refunded amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal? Amount { get; init; }

    /// <summary>
    /// Gets or sets the currency code.
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    /// <inheritdoc />
    public override IEnumerable<string> GetSignatureFields()
    {
        return new[]
        {
            MerchantAccount ?? string.Empty,
            OrderReference ?? string.Empty,
            TransactionStatus ?? string.Empty,
            ReasonCode.ToString()
        };
    }
}
