using Microsoft.Extensions.Options;

namespace WayForPaySDK.Options;

/// <summary>
/// Validates WayForPayOptions configuration values at startup.
/// </summary>
public sealed class WayForPayOptionsValidator : IValidateOptions<WayForPayOptions>
{
    public ValidateOptionsResult Validate(string? name, WayForPayOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.MerchantAccount))
        {
            failures.Add("MerchantAccount is required and cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(options.MerchantDomainName))
        {
            failures.Add("MerchantDomainName is required and cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(options.MerchantSecretKey))
        {
            failures.Add("MerchantSecretKey is required and cannot be empty.");
        }

        if (options.TimeoutSeconds < 1 || options.TimeoutSeconds > 300)
        {
            failures.Add("TimeoutSeconds must be between 1 and 300 seconds.");
        }

        if (string.IsNullOrWhiteSpace(options.ApiBaseUrl))
        {
            failures.Add("ApiBaseUrl is required and cannot be empty.");
        }
        else if (!Uri.TryCreate(options.ApiBaseUrl, UriKind.Absolute, out var uri))
        {
            failures.Add("ApiBaseUrl must be a valid absolute URL.");
        }
        else if (uri.Scheme != "https" && uri.Scheme != "http")
        {
            failures.Add("ApiBaseUrl must use HTTP or HTTPS protocol.");
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
