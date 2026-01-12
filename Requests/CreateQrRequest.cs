using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

public sealed class CreateQrRequest : ApiRequest
{
    public override string TransactionType => "CREATE_QR";

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

    [JsonPropertyName("serviceUrl")]
    public string? ServiceUrl { get; set; }

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
