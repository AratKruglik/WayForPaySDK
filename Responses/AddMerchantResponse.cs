using System.Text.Json.Serialization;

namespace WayForPaySDK.Responses;

public sealed class AddMerchantResponse : MmsResponse
{
    [JsonPropertyName("merchantAccount")]
    public string? MerchantAccount { get; init; }

    [JsonPropertyName("secretKey")]
    public string? SecretKey { get; init; }
}
