namespace WayForPaySDK.Domain.Enums;

/// <summary>
/// Merchant transaction type (authorization mode).
/// </summary>
public enum MerchantTransactionType
{
    /// <summary>
    /// Direct sale - funds are captured immediately.
    /// </summary>
    Sale,

    /// <summary>
    /// Authorization only - funds are held but not captured.
    /// Requires a separate capture (settle) request.
    /// </summary>
    Auth
}
