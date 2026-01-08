using WayForPaySDK.Domain.Enums;

namespace WayForPaySDK.Domain;

/// <summary>
/// Represents a payment transaction.
/// </summary>
public sealed record Transaction
{
    /// <summary>
    /// Gets the order reference (unique order identifier).
    /// </summary>
    public required string OrderReference { get; init; }

    /// <summary>
    /// Gets the date when the transaction was created.
    /// </summary>
    public required DateTimeOffset CreatedDate { get; init; }

    /// <summary>
    /// Gets the transaction amount.
    /// </summary>
    public required decimal Amount { get; init; }

    /// <summary>
    /// Gets the transaction currency.
    /// </summary>
    public required Currency Currency { get; init; }

    /// <summary>
    /// Gets the transaction status.
    /// </summary>
    public required TransactionStatus Status { get; init; }

    /// <summary>
    /// Gets the date when the transaction was processed.
    /// </summary>
    public required DateTimeOffset ProcessingDate { get; init; }

    /// <summary>
    /// Gets the transaction reason.
    /// </summary>
    public required Reason Reason { get; init; }

    /// <summary>
    /// Gets the client's email address.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Gets the client's phone number.
    /// </summary>
    public string? Phone { get; init; }

    /// <summary>
    /// Gets the payment system used for the transaction.
    /// </summary>
    public PaymentSystem? PaymentSystem { get; init; }

    /// <summary>
    /// Gets the masked card number (PAN).
    /// </summary>
    public string? CardPan { get; init; }

    /// <summary>
    /// Gets the card type (e.g., Visa, MasterCard).
    /// </summary>
    public string? CardType { get; init; }

    /// <summary>
    /// Gets the card issuer bank country.
    /// </summary>
    public string? IssuerBankCountry { get; init; }

    /// <summary>
    /// Gets the card issuer bank name.
    /// </summary>
    public string? IssuerBankName { get; init; }

    /// <summary>
    /// Gets the transaction fee.
    /// </summary>
    public decimal? Fee { get; init; }

    /// <summary>
    /// Gets the base amount (in base currency).
    /// </summary>
    public decimal? BaseAmount { get; init; }

    /// <summary>
    /// Gets the base currency.
    /// </summary>
    public Currency? BaseCurrency { get; init; }

    /// <summary>
    /// Gets a value indicating whether the transaction status is Created.
    /// </summary>
    public bool IsStatusCreated => Status == TransactionStatus.Created;

    /// <summary>
    /// Gets a value indicating whether the transaction status is InProcessing.
    /// </summary>
    public bool IsStatusInProcessing => Status == TransactionStatus.InProcessing;

    /// <summary>
    /// Gets a value indicating whether the transaction status is WaitingAuthComplete.
    /// </summary>
    public bool IsStatusWaitAuthComplete => Status == TransactionStatus.WaitingAuthComplete;

    /// <summary>
    /// Gets a value indicating whether the transaction status is Approved.
    /// </summary>
    public bool IsStatusApproved => Status == TransactionStatus.Approved;

    /// <summary>
    /// Gets a value indicating whether the transaction status is Pending.
    /// </summary>
    public bool IsStatusPending => Status == TransactionStatus.Pending;

    /// <summary>
    /// Gets a value indicating whether the transaction status is Expired.
    /// </summary>
    public bool IsStatusExpired => Status == TransactionStatus.Expired;

    /// <summary>
    /// Gets a value indicating whether the transaction status is Refunded.
    /// </summary>
    public bool IsStatusRefunded => Status == TransactionStatus.Refunded;

    /// <summary>
    /// Gets a value indicating whether the transaction status is Voided.
    /// </summary>
    public bool IsStatusVoided => Status == TransactionStatus.Voided;

    /// <summary>
    /// Gets a value indicating whether the transaction status is Declined.
    /// </summary>
    public bool IsStatusDeclined => Status == TransactionStatus.Declined;

    /// <summary>
    /// Gets a value indicating whether the transaction status is RefundInProcessing.
    /// </summary>
    public bool IsStatusRefundInProcessing => Status == TransactionStatus.RefundInProcessing;
}
