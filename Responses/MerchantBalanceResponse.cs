using System.Text.Json.Serialization;

namespace WayForPaySDK.Responses;

public sealed class MerchantBalanceResponse : MmsResponse
{
    [JsonPropertyName("merchantAccount")]
    public string? MerchantAccount { get; init; }

    [JsonPropertyName("balance_UAH")]
    public decimal? BalanceUah { get; init; }
}
