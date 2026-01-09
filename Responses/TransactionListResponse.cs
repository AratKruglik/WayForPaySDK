using System.Text.Json.Serialization;
using WayForPaySDK.Domain;

namespace WayForPaySDK.Responses;

public sealed class TransactionListResponse : ApiResponse
{
    [JsonPropertyName("merchantAccount")]
    public string? MerchantAccount { get; init; }

    [JsonPropertyName("transactionList")]
    public Transaction[]? TransactionList { get; init; }

    public override IEnumerable<string> GetSignatureFields()
    {
        return new[] { MerchantAccount ?? string.Empty };
    }
}
