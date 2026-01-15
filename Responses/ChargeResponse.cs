using System.Globalization;
using System.Text.Json.Serialization;
using WayForPaySDK.Domain.Enums;

namespace WayForPaySDK.Responses;

public sealed class ChargeResponse : ApiResponse
{
    [JsonPropertyName("merchantAccount")]
    public string? MerchantAccount { get; init; }

    [JsonPropertyName("orderReference")]
    public string? OrderReference { get; init; }

    [JsonPropertyName("amount")]
    public decimal? Amount { get; init; }

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    [JsonPropertyName("authCode")]
    public string? AuthCode { get; init; }

    [JsonPropertyName("cardPan")]
    public string? CardPan { get; init; }

    [JsonPropertyName("cardType")]
    public string? CardType { get; init; }

    [JsonPropertyName("transactionStatus")]
    public string? TransactionStatus { get; init; }

    [JsonPropertyName("issuerBankCountry")]
    public string? IssuerBankCountry { get; init; }

    [JsonPropertyName("issuerBankName")]
    public string? IssuerBankName { get; init; }

    [JsonPropertyName("recToken")]
    public string? RecToken { get; init; }

    [JsonPropertyName("transactionId")]
    public long? TransactionId { get; init; }

    [JsonPropertyName("createdDate")]
    public long? CreatedDate { get; init; }

    [JsonPropertyName("processingDate")]
    public long? ProcessingDate { get; init; }

    [JsonPropertyName("fee")]
    public decimal? Fee { get; init; }

    [JsonPropertyName("paymentSystem")]
    public string? PaymentSystem { get; init; }

    [JsonPropertyName("d3AcsUrl")]
    public string? ThreeDsAcsUrl { get; init; }

    [JsonPropertyName("d3Md")]
    public string? ThreeDsMd { get; init; }

    [JsonPropertyName("d3Pareq")]
    public string? ThreeDsPaReq { get; init; }

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
            ReasonCode.ToString(CultureInfo.InvariantCulture)
        };
    }
}
