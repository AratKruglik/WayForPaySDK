using System.Text.Json.Serialization;

namespace WayForPaySDK.Responses;

public sealed class VerifyResponse : ApiResponse
{
    [JsonPropertyName("merchantAccount")]
    public string? MerchantAccount { get; init; }

    [JsonPropertyName("orderReference")]
    public string? OrderReference { get; init; }

    [JsonPropertyName("cardPan")]
    public string? CardPan { get; init; }

    [JsonPropertyName("transactionStatus")]
    public string? TransactionStatus { get; init; }

    [JsonPropertyName("recToken")]
    public string? RecToken { get; init; }

    [JsonPropertyName("paymentSystem")]
    public string? PaymentSystem { get; init; }

    [JsonPropertyName("rrn")]
    public string? Rrn { get; init; }

    [JsonPropertyName("url")]
    public string? Url { get; init; }

    public override IEnumerable<string> GetSignatureFields()
    {
        return new[] { MerchantAccount ?? string.Empty, OrderReference ?? string.Empty };
    }
}
