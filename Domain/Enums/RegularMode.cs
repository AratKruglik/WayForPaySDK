namespace WayForPaySDK.Domain.Enums;

/// <summary>
/// Regular payment frequency mode.
/// </summary>
public enum RegularMode
{
    /// <summary>
    /// Client-defined schedule.
    /// </summary>
    Client,

    /// <summary>
    /// No regular payments.
    /// </summary>
    None,

    /// <summary>
    /// One-time regular payment.
    /// </summary>
    Once,

    /// <summary>
    /// Daily payments.
    /// </summary>
    Daily,

    /// <summary>
    /// Weekly payments.
    /// </summary>
    Weekly,

    /// <summary>
    /// Monthly payments.
    /// </summary>
    Monthly,

    /// <summary>
    /// Quarterly payments (every 3 months).
    /// </summary>
    Quarterly,

    /// <summary>
    /// Half-yearly payments (every 6 months).
    /// </summary>
    HalfYearly,

    /// <summary>
    /// Yearly payments.
    /// </summary>
    Yearly
}
