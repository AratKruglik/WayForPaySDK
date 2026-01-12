using System.Text.Json.Serialization;

namespace WayForPaySDK.Responses;

public sealed class CreateQrResponse : ApiResponse
{
    [JsonPropertyName("qrCodeUrl")]
    public string? QrCodeUrl { get; init; }

    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; init; }

    public override IEnumerable<string> GetSignatureFields()
    {
        return new[]
        {
            MerchantSignature ?? string.Empty
        };
        // Note: The base class usually doesn't validate response signature in the same way as request.
        // We need to check how other responses implement GetSignatureFields.
    }
}
