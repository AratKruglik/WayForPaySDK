using System.Text.Json.Serialization;
using WayForPaySDK.Constants;

namespace WayForPaySDK.Handlers;

/// <summary>
/// Webhook payload received from WayForPay payment system.
/// Contains transaction details and status after payment processing.
/// </summary>
public sealed record WebhookPayload
{
    /// <summary>
    /// Gets or sets the merchant account identifier.
    /// </summary>
    [JsonPropertyName("merchantAccount")]
    public required string MerchantAccount { get; init; }

    /// <summary>
    /// Gets or sets the unique order reference.
    /// </summary>
    [JsonPropertyName("orderReference")]
    public required string OrderReference { get; init; }

    /// <summary>
    /// Gets or sets the webhook signature for validation.
    /// Calculated from: merchantAccount;orderReference;amount;currency;authCode;cardPan;transactionStatus;reasonCode
    /// </summary>
    [JsonPropertyName("merchantSignature")]
    public required string MerchantSignature { get; init; }

    /// <summary>
    /// Gets or sets the transaction amount.
    /// </summary>
    [JsonPropertyName("amount")]
    [JsonConverter(typeof(Serialization.DecimalJsonConverter))]
    public required decimal Amount { get; init; }

    /// <summary>
    /// Gets or sets the currency code (e.g., "UAH", "USD", "EUR").
    /// </summary>
    [JsonPropertyName("currency")]
    public required string Currency { get; init; }

    /// <summary>
    /// Gets or sets the bank authorization code.
    /// </summary>
    [JsonPropertyName("authCode")]
    public string? AuthCode { get; init; }

    /// <summary>
    /// Gets or sets the masked card number (e.g., "411111****1111").
    /// </summary>
    [JsonPropertyName("cardPan")]
    public string? CardPan { get; init; }

    /// <summary>
    /// Gets or sets the card type (e.g., "Visa", "MasterCard").
    /// </summary>
    [JsonPropertyName("cardType")]
    public string? CardType { get; init; }

    /// <summary>
    /// Gets or sets the transaction status.
    /// Values: "Approved", "Declined", "Refunded", "InProcessing", "WaitingAuthComplete", "Expired", "Voided"
    /// </summary>
    [JsonPropertyName("transactionStatus")]
    public required string TransactionStatus { get; init; }

    /// <summary>
    /// Gets or sets the reason code for the transaction result.
    /// </summary>
    [JsonPropertyName("reasonCode")]
    public required int ReasonCode { get; init; }

    /// <summary>
    /// Gets or sets the reason description.
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    /// <summary>
    /// Gets or sets the transaction fee amount.
    /// </summary>
    [JsonPropertyName("fee")]
    public decimal? Fee { get; init; }

    /// <summary>
    /// Gets or sets the payment system name (e.g., "card", "privat24", "googlePay", "applePay").
    /// </summary>
    [JsonPropertyName("paymentSystem")]
    public string? PaymentSystem { get; init; }

    /// <summary>
    /// Gets or sets the recurring payment token for future transactions.
    /// </summary>
    [JsonPropertyName("recToken")]
    public string? RecToken { get; init; }

    /// <summary>
    /// Gets or sets the transaction date as Unix timestamp.
    /// </summary>
    [JsonPropertyName("transactionDate")]
    public long? TransactionDate { get; init; }

    /// <summary>
    /// Gets or sets the client email.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    /// <summary>
    /// Gets or sets the client phone number.
    /// </summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; init; }

    /// <summary>
    /// Gets or sets the client first name.
    /// </summary>
    [JsonPropertyName("clientFirstName")]
    public string? ClientFirstName { get; init; }

    /// <summary>
    /// Gets or sets the client last name.
    /// </summary>
    [JsonPropertyName("clientLastName")]
    public string? ClientLastName { get; init; }

    /// <summary>
    /// Gets or sets the processing date as Unix timestamp.
    /// </summary>
    [JsonPropertyName("processingDate")]
    public long? ProcessingDate { get; init; }

    /// <summary>
    /// Gets a value indicating whether the transaction was successful (reasonCode = 1100).
    /// </summary>
    [JsonIgnore]
    public bool IsSuccess => ReasonCode == ReasonCodes.Ok;

    /// <summary>
    /// Gets a value indicating whether the transaction status is "Approved".
    /// </summary>
    [JsonIgnore]
    public bool IsApproved => TransactionStatus == "Approved";

    /// <summary>
    /// Gets a value indicating whether the transaction status is "Declined".
    /// </summary>
    [JsonIgnore]
    public bool IsDeclined => TransactionStatus == "Declined";

    /// <summary>
    /// Gets a value indicating whether the transaction status is "Refunded".
    /// </summary>
    [JsonIgnore]
    public bool IsRefunded => TransactionStatus == "Refunded";

    /// <summary>
    /// Gets a value indicating whether the transaction status is "InProcessing".
    /// </summary>
    [JsonIgnore]
    public bool IsInProcessing => TransactionStatus == "InProcessing";

    /// <summary>
    /// Gets a value indicating whether the transaction status is "Voided".
    /// </summary>
    [JsonIgnore]
    public bool IsVoided => TransactionStatus == "Voided";
}
