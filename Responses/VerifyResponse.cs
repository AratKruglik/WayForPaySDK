using System.Text.Json.Serialization;

namespace WayForPaySDK.Responses;

/// <summary>
/// Response from card verification operation.
/// Contains recToken for future recurring payments if verification succeeded.
/// </summary>
public sealed class VerifyResponse : ApiResponse
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
    /// This token should be stored securely and used for future charges.
    /// </summary>
    [JsonPropertyName("recToken")]
    public string? RecToken { get; init; }

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

    /// <summary>
    /// Gets or sets the 3D Secure redirect URL if additional authentication is required.
    /// If present, redirect the user to this URL to complete 3DS authentication,
    /// then call Complete3DS with the authentication result.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <inheritdoc />
    public override IEnumerable<string> GetSignatureFields()
    {
        return new[] { MerchantAccount ?? string.Empty, OrderReference ?? string.Empty };
    }
}
