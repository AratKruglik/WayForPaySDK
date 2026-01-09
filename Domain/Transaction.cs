using WayForPaySDK.Domain.Enums;

namespace WayForPaySDK.Domain;

public sealed record Transaction
{
    public required string OrderReference { get; init; }
    public required DateTimeOffset CreatedDate { get; init; }
    public required decimal Amount { get; init; }
    public required Currency Currency { get; init; }
    public required TransactionStatus Status { get; init; }
    public required DateTimeOffset ProcessingDate { get; init; }
    public required Reason Reason { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public PaymentSystem? PaymentSystem { get; init; }
    public string? CardPan { get; init; }
    public string? CardType { get; init; }
    public string? IssuerBankCountry { get; init; }
    public string? IssuerBankName { get; init; }
    public decimal? Fee { get; init; }
    public decimal? BaseAmount { get; init; }
    public Currency? BaseCurrency { get; init; }

    public bool IsStatusCreated => Status == TransactionStatus.Created;
    public bool IsStatusInProcessing => Status == TransactionStatus.InProcessing;
    public bool IsStatusWaitAuthComplete => Status == TransactionStatus.WaitingAuthComplete;
    public bool IsStatusApproved => Status == TransactionStatus.Approved;
    public bool IsStatusPending => Status == TransactionStatus.Pending;
    public bool IsStatusExpired => Status == TransactionStatus.Expired;
    public bool IsStatusRefunded => Status == TransactionStatus.Refunded;
    public bool IsStatusVoided => Status == TransactionStatus.Voided;
    public bool IsStatusDeclined => Status == TransactionStatus.Declined;
    public bool IsStatusRefundInProcessing => Status == TransactionStatus.RefundInProcessing;
}
