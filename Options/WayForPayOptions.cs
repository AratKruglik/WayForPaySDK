using System.ComponentModel.DataAnnotations;

namespace WayForPaySDK.Options;

public sealed class WayForPayOptions
{
    public const string SectionName = "WayForPay";

    [Required]
    public required string MerchantAccount { get; set; }

    [Required]
    public required string MerchantDomainName { get; set; }

    [Required]
    public required string MerchantSecretKey { get; set; }

    public string ApiBaseUrl { get; set; } = "https://api.wayforpay.com/api";

    [Range(1, 300)]
    public int TimeoutSeconds { get; set; } = 30;
}
