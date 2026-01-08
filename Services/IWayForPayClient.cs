using WayForPaySDK.Domain;
using WayForPaySDK.Responses;

namespace WayForPaySDK.Services;

/// <summary>
/// Main interface for WayForPay API operations.
/// </summary>
public interface IWayForPayClient
{
    /// <summary>
    /// Charges a card directly (server-to-server).
    /// </summary>
    /// <param name="orderReference">Unique order identifier.</param>
    /// <param name="amount">Payment amount.</param>
    /// <param name="currency">Currency code (e.g., "UAH").</param>
    /// <param name="card">Card details for payment.</param>
    /// <param name="products">Products in the order.</param>
    /// <param name="client">Client information (optional).</param>
    /// <param name="serviceUrl">Callback URL (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Charge response with transaction details.</returns>
    Task<ChargeResponse> ChargeAsync(
        string orderReference,
        decimal amount,
        string currency,
        Card card,
        IEnumerable<Product> products,
        Client? client = null,
        string? serviceUrl = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Charges using a recurring payment token.
    /// </summary>
    /// <param name="orderReference">Unique order identifier.</param>
    /// <param name="amount">Payment amount.</param>
    /// <param name="currency">Currency code (e.g., "UAH").</param>
    /// <param name="recToken">Recurring payment token.</param>
    /// <param name="products">Products in the order.</param>
    /// <param name="client">Client information (optional).</param>
    /// <param name="serviceUrl">Callback URL (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Charge response with transaction details.</returns>
    Task<ChargeResponse> ChargeWithTokenAsync(
        string orderReference,
        decimal amount,
        string currency,
        string recToken,
        IEnumerable<Product> products,
        Client? client = null,
        string? serviceUrl = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refunds a transaction.
    /// </summary>
    /// <param name="orderReference">Original order reference.</param>
    /// <param name="amount">Amount to refund.</param>
    /// <param name="currency">Currency code.</param>
    /// <param name="comment">Refund reason/comment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Refund response with status.</returns>
    Task<RefundResponse> RefundAsync(
        string orderReference,
        decimal amount,
        string currency,
        string comment,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks the status of a transaction.
    /// </summary>
    /// <param name="orderReference">Order reference to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Transaction status response.</returns>
    Task<CheckStatusResponse> CheckStatusAsync(
        string orderReference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Settles (captures) a previously authorized transaction.
    /// Use this when MerchantTransactionType was AUTH to complete the payment.
    /// </summary>
    /// <param name="orderReference">Original order reference from authorization.</param>
    /// <param name="amount">Amount to settle (must be &lt;= authorized amount).</param>
    /// <param name="currency">Currency code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Settle response with status.</returns>
    Task<SettleResponse> SettleAsync(
        string orderReference,
        decimal amount,
        string currency,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Voids (cancels) a previously authorized transaction.
    /// Use this when MerchantTransactionType was AUTH to release the hold without capturing funds.
    /// </summary>
    /// <param name="orderReference">Original order reference from authorization.</param>
    /// <param name="amount">Amount to void.</param>
    /// <param name="currency">Currency code.</param>
    /// <param name="comment">Optional reason/comment for voiding.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Void response with status.</returns>
    Task<VoidResponse> VoidAsync(
        string orderReference,
        decimal amount,
        string currency,
        string? comment = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a purchase payment request (redirect flow).
    /// Returns a URL to redirect the client to for payment on WayForPay page.
    /// </summary>
    /// <param name="orderReference">Unique order identifier.</param>
    /// <param name="amount">Payment amount.</param>
    /// <param name="currency">Currency code (e.g., "UAH").</param>
    /// <param name="products">Products in the order.</param>
    /// <param name="client">Client information (optional).</param>
    /// <param name="returnUrl">URL to redirect after payment (optional).</param>
    /// <param name="serviceUrl">Callback URL for server notifications (optional).</param>
    /// <param name="language">Payment page language (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Purchase response with redirect URL.</returns>
    Task<PurchaseResponse> CreatePurchaseAsync(
        string orderReference,
        decimal amount,
        string currency,
        IEnumerable<Product> products,
        Client? client = null,
        string? returnUrl = null,
        string? serviceUrl = null,
        string? language = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an invoice for payment.
    /// Returns an invoice URL that can be sent to the client.
    /// </summary>
    /// <param name="orderReference">Unique order identifier.</param>
    /// <param name="amount">Payment amount.</param>
    /// <param name="currency">Currency code (e.g., "UAH").</param>
    /// <param name="products">Products in the order.</param>
    /// <param name="client">Client information (optional).</param>
    /// <param name="returnUrl">URL to redirect after payment (optional).</param>
    /// <param name="serviceUrl">Callback URL for server notifications (optional).</param>
    /// <param name="language">Payment page language (optional).</param>
    /// <param name="orderLifetime">Invoice lifetime in seconds (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Invoice response with payment URL.</returns>
    Task<InvoiceResponse> CreateInvoiceAsync(
        string orderReference,
        decimal amount,
        string currency,
        IEnumerable<Product> products,
        Client? client = null,
        string? returnUrl = null,
        string? serviceUrl = null,
        string? language = null,
        int? orderLifetime = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes 3D Secure authentication after user verification on bank's page.
    /// </summary>
    /// <param name="d3Md">Payment Authentication Request (MD) from bank.</param>
    /// <param name="d3Pares">Payment Authentication Response (PARes) from bank (Base64-encoded).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Complete 3DS response with final transaction status.</returns>
    Task<Complete3DSResponse> Complete3DSAsync(
        string d3Md,
        string d3Pares,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies a card without charging funds (creates a recToken for recurring payments).
    /// WayForPay performs a 0.01 UAH hold that is automatically reversed.
    /// </summary>
    /// <param name="orderReference">Unique order identifier.</param>
    /// <param name="card">Card details to verify.</param>
    /// <param name="client">Client information (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Verify response with recToken if successful.</returns>
    Task<VerifyResponse> VerifyAsync(
        string orderReference,
        Card card,
        Client? client = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of transactions for a given date range.
    /// </summary>
    /// <param name="dateBegin">Start date for transaction search.</param>
    /// <param name="dateEnd">End date for transaction search.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Transaction list response.</returns>
    Task<TransactionListResponse> GetTransactionListAsync(
        DateTimeOffset dateBegin,
        DateTimeOffset dateEnd,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Charges a card with recurring payment settings (for subscriptions).
    /// </summary>
    /// <param name="orderReference">Unique order identifier.</param>
    /// <param name="amount">Payment amount.</param>
    /// <param name="currency">Currency code (e.g., "UAH").</param>
    /// <param name="card">Card details for payment.</param>
    /// <param name="products">Products in the order.</param>
    /// <param name="regular">Recurring payment settings.</param>
    /// <param name="client">Client information (optional).</param>
    /// <param name="serviceUrl">Callback URL (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Charge response with transaction details.</returns>
    Task<ChargeResponse> ChargeWithRegularAsync(
        string orderReference,
        decimal amount,
        string currency,
        Card card,
        IEnumerable<Product> products,
        Regular regular,
        Client? client = null,
        string? serviceUrl = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a purchase with recurring payment settings (redirect flow with subscription).
    /// </summary>
    /// <param name="orderReference">Unique order identifier.</param>
    /// <param name="amount">Payment amount.</param>
    /// <param name="currency">Currency code (e.g., "UAH").</param>
    /// <param name="products">Products in the order.</param>
    /// <param name="regular">Recurring payment settings.</param>
    /// <param name="client">Client information (optional).</param>
    /// <param name="returnUrl">URL to redirect after payment (optional).</param>
    /// <param name="serviceUrl">Callback URL for server notifications (optional).</param>
    /// <param name="language">Payment page language (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Purchase response with redirect URL.</returns>
    Task<PurchaseResponse> CreatePurchaseWithRegularAsync(
        string orderReference,
        decimal amount,
        string currency,
        IEnumerable<Product> products,
        Regular regular,
        Client? client = null,
        string? returnUrl = null,
        string? serviceUrl = null,
        string? language = null,
        CancellationToken cancellationToken = default);
}
