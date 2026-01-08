using System.ComponentModel.DataAnnotations;

namespace WayForPaySDK.Options;

/// <summary>
/// Configuration options for WayForPay SDK.
/// </summary>
public sealed class WayForPayOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "WayForPay";

    /// <summary>
    /// Gets or sets the merchant account identifier.
    /// </summary>
    [Required]
    public required string MerchantAccount { get; set; }

    /// <summary>
    /// Gets or sets the merchant domain name.
    /// </summary>
    [Required]
    public required string MerchantDomainName { get; set; }

    /// <summary>
    /// Gets or sets the merchant secret key for signing requests.
    /// </summary>
    [Required]
    public required string MerchantSecretKey { get; set; }

    /// <summary>
    /// Gets or sets the API base URL. Defaults to production URL.
    /// </summary>
    public string ApiBaseUrl { get; set; } = "https://api.wayforpay.com/api";

    /// <summary>
    /// Gets or sets the timeout for API requests in seconds. Defaults to 30 seconds.
    /// </summary>
    [Range(1, 300)]
    public int TimeoutSeconds { get; set; } = 30;
}
