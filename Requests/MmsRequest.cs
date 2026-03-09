using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

public abstract class MmsRequest
{
    [JsonPropertyName("merchantAccount")]
    public required string MerchantAccount { get; set; }

    [JsonPropertyName("merchantSignature")]
    public required string MerchantSignature { get; set; }

    [JsonIgnore]
    public abstract string MmsOperation { get; }

    public abstract IEnumerable<string> GetSignatureFields();
}
