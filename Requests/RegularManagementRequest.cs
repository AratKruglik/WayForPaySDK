using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

public abstract class RegularManagementRequest
{
    [JsonPropertyName("apiVersion")]
    public int ApiVersion => 1;

    [JsonPropertyName("requestType")]
    public abstract string RequestType { get; }

    [JsonPropertyName("merchantAccount")]
    public required string MerchantAccount { get; set; }

    [JsonPropertyName("merchantPassword")]
    public required string MerchantPassword { get; set; }

    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; set; }
}
