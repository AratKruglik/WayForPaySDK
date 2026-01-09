using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

public sealed class VerifyRequest : ApiRequest
{
    public override string TransactionType => "VERIFY";

    [JsonPropertyName("merchantDomainName")]
    public required string MerchantDomainName { get; set; }

    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; set; }

    [JsonPropertyName("orderDate")]
    public required long OrderDate { get; set; }

    [JsonPropertyName("card")]
    public required string CardNumber { get; set; }

    [JsonPropertyName("expMonth")]
    public required string ExpMonth { get; set; }

    [JsonPropertyName("expYear")]
    public required string ExpYear { get; set; }

    [JsonPropertyName("cardCvv")]
    public required string CardCvv { get; set; }

    [JsonPropertyName("cardHolder")]
    public required string CardHolder { get; set; }

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
