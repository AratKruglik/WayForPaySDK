using System.Text.Json.Serialization;
using WayForPaySDK.Domain;
using WayForPaySDK.Domain.Enums;

namespace WayForPaySDK.Requests;

public sealed class ChargeRequest : ApiRequest
{
    public override string TransactionType => "CHARGE";

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

    [JsonPropertyName("card")]
    public string? CardNumber { get; set; }

    [JsonPropertyName("expMonth")]
    public string? ExpMonth { get; set; }

    [JsonPropertyName("expYear")]
    public string? ExpYear { get; set; }

    [JsonPropertyName("cardCvv")]
    public string? CardCvv { get; set; }

    [JsonPropertyName("cardHolder")]
    public string? CardHolder { get; set; }

    [JsonPropertyName("recToken")]
    public string? RecToken { get; set; }

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

    [JsonPropertyName("clientIpAddress")]
    public string? ClientIpAddress { get; set; }

    [JsonPropertyName("serviceUrl")]
    public string? ServiceUrl { get; set; }

    [JsonPropertyName("merchantTransactionType")]
    public string? MerchantTransactionType { get; set; }

    [JsonPropertyName("merchantTransactionSecureType")]
    public string? MerchantTransactionSecureType { get; set; }

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

        fields.AddRange(ProductName);
        fields.AddRange(ProductCount.Select(c => c.ToString()));
        fields.AddRange(ProductPrice.Select(p => p.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)));

        return fields;
    }
}
