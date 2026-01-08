using System.Text.Json.Serialization;
using WayForPaySDK.Domain;

namespace WayForPaySDK.Responses;

/// <summary>
/// Base class for all WayForPay API responses.
/// </summary>
public abstract class ApiResponse
{
    /// <summary>
    /// Gets or sets the reason code.
    /// </summary>
    [JsonPropertyName("reasonCode")]
    public required int ReasonCode { get; init; }

    /// <summary>
    /// Gets or sets the reason message.
    /// </summary>
    [JsonPropertyName("reason")]
    public required string ReasonMessage { get; init; }

    /// <summary>
    /// Gets or sets the merchant signature from the response.
    /// </summary>
    [JsonPropertyName("merchantSignature")]
    public string? MerchantSignature { get; init; }

    /// <summary>
    /// Gets the reason as a domain object.
    /// </summary>
    [JsonIgnore]
    public Reason Reason => new() { Code = ReasonCode, Message = ReasonMessage };

    /// <summary>
    /// Gets a value indicating whether the response indicates success.
    /// </summary>
    [JsonIgnore]
    public bool IsSuccess => Reason.IsSuccess;

    /// <summary>
    /// Gets the fields used to verify the response signature, in order.
    /// </summary>
    public abstract IEnumerable<string> GetSignatureFields();
}
