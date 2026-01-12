using System.Globalization;
using System.Text.Json.Serialization;

namespace WayForPaySDK.Responses;

public sealed class P2PCreditResponse : ApiResponse
{
    [JsonPropertyName("merchantAccount")]
    public required string MerchantAccount { get; init; }

    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; init; }

    [JsonPropertyName("amount")]
    public required decimal Amount { get; init; }

    [JsonPropertyName("currency")]
    public required string Currency { get; init; }

    [JsonPropertyName("authCode")]
    public string? AuthCode { get; init; }

    [JsonPropertyName("createdDate")]
    public long? CreatedDate { get; init; }

    [JsonPropertyName("processingDate")]
    public long? ProcessingDate { get; init; }

    [JsonPropertyName("transactionStatus")]
    public required string TransactionStatus { get; init; }

    public override IEnumerable<string> GetSignatureFields()
    {
        return new[]
        {
            MerchantAccount,
            OrderReference,
            Amount.ToString("0.##", CultureInfo.InvariantCulture),
            Currency,
            AuthCode ?? string.Empty,
            CreatedDate?.ToString() ?? string.Empty,
            ProcessingDate?.ToString() ?? string.Empty,
            TransactionStatus,
            ReasonCode.ToString(CultureInfo.InvariantCulture)
        };
    }
}
