using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

/// <summary>
/// Request for verifying card without charging funds.
/// Creates a card token (recToken) for future recurring payments.
/// WayForPay performs a 0.01 UAH hold that is automatically reversed.
/// </summary>
public sealed class VerifyRequest : ApiRequest
{
    /// <inheritdoc />
    public override string TransactionType => "VERIFY";

    /// <summary>
    /// Gets or sets the merchant domain name.
    /// </summary>
    [JsonPropertyName("merchantDomainName")]
    public required string MerchantDomainName { get; set; }

    /// <summary>
    /// Gets or sets the unique order reference for this verification.
    /// </summary>
    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; set; }

    /// <summary>
    /// Gets or sets the order date as Unix timestamp.
    /// </summary>
    [JsonPropertyName("orderDate")]
    public required long OrderDate { get; set; }

    /// <summary>
    /// Gets or sets the card number.
    /// </summary>
    [JsonPropertyName("card")]
    public required string CardNumber { get; set; }

    /// <summary>
    /// Gets or sets the card expiry month (01-12).
    /// </summary>
    [JsonPropertyName("expMonth")]
    public required string ExpMonth { get; set; }

    /// <summary>
    /// Gets or sets the card expiry year (YYYY format).
    /// </summary>
    [JsonPropertyName("expYear")]
    public required string ExpYear { get; set; }

    /// <summary>
    /// Gets or sets the card CVV/CVC security code.
    /// </summary>
    [JsonPropertyName("cardCvv")]
    public required string CardCvv { get; set; }

    /// <summary>
    /// Gets or sets the cardholder's name (as printed on card).
    /// </summary>
    [JsonPropertyName("cardHolder")]
    public required string CardHolder { get; set; }

    /// <summary>
    /// Gets or sets the client's first name (optional).
    /// </summary>
    [JsonPropertyName("clientFirstName")]
    public string? ClientFirstName { get; set; }

    /// <summary>
    /// Gets or sets the client's last name (optional).
    /// </summary>
    [JsonPropertyName("clientLastName")]
    public string? ClientLastName { get; set; }

    /// <summary>
    /// Gets or sets the client's email (optional).
    /// </summary>
    [JsonPropertyName("clientEmail")]
    public string? ClientEmail { get; set; }

    /// <summary>
    /// Gets or sets the client's phone (optional).
    /// </summary>
    [JsonPropertyName("clientPhone")]
    public string? ClientPhone { get; set; }

    /// <summary>
    /// Gets or sets the client's country code (optional).
    /// </summary>
    [JsonPropertyName("clientCountry")]
    public string? ClientCountry { get; set; }

    /// <summary>
    /// Gets or sets the client's IP address (optional but recommended for fraud prevention).
    /// </summary>
    [JsonPropertyName("clientIpAddress")]
    public string? ClientIpAddress { get; set; }

    /// <inheritdoc />
    public override IEnumerable<string> GetSignatureFields()
    {
        return new[]
        {
            MerchantAccount,
            MerchantDomainName,
            OrderReference,
            OrderDate.ToString()
        };
    }
}
