using System.Text.Json.Serialization;
using WayForPaySDK.Domain;
using WayForPaySDK.Domain.Enums;

namespace WayForPaySDK.Requests;

/// <summary>
/// Request for direct card charge (server-to-server).
/// </summary>
public sealed class ChargeRequest : ApiRequest
{
    /// <inheritdoc />
    public override string TransactionType => "CHARGE";

    /// <summary>
    /// Gets or sets the merchant domain name.
    /// </summary>
    [JsonPropertyName("merchantDomainName")]
    public required string MerchantDomainName { get; set; }

    /// <summary>
    /// Gets or sets the unique order reference.
    /// </summary>
    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; set; }

    /// <summary>
    /// Gets or sets the order date as Unix timestamp.
    /// </summary>
    [JsonPropertyName("orderDate")]
    public required long OrderDate { get; set; }

    /// <summary>
    /// Gets or sets the payment amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public required decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the currency code (e.g., "UAH", "USD").
    /// </summary>
    [JsonPropertyName("currency")]
    public required string Currency { get; set; }

    /// <summary>
    /// Gets or sets the card number (when paying with card details).
    /// </summary>
    [JsonPropertyName("card")]
    public string? CardNumber { get; set; }

    /// <summary>
    /// Gets or sets the card expiration month (01-12).
    /// </summary>
    [JsonPropertyName("expMonth")]
    public string? ExpMonth { get; set; }

    /// <summary>
    /// Gets or sets the card expiration year (4 digits).
    /// </summary>
    [JsonPropertyName("expYear")]
    public string? ExpYear { get; set; }

    /// <summary>
    /// Gets or sets the card CVV code.
    /// </summary>
    [JsonPropertyName("cardCvv")]
    public string? CardCvv { get; set; }

    /// <summary>
    /// Gets or sets the cardholder name.
    /// </summary>
    [JsonPropertyName("cardHolder")]
    public string? CardHolder { get; set; }

    /// <summary>
    /// Gets or sets the recurring payment token (alternative to card details).
    /// </summary>
    [JsonPropertyName("recToken")]
    public string? RecToken { get; set; }

    /// <summary>
    /// Gets or sets the product names.
    /// </summary>
    [JsonPropertyName("productName")]
    public required string[] ProductName { get; set; }

    /// <summary>
    /// Gets or sets the product prices.
    /// </summary>
    [JsonPropertyName("productPrice")]
    public required decimal[] ProductPrice { get; set; }

    /// <summary>
    /// Gets or sets the product quantities.
    /// </summary>
    [JsonPropertyName("productCount")]
    public required int[] ProductCount { get; set; }

    /// <summary>
    /// Gets or sets the client first name.
    /// </summary>
    [JsonPropertyName("clientFirstName")]
    public string? ClientFirstName { get; set; }

    /// <summary>
    /// Gets or sets the client last name.
    /// </summary>
    [JsonPropertyName("clientLastName")]
    public string? ClientLastName { get; set; }

    /// <summary>
    /// Gets or sets the client email.
    /// </summary>
    [JsonPropertyName("clientEmail")]
    public string? ClientEmail { get; set; }

    /// <summary>
    /// Gets or sets the client phone.
    /// </summary>
    [JsonPropertyName("clientPhone")]
    public string? ClientPhone { get; set; }

    /// <summary>
    /// Gets or sets the client country code.
    /// </summary>
    [JsonPropertyName("clientCountry")]
    public string? ClientCountry { get; set; }

    /// <summary>
    /// Gets or sets the client IP address.
    /// </summary>
    [JsonPropertyName("clientIpAddress")]
    public string? ClientIpAddress { get; set; }

    /// <summary>
    /// Gets or sets the service URL for callbacks.
    /// </summary>
    [JsonPropertyName("serviceUrl")]
    public string? ServiceUrl { get; set; }

    /// <summary>
    /// Gets or sets the merchant transaction type (SALE or AUTH).
    /// </summary>
    [JsonPropertyName("merchantTransactionType")]
    public string? MerchantTransactionType { get; set; }

    /// <summary>
    /// Gets or sets the merchant transaction secure type (AUTO, 3DS, NON3DS).
    /// </summary>
    [JsonPropertyName("merchantTransactionSecureType")]
    public string? MerchantTransactionSecureType { get; set; }

    /// <summary>
    /// Gets or sets the recurring payment amount (if different from initial).
    /// </summary>
    [JsonPropertyName("regularAmount")]
    public decimal? RegularAmount { get; set; }

    /// <summary>
    /// Gets or sets the recurring payment modes (e.g., ["daily", "monthly"]).
    /// </summary>
    [JsonPropertyName("regularMode")]
    public string[]? RegularMode { get; set; }

    /// <summary>
    /// Gets or sets when recurring payments should occur (day/date of month).
    /// </summary>
    [JsonPropertyName("regularOn")]
    public string? RegularOn { get; set; }

    /// <summary>
    /// Gets or sets the count of recurring payments.
    /// </summary>
    [JsonPropertyName("regularCount")]
    public int? RegularCount { get; set; }

    /// <summary>
    /// Gets or sets the recurring payment behavior (default, none, preset).
    /// </summary>
    [JsonPropertyName("regularBehavior")]
    public string? RegularBehavior { get; set; }

    /// <inheritdoc />
    public override IEnumerable<string> GetSignatureFields()
    {
        var fields = new List<string>
        {
            MerchantAccount,
            MerchantDomainName,
            OrderReference,
            OrderDate.ToString(),
            Amount.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture),
            Currency
        };

        // Add product names
        fields.AddRange(ProductName);

        // Add product counts
        fields.AddRange(ProductCount.Select(c => c.ToString()));

        // Add product prices
        fields.AddRange(ProductPrice.Select(p => p.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)));

        return fields;
    }
}
