using System.Globalization;
using System.Text.Json;
using WayForPaySDK.Crypto;
using WayForPaySDK.Exceptions;
using WayForPaySDK.Serialization;

namespace WayForPaySDK.Handlers;

/// <summary>
/// Implementation of webhook callback handler for WayForPay.
/// Handles parsing, signature validation, and response generation.
/// </summary>
public sealed class WebhookHandler : IWebhookHandler
{
    private readonly ISignatureGenerator _signatureGenerator;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebhookHandler"/> class.
    /// </summary>
    /// <param name="signatureGenerator">Signature generator for validation and signing.</param>
    public WebhookHandler(ISignatureGenerator signatureGenerator)
    {
        _signatureGenerator = signatureGenerator ?? throw new ArgumentNullException(nameof(signatureGenerator));
        _jsonOptions = new JsonSerializerOptions
        {
            TypeInfoResolver = WayForPayJsonContext.Default
        };
    }

    /// <inheritdoc />
    public async Task<WebhookPayload> ParseAsync(Stream body, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(body);

        var payload = await JsonSerializer.DeserializeAsync<WebhookPayload>(
            body,
            _jsonOptions,
            cancellationToken)
            ?? throw new JsonParseException("Failed to deserialize webhook payload");

        ValidateSignature(payload);
        return payload;
    }

    /// <inheritdoc />
    public WebhookPayload Parse(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        var payload = JsonSerializer.Deserialize<WebhookPayload>(json, _jsonOptions)
            ?? throw new JsonParseException("Failed to deserialize webhook payload");

        ValidateSignature(payload);
        return payload;
    }

    /// <inheritdoc />
    public WebhookResponse CreateResponse(WebhookPayload payload, WebhookStatus status = WebhookStatus.Accept)
    {
        ArgumentNullException.ThrowIfNull(payload);

        var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var statusString = status.ToString().ToLowerInvariant();

        // Response signature fields: orderReference;status;time
        var signatureFields = new[]
        {
            payload.OrderReference,
            statusString,
            time.ToString(CultureInfo.InvariantCulture)
        };

        var signature = _signatureGenerator.GenerateSignature(signatureFields);

        return new WebhookResponse
        {
            OrderReference = payload.OrderReference,
            Status = statusString,
            Time = time,
            Signature = signature
        };
    }

    /// <inheritdoc />
    public string SerializeResponse(WebhookResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        return JsonSerializer.Serialize(response, _jsonOptions);
    }

    private void ValidateSignature(WebhookPayload payload)
    {
        // Webhook signature fields:
        // merchantAccount;orderReference;amount;currency;authCode;cardPan;transactionStatus;reasonCode
        var signatureFields = new[]
        {
            payload.MerchantAccount,
            payload.OrderReference,
            payload.Amount.ToString("0.##", CultureInfo.InvariantCulture),
            payload.Currency,
            payload.AuthCode ?? string.Empty,
            payload.CardPan ?? string.Empty,
            payload.TransactionStatus,
            payload.ReasonCode.ToString(CultureInfo.InvariantCulture)
        };

        var expectedSignature = _signatureGenerator.GenerateSignature(signatureFields);

        if (!_signatureGenerator.VerifySignature(expectedSignature, payload.MerchantSignature))
        {
            throw new SignatureException(expectedSignature, payload.MerchantSignature);
        }
    }
}
