using System.Text.Json.Serialization;

namespace WayForPaySDK.Responses;

/// <summary>
/// Response from a purchase request (redirect flow).
/// Contains the URL to redirect the client to for payment.
/// </summary>
public sealed class PurchaseResponse : ApiResponse
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
    /// Gets or sets the redirect URL to WayForPay payment page.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    /// Gets or sets the invoice URL (alternative payment link).
    /// </summary>
    [JsonPropertyName("invoiceUrl")]
    public string? InvoiceUrl { get; init; }

    /// <summary>
    /// Gets or sets the QR code image URL for payment.
    /// </summary>
    [JsonPropertyName("qrCode")]
    public string? QrCode { get; init; }

    /// <inheritdoc />
    public override IEnumerable<string> GetSignatureFields()
    {
        return new[]
        {
            MerchantAccount ?? string.Empty,
            OrderReference ?? string.Empty,
            ReasonCode.ToString()
        };
    }
}
