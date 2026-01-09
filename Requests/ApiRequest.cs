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
}
