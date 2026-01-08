namespace WayForPaySDK.Domain.Enums;

/// <summary>
/// Supported payment systems in WayForPay.
/// </summary>
public enum PaymentSystem
{
    /// <summary>
    /// Credit/debit card payment.
    /// </summary>
    Card,

    /// <summary>
    /// Privat24 online banking.
    /// </summary>
    Privat24,

    /// <summary>
    /// LiqPay terminal.
    /// </summary>
    LpTerminal,

    /// <summary>
    /// Bitcoin payment.
    /// </summary>
    Btc,

    /// <summary>
    /// Bank cash payment.
    /// </summary>
    BankCash,

    /// <summary>
    /// Credit payment (installments).
    /// </summary>
    Credit,

    /// <summary>
    /// Pay in parts (PrivatBank).
    /// </summary>
    PayParts,

    /// <summary>
    /// QR code payment.
    /// </summary>
    QrCode,

    /// <summary>
    /// Masterpass digital wallet.
    /// </summary>
    MasterPass,

    /// <summary>
    /// Visa Checkout digital wallet.
    /// </summary>
    VisaCheckout,

    /// <summary>
    /// Google Pay.
    /// </summary>
    GooglePay,

    /// <summary>
    /// Apple Pay.
    /// </summary>
    ApplePay,

    /// <summary>
    /// Pay in parts (Monobank).
    /// </summary>
    PayPartsMono
}
