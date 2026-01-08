using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

/// <summary>
/// Request for completing 3D Secure authentication.
/// Used after cardholder completes authentication on bank's ACS page.
/// </summary>
public sealed class Complete3DSRequest : ApiRequest
{
    /// <inheritdoc />
    public override string TransactionType => "COMPLETE_3DS";

    /// <summary>
    /// Gets or sets the merchant domain name.
    /// </summary>
    [JsonPropertyName("merchantDomainName")]
    public required string MerchantDomainName { get; set; }

    /// <summary>
    /// Gets or sets the Payment Authentication Response (PARes) from the bank.
    /// This value is received after successful 3D Secure authentication.
    /// </summary>
    [JsonPropertyName("d3Md")]
    public required string D3Md { get; set; }

    /// <summary>
    /// Gets or sets the Payment Authentication Response (PARes) from the bank.
    /// Base64-encoded XML message containing authentication result.
    /// </summary>
    [JsonPropertyName("d3Pares")]
    public required string D3Pares { get; set; }

    /// <inheritdoc />
    public override IEnumerable<string> GetSignatureFields()
    {
        return new[] { MerchantAccount, D3Md, D3Pares };
    }
}
