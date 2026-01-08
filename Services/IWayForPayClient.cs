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
}
