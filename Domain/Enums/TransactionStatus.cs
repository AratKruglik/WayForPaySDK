namespace WayForPaySDK.Domain.Enums;

/// <summary>
/// Transaction status values returned by WayForPay API.
/// </summary>
public enum TransactionStatus
{
    /// <summary>
    /// Transaction has been created but not yet processed.
    /// </summary>
    Created,

    /// <summary>
    /// Transaction is currently being processed.
    /// </summary>
    InProcessing,

    /// <summary>
    /// Waiting for 3DS authentication to complete.
    /// </summary>
    WaitingAuthComplete,

    /// <summary>
    /// Transaction has been approved successfully.
    /// </summary>
    Approved,

    /// <summary>
    /// Transaction is pending (awaiting confirmation).
    /// </summary>
    Pending,

    /// <summary>
    /// Transaction has expired.
    /// </summary>
    Expired,

    /// <summary>
    /// Transaction has been refunded.
    /// </summary>
    Refunded,

    /// <summary>
    /// Transaction has been voided.
    /// </summary>
    Voided,

    /// <summary>
    /// Transaction has been declined.
    /// </summary>
    Declined,

    /// <summary>
    /// Refund is currently being processed.
    /// </summary>
    RefundInProcessing
}
