using System.Text.Json.Serialization;

namespace WayForPaySDK.Responses;

/// <summary>
/// Response from completing 3D Secure authentication.
/// Contains final transaction status after 3DS verification.
/// </summary>
public sealed class Complete3DSResponse : ApiResponse
{
    /// <summary>
    /// Gets or sets the merchant account.
    /// </summary>
    [JsonPropertyName("merchantAccount")]
    public string? MerchantAccount { get; init; }

    /// <summary>
    /// Gets or sets the merchant's order reference.
    /// </summary>
    [JsonPropertyName("orderReference")]
    public string? OrderReference { get; init; }

    /// <summary>
    /// Gets or sets the transaction amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal? Amount { get; init; }

    /// <summary>
    /// Gets or sets the transaction currency (ISO 4217).
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    /// <summary>
    /// Gets or sets the authorization code from the bank.
    /// Present only for approved transactions.
    /// </summary>
    [JsonPropertyName("authCode")]
    public string? AuthCode { get; init; }

    /// <summary>
    /// Gets or sets the masked card number (e.g., "444444******4444").
    /// </summary>
    [JsonPropertyName("cardPan")]
    public string? CardPan { get; init; }

    /// <summary>
    /// Gets or sets the transaction status.
    /// </summary>
    [JsonPropertyName("transactionStatus")]
    public string? TransactionStatus { get; init; }

    /// <summary>
    /// Gets or sets the cardholder's card token for recurring payments.
    /// Present when card tokenization is enabled.
    /// </summary>
    [JsonPropertyName("recToken")]
    public string? RecToken { get; init; }

    /// <summary>
    /// Gets or sets the fee charged by WayForPay.
    /// </summary>
    [JsonPropertyName("fee")]
    public decimal? Fee { get; init; }

    /// <summary>
    /// Gets or sets the payment system used (e.g., "visa", "mastercard").
    /// </summary>
    [JsonPropertyName("paymentSystem")]
    public string? PaymentSystem { get; init; }

    /// <summary>
    /// Gets or sets the Retrieval Reference Number (RRN) from the acquiring bank.
    /// </summary>
    [JsonPropertyName("rrn")]
    public string? Rrn { get; init; }

    /// <inheritdoc />
    public override IEnumerable<string> GetSignatureFields()
    {
        return new[]
        {
            MerchantAccount ?? string.Empty,
            OrderReference ?? string.Empty,
            Amount?.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            Currency ?? string.Empty
        };
    }
}
