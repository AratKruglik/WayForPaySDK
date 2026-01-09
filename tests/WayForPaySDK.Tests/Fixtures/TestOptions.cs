using Microsoft.Extensions.Options;
using WayForPaySDK.Options;

namespace WayForPaySDK.Tests.Fixtures;

public static class TestOptions
{
    public const string TestMerchantAccount = "test_merchant";
    public const string TestMerchantDomainName = "test.example.com";
    public const string TestMerchantSecretKey = "test_secret_key_12345678901234567890";
    public const string TestApiBaseUrl = "https://api.wayforpay.test";

    public static WayForPayOptions Create() => new()
    {
        MerchantAccount = TestMerchantAccount,
        MerchantDomainName = TestMerchantDomainName,
        MerchantSecretKey = TestMerchantSecretKey,
        ApiBaseUrl = TestApiBaseUrl,
        TimeoutSeconds = 30
    };

    public static WayForPayOptions CreateWithCustomSecret(string secretKey) => new()
    {
        MerchantAccount = TestMerchantAccount,
        MerchantDomainName = TestMerchantDomainName,
        MerchantSecretKey = secretKey,
        ApiBaseUrl = TestApiBaseUrl,
        TimeoutSeconds = 30
    };

    public static IOptions<WayForPayOptions> CreateOptions() =>
        Microsoft.Extensions.Options.Options.Create(Create());

    public static IOptions<WayForPayOptions> CreateOptionsWithCustomSecret(string secretKey) =>
        Microsoft.Extensions.Options.Options.Create(CreateWithCustomSecret(secretKey));
}
