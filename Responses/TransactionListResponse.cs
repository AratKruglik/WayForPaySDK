using System.Text.Json.Serialization;
using WayForPaySDK.Domain;

namespace WayForPaySDK.Responses;

/// <summary>
/// Response containing a list of transactions for the requested date range.
/// </summary>
public sealed class TransactionListResponse : ApiResponse
{
    /// <summary>
    /// Gets or sets the merchant account.
    /// </summary>
    [JsonPropertyName("merchantAccount")]
    public string? MerchantAccount { get; init; }

    /// <summary>
    /// Gets or sets the list of transactions.
    /// </summary>
    [JsonPropertyName("transactionList")]
    public Transaction[]? TransactionList { get; init; }

    /// <inheritdoc />
    public override IEnumerable<string> GetSignatureFields()
    {
        return new[] { MerchantAccount ?? string.Empty };
    }
}
