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
    Task<RefundResponse> RefundAsync(
        string orderReference,
        decimal amount,
        string currency,
        string comment,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks the status of a transaction.
    /// </summary>
    Task<CheckStatusResponse> CheckStatusAsync(
        string orderReference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Settles (captures) a previously authorized transaction.
    /// Use this when MerchantTransactionType was AUTH to complete the payment.
    /// </summary>
    Task<SettleResponse> SettleAsync(
        string orderReference,
        decimal amount,
        string currency,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Voids (cancels) a previously authorized transaction.
    /// Use this when MerchantTransactionType was AUTH to release the hold without capturing funds.
    /// </summary>
    Task<VoidResponse> VoidAsync(
        string orderReference,
        decimal amount,
        string currency,
        string? comment = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a P2P credit transfer to a beneficiary card.
    /// </summary>
    Task<P2PCreditResponse> P2PCreditAsync(
        string orderReference,
        decimal amount,
        string currency,
        string cardBeneficiary,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a P2P transfer to a bank account (IBAN).
    /// </summary>
    Task<P2PAccountResponse> P2PAccountAsync(
        string orderReference,
        decimal amount,
        string currency,
        string iban,
        string okpo,
        string accountName,
        string description,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a purchase payment request (redirect flow).
    /// Returns a URL to redirect the client to for payment on WayForPay page.
    /// </summary>
    Task<PurchaseResponse> CreatePurchaseAsync(
        string orderReference,
        decimal amount,
        string currency,
        IEnumerable<Product> products,
        Client? client = null,
        string? returnUrl = null,
        string? serviceUrl = null,
        string? language = null,
        string? paymentSystems = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an invoice for payment.
    /// Returns an invoice URL that can be sent to the client.
    /// </summary>
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
        string? paymentSystems = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes 3D Secure authentication after user verification on bank's page.
    /// </summary>
    Task<Complete3DSResponse> Complete3DSAsync(
        string d3Md,
        string d3Pares,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies a card without charging funds (creates a recToken for recurring payments).
    /// WayForPay performs a 0.01 UAH hold that is automatically reversed.
    /// </summary>
    Task<VerifyResponse> VerifyAsync(
        string orderReference,
        Card card,
        Client? client = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of transactions for a given date range.
    /// </summary>
    Task<TransactionListResponse> GetTransactionListAsync(
        DateTimeOffset dateBegin,
        DateTimeOffset dateEnd,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Charges a card with recurring payment settings (for subscriptions).
    /// </summary>
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
        string? paymentSystems = null,
        CancellationToken cancellationToken = default);
}
