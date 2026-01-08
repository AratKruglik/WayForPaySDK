using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

/// <summary>
/// Base class for all WayForPay API requests.
/// </summary>
public abstract class ApiRequest
{
    /// <summary>
    /// Gets the transaction type identifier (e.g., "CHARGE", "REFUND", "CHECK_STATUS").
    /// </summary>
    [JsonPropertyName("transactionType")]
    public abstract string TransactionType { get; }

    /// <summary>
    /// Gets or sets the merchant account identifier.
    /// </summary>
    [JsonPropertyName("merchantAccount")]
    public required string MerchantAccount { get; set; }

    /// <summary>
    /// Gets or sets the request signature.
    /// </summary>
    [JsonPropertyName("merchantSignature")]
    public required string MerchantSignature { get; set; }

    /// <summary>
    /// Gets the API version. Defaults to 1.
    /// </summary>
    [JsonPropertyName("apiVersion")]
    public virtual int ApiVersion => 1;

    /// <summary>
    /// Gets the fields used to generate the request signature, in order.
    /// </summary>
    public abstract IEnumerable<string> GetSignatureFields();
}
