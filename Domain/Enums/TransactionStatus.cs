namespace WayForPaySDK.Domain.Enums;

public enum TransactionStatus
{
    Created,
    InProcessing,
    WaitingAuthComplete,
    Approved,
    Pending,
    Expired,
    Refunded,
    Voided,
    Declined,
    RefundInProcessing
}
