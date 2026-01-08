using System.Text.Json.Serialization;
using WayForPaySDK.Domain.Enums;

namespace WayForPaySDK.Responses;

/// <summary>
/// Response from a charge request.
/// </summary>
public sealed class ChargeResponse : ApiResponse
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
    /// Gets or sets the transaction amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal? Amount { get; init; }

    /// <summary>
    /// Gets or sets the currency code.
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    /// <summary>
    /// Gets or sets the authorization code.
    /// </summary>
    [JsonPropertyName("authCode")]
    public string? AuthCode { get; init; }

    /// <summary>
    /// Gets or sets the masked card number.
    /// </summary>
    [JsonPropertyName("cardPan")]
    public string? CardPan { get; init; }

    /// <summary>
    /// Gets or sets the card type (Visa, MasterCard, etc.).
    /// </summary>
    [JsonPropertyName("cardType")]
    public string? CardType { get; init; }

    /// <summary>
    /// Gets or sets the transaction status.
    /// </summary>
    [JsonPropertyName("transactionStatus")]
    public string? TransactionStatus { get; init; }

    /// <summary>
    /// Gets or sets the issuer bank country.
    /// </summary>
    [JsonPropertyName("issuerBankCountry")]
    public string? IssuerBankCountry { get; init; }

    /// <summary>
    /// Gets or sets the issuer bank name.
    /// </summary>
    [JsonPropertyName("issuerBankName")]
    public string? IssuerBankName { get; init; }

    /// <summary>
    /// Gets or sets the recurring payment token for future charges.
    /// </summary>
    [JsonPropertyName("recToken")]
    public string? RecToken { get; init; }

    /// <summary>
    /// Gets or sets the transaction ID.
    /// </summary>
    [JsonPropertyName("transactionId")]
    public long? TransactionId { get; init; }

    /// <summary>
    /// Gets or sets the processing date as Unix timestamp.
    /// </summary>
    [JsonPropertyName("createdDate")]
    public long? CreatedDate { get; init; }

    /// <summary>
    /// Gets or sets the processing date as Unix timestamp.
    /// </summary>
    [JsonPropertyName("processingDate")]
    public long? ProcessingDate { get; init; }

    /// <summary>
    /// Gets or sets the transaction fee.
    /// </summary>
    [JsonPropertyName("fee")]
    public decimal? Fee { get; init; }

    /// <summary>
    /// Gets or sets the payment system used.
    /// </summary>
    [JsonPropertyName("paymentSystem")]
    public string? PaymentSystem { get; init; }

    /// <summary>
    /// Gets or sets the 3DS redirect URL (if 3DS is required).
    /// </summary>
    [JsonPropertyName("d3AcsUrl")]
    public string? ThreeDsAcsUrl { get; init; }

    /// <summary>
    /// Gets or sets the 3DS MD parameter.
    /// </summary>
    [JsonPropertyName("d3Md")]
    public string? ThreeDsMd { get; init; }

    /// <summary>
    /// Gets or sets the 3DS PaReq parameter.
    /// </summary>
    [JsonPropertyName("d3Pareq")]
    public string? ThreeDsPaReq { get; init; }

    /// <inheritdoc />
    public override IEnumerable<string> GetSignatureFields()
    {
        return new[]
        {
            MerchantAccount ?? string.Empty,
            OrderReference ?? string.Empty,
            Amount?.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            Currency ?? string.Empty,
            AuthCode ?? string.Empty,
            CardPan ?? string.Empty,
            TransactionStatus ?? string.Empty,
            ReasonCode.ToString()
        };
    }
}
