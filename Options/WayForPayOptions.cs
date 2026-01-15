using System.ComponentModel.DataAnnotations;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace WayForPaySDK.Options;

/// <summary>
/// Configuration options for WayForPay SDK.
/// </summary>
public sealed class WayForPayOptions
{
    /// <summary>
    /// The configuration section name for binding from appsettings.
    /// </summary>
    public const string SectionName = "WayForPay";

    /// <summary>
    /// The merchant account identifier provided by WayForPay.
    /// </summary>
    [Required]
    public required string MerchantAccount { get; set; }

    /// <summary>
    /// The merchant domain name registered with WayForPay.
    /// </summary>
    [Required]
    public required string MerchantDomainName { get; set; }

    /// <summary>
    /// The merchant's secret key for generating signatures and authenticating requests.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <strong>Security Note:</strong> This key is used for HMAC-MD5 signature generation and is sent
    /// directly to WayForPay's Regular API endpoints. Store this value securely using environment
    /// variables, Azure Key Vault, or other secure configuration providers. Never commit this value
    /// to source control.
    /// </para>
    /// </remarks>
    [Required]
    public required string MerchantSecretKey { get; set; }

    /// <summary>
    /// The base URL for WayForPay API. Defaults to the production API endpoint.
    /// </summary>
    public string ApiBaseUrl { get; set; } = "https://api.wayforpay.com/api";

    /// <summary>
    /// The timeout in seconds for HTTP requests. Must be between 1 and 300 seconds.
    /// </summary>
    [Range(1, 300)]
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Optional callback for custom server certificate validation.
    /// Use this for certificate pinning or custom validation logic.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <strong>Security Note:</strong> Only set this if you need custom certificate validation.
    /// When null (default), standard .NET certificate validation is used.
    /// </para>
    /// <para>
    /// Example for certificate pinning:
    /// <code>
    /// options.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
    /// {
    ///     if (cert == null) return false;
    ///     var expectedThumbprint = "YOUR_EXPECTED_THUMBPRINT";
    ///     return string.Equals(cert.GetCertHashString(), expectedThumbprint, StringComparison.OrdinalIgnoreCase);
    /// };
    /// </code>
    /// </para>
    /// </remarks>
    public Func<HttpRequestMessage, X509Certificate2?, X509Chain?, SslPolicyErrors, bool>?
        ServerCertificateCustomValidationCallback { get; set; }
}
