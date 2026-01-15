using System.Globalization;
using System.Text.Json.Serialization;

namespace WayForPaySDK.Responses;

public sealed class InvoiceResponse : ApiResponse
{
    [JsonPropertyName("merchantAccount")]
    public string? MerchantAccount { get; init; }

    [JsonPropertyName("orderReference")]
    public string? OrderReference { get; init; }

    [JsonPropertyName("invoiceUrl")]
    public string? InvoiceUrl { get; init; }

    [JsonPropertyName("qrCode")]
    public string? QrCode { get; init; }

    public override IEnumerable<string> GetSignatureFields()
    {
        return new[]
        {
            MerchantAccount ?? string.Empty,
            OrderReference ?? string.Empty,
            ReasonCode.ToString(CultureInfo.InvariantCulture)
        };
    }
}
