using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

/// <summary>
/// Request for retrieving a list of transactions for a given date range.
/// </summary>
public sealed class TransactionListRequest : ApiRequest
{
    /// <inheritdoc />
    public override string TransactionType => "TRANSACTION_LIST";

    /// <summary>
    /// Gets or sets the merchant domain name.
    /// </summary>
    [JsonPropertyName("merchantDomainName")]
    public required string MerchantDomainName { get; set; }

    /// <summary>
    /// Gets or sets the start date for transaction search as Unix timestamp.
    /// </summary>
    [JsonPropertyName("dateBegin")]
    public required long DateBegin { get; set; }

    /// <summary>
    /// Gets or sets the end date for transaction search as Unix timestamp.
    /// </summary>
    [JsonPropertyName("dateEnd")]
    public required long DateEnd { get; set; }

    /// <inheritdoc />
    public override IEnumerable<string> GetSignatureFields()
    {
        return new[]
        {
            MerchantAccount,
            DateBegin.ToString(),
            DateEnd.ToString()
        };
    }
}
