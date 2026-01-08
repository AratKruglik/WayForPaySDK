using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

/// <summary>
/// Request to check transaction status.
/// </summary>
public sealed class CheckStatusRequest : ApiRequest
{
    /// <inheritdoc />
    public override string TransactionType => "CHECK_STATUS";

    /// <summary>
    /// Gets or sets the order reference to check.
    /// </summary>
    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; set; }

    /// <inheritdoc />
    public override IEnumerable<string> GetSignatureFields()
    {
        return new[]
        {
            MerchantAccount,
            OrderReference
        };
    }
}
