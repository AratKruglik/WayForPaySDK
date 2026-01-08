using System.Text.Json.Serialization;

namespace WayForPaySDK.Responses;

/// <summary>
/// Response from an invoice creation request.
/// Contains the invoice URL that can be sent to the client for payment.
/// </summary>
public sealed class InvoiceResponse : ApiResponse
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
    /// Gets or sets the invoice URL for payment.
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
