using System.Globalization;
using System.Text.Json.Serialization;

namespace WayForPaySDK.Responses;

public abstract class SimpleTransactionResponse : ApiResponse
{
    [JsonPropertyName("merchantAccount")]
    public string? MerchantAccount { get; init; }

    [JsonPropertyName("orderReference")]
    public string? OrderReference { get; init; }

    [JsonPropertyName("transactionStatus")]
    public string? TransactionStatus { get; init; }

    [JsonPropertyName("amount")]
    public decimal? Amount { get; init; }

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    public override IEnumerable<string> GetSignatureFields()
    {
        return new[]
        {
            MerchantAccount ?? string.Empty,
            OrderReference ?? string.Empty,
            TransactionStatus ?? string.Empty,
            ReasonCode.ToString(CultureInfo.InvariantCulture)
        };
    }
}
