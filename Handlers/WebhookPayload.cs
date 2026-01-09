using System.Text.Json.Serialization;
using WayForPaySDK.Constants;

namespace WayForPaySDK.Handlers;

public sealed record WebhookPayload
{
    [JsonPropertyName("merchantAccount")]
    public required string MerchantAccount { get; init; }

    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; init; }

    [JsonPropertyName("merchantSignature")]
    public required string MerchantSignature { get; init; }

    [JsonPropertyName("amount")]
    [JsonConverter(typeof(Serialization.DecimalJsonConverter))]
    public required decimal Amount { get; init; }

    [JsonPropertyName("currency")]
    public required string Currency { get; init; }

    [JsonPropertyName("authCode")]
    public string? AuthCode { get; init; }

    [JsonPropertyName("cardPan")]
    public string? CardPan { get; init; }

    [JsonPropertyName("cardType")]
    public string? CardType { get; init; }

    [JsonPropertyName("transactionStatus")]
    public required string TransactionStatus { get; init; }

    [JsonPropertyName("reasonCode")]
    public required int ReasonCode { get; init; }

    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    [JsonPropertyName("fee")]
    public decimal? Fee { get; init; }

    [JsonPropertyName("paymentSystem")]
    public string? PaymentSystem { get; init; }

    [JsonPropertyName("recToken")]
    public string? RecToken { get; init; }

    [JsonPropertyName("transactionDate")]
    public long? TransactionDate { get; init; }

    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("phone")]
    public string? Phone { get; init; }

    [JsonPropertyName("clientFirstName")]
    public string? ClientFirstName { get; init; }

    [JsonPropertyName("clientLastName")]
    public string? ClientLastName { get; init; }

    [JsonPropertyName("processingDate")]
    public long? ProcessingDate { get; init; }

    [JsonIgnore]
    public bool IsSuccess => ReasonCode == ReasonCodes.Ok;

    [JsonIgnore]
    public bool IsApproved => TransactionStatus == "Approved";

    [JsonIgnore]
    public bool IsDeclined => TransactionStatus == "Declined";

    [JsonIgnore]
    public bool IsRefunded => TransactionStatus == "Refunded";

    [JsonIgnore]
    public bool IsInProcessing => TransactionStatus == "InProcessing";

    [JsonIgnore]
    public bool IsVoided => TransactionStatus == "Voided";
}
