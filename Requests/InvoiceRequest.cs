using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

/// <summary>
/// Request for creating an invoice.
/// Creates a payment link that can be sent to the client.
/// </summary>
public sealed class InvoiceRequest : ApiRequest
{
    /// <inheritdoc />
    public override string TransactionType => "CREATE_INVOICE";

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
    /// Gets or sets the service URL for server-to-server callbacks.
    /// </summary>
    [JsonPropertyName("serviceUrl")]
    public string? ServiceUrl { get; set; }

    /// <summary>
    /// Gets or sets the return URL after successful payment.
    /// </summary>
    [JsonPropertyName("returnUrl")]
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// Gets or sets the payment page language (uk, ru, en).
    /// </summary>
    [JsonPropertyName("language")]
    public string? Language { get; set; }

    /// <summary>
    /// Gets or sets the order lifetime in seconds.
    /// </summary>
    [JsonPropertyName("orderLifetime")]
    public int? OrderLifetime { get; set; }

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
