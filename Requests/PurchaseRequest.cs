using System.Text.Json.Serialization;
using WayForPaySDK.Domain;

namespace WayForPaySDK.Requests;

public sealed class PurchaseRequest : ApiRequest
{
    public override string TransactionType => "PURCHASE";

    [JsonPropertyName("merchantDomainName")]
    public required string MerchantDomainName { get; set; }

    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; set; }

    [JsonPropertyName("orderDate")]
    public required long OrderDate { get; set; }

    [JsonPropertyName("amount")]
    public required decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public required string Currency { get; set; }

    [JsonPropertyName("productName")]
    public required string[] ProductName { get; set; }

    [JsonPropertyName("productPrice")]
    public required decimal[] ProductPrice { get; set; }

    [JsonPropertyName("productCount")]
    public required int[] ProductCount { get; set; }

    [JsonPropertyName("clientFirstName")]
    public string? ClientFirstName { get; set; }

    [JsonPropertyName("clientLastName")]
    public string? ClientLastName { get; set; }

    [JsonPropertyName("clientEmail")]
    public string? ClientEmail { get; set; }

    [JsonPropertyName("clientPhone")]
    public string? ClientPhone { get; set; }

    [JsonPropertyName("clientCountry")]
    public string? ClientCountry { get; set; }

    [JsonPropertyName("returnUrl")]
    public string? ReturnUrl { get; set; }

    [JsonPropertyName("serviceUrl")]
    public string? ServiceUrl { get; set; }

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("orderTimeout")]
    public int? OrderTimeout { get; set; }

    [JsonPropertyName("merchantTransactionType")]
    public string? MerchantTransactionType { get; set; }

    [JsonPropertyName("paymentSystems")]
    public string? PaymentSystems { get; set; }

    [JsonPropertyName("regularAmount")]
    public decimal? RegularAmount { get; set; }

    [JsonPropertyName("regularMode")]
    public string[]? RegularMode { get; set; }

    [JsonPropertyName("regularOn")]
    public string? RegularOn { get; set; }

    [JsonPropertyName("regularCount")]
    public int? RegularCount { get; set; }

    [JsonPropertyName("regularBehavior")]
    public string? RegularBehavior { get; set; }

    [JsonPropertyName("splits")]
    public IEnumerable<Split>? Splits { get; set; }

    public override IEnumerable<string> GetSignatureFields() =>
        BuildProductSignatureFields(MerchantAccount, MerchantDomainName, OrderReference,
            OrderDate, Amount, Currency, ProductName, ProductCount, ProductPrice);
}
