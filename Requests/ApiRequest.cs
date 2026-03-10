using System.Globalization;
using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

public abstract class ApiRequest
{
    [JsonPropertyName("transactionType")]
    public abstract string TransactionType { get; }

    [JsonPropertyName("merchantAccount")]
    public required string MerchantAccount { get; set; }

    [JsonPropertyName("merchantSignature")]
    public required string MerchantSignature { get; set; }

    [JsonPropertyName("apiVersion")]
    public virtual int ApiVersion => 1;

    public abstract IEnumerable<string> GetSignatureFields();

    protected static IEnumerable<string> BuildProductSignatureFields(
        string merchantAccount, string merchantDomainName, string orderReference,
        long orderDate, decimal amount, string currency,
        string[] productName, int[] productCount, decimal[] productPrice)
    {
        var fields = new List<string>
        {
            merchantAccount,
            merchantDomainName,
            orderReference,
            orderDate.ToString(),
            amount.ToString("0.##", CultureInfo.InvariantCulture),
            currency
        };

        fields.AddRange(productName);
        fields.AddRange(productCount.Select(c => c.ToString()));
        fields.AddRange(productPrice.Select(p => p.ToString("0.##", CultureInfo.InvariantCulture)));

        return fields;
    }
}
