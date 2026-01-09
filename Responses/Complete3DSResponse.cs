using System.Text.Json.Serialization;

namespace WayForPaySDK.Responses;

public sealed class Complete3DSResponse : ApiResponse
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

    [JsonPropertyName("transactionStatus")]
    public string? TransactionStatus { get; init; }

    [JsonPropertyName("recToken")]
    public string? RecToken { get; init; }

    [JsonPropertyName("fee")]
    public decimal? Fee { get; init; }

    [JsonPropertyName("paymentSystem")]
    public string? PaymentSystem { get; init; }

    [JsonPropertyName("rrn")]
    public string? Rrn { get; init; }

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
