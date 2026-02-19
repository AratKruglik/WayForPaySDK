using FluentAssertions;
using WayForPaySDK.Options;

namespace WayForPaySDK.Tests.Unit;

public class WayForPayOptionsValidatorTests
{
    private readonly WayForPayOptionsValidator _sut = new();

    [Fact]
    public void Validate_WithValidOptions_ReturnsSuccess()
    {
        var options = new WayForPayOptions
        {
            MerchantAccount = "test_merchant",
            MerchantDomainName = "test.example.com",
            MerchantSecretKey = "secret_key_12345",
            ApiBaseUrl = "https://api.wayforpay.com/api",
            TimeoutSeconds = 30
        };

        var result = _sut.Validate(null, options);

        result.Succeeded.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WithEmptyMerchantAccount_ReturnsFail(string? merchantAccount)
    {
        var options = new WayForPayOptions
        {
            MerchantAccount = merchantAccount!,
            MerchantDomainName = "test.example.com",
            MerchantSecretKey = "secret_key_12345",
            ApiBaseUrl = "https://api.wayforpay.com/api"
        };

        var result = _sut.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("MerchantAccount");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WithEmptyMerchantDomainName_ReturnsFail(string? domainName)
    {
        var options = new WayForPayOptions
        {
            MerchantAccount = "test_merchant",
            MerchantDomainName = domainName!,
            MerchantSecretKey = "secret_key_12345",
            ApiBaseUrl = "https://api.wayforpay.com/api"
        };

        var result = _sut.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("MerchantDomainName");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WithEmptyMerchantSecretKey_ReturnsFail(string? secretKey)
    {
        var options = new WayForPayOptions
        {
            MerchantAccount = "test_merchant",
            MerchantDomainName = "test.example.com",
            MerchantSecretKey = secretKey!,
            ApiBaseUrl = "https://api.wayforpay.com/api"
        };

        var result = _sut.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("MerchantSecretKey");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(301)]
    [InlineData(1000)]
    public void Validate_WithInvalidTimeoutSeconds_ReturnsFail(int timeout)
    {
        var options = new WayForPayOptions
        {
            MerchantAccount = "test_merchant",
            MerchantDomainName = "test.example.com",
            MerchantSecretKey = "secret_key_12345",
            ApiBaseUrl = "https://api.wayforpay.com/api",
            TimeoutSeconds = timeout
        };

        var result = _sut.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("TimeoutSeconds");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(300)]
    public void Validate_WithValidTimeoutSeconds_ReturnsSuccess(int timeout)
    {
        var options = new WayForPayOptions
        {
            MerchantAccount = "test_merchant",
            MerchantDomainName = "test.example.com",
            MerchantSecretKey = "secret_key_12345",
            ApiBaseUrl = "https://api.wayforpay.com/api",
            TimeoutSeconds = timeout
        };

        var result = _sut.Validate(null, options);

        result.Succeeded.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WithEmptyApiBaseUrl_ReturnsFail(string? apiUrl)
    {
        var options = new WayForPayOptions
        {
            MerchantAccount = "test_merchant",
            MerchantDomainName = "test.example.com",
            MerchantSecretKey = "secret_key_12345",
            ApiBaseUrl = apiUrl!
        };

        var result = _sut.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("ApiBaseUrl");
    }

    [Theory]
    [InlineData("invalid-url")]
    [InlineData("not-a-url")]
    [InlineData("/relative/path")]
    public void Validate_WithInvalidApiBaseUrl_ReturnsFail(string apiUrl)
    {
        var options = new WayForPayOptions
        {
            MerchantAccount = "test_merchant",
            MerchantDomainName = "test.example.com",
            MerchantSecretKey = "secret_key_12345",
            ApiBaseUrl = apiUrl
        };

        var result = _sut.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("ApiBaseUrl");
    }

    [Theory]
    [InlineData("ftp://api.wayforpay.com/api")]
    [InlineData("file:///path/to/file")]
    public void Validate_WithNonHttpProtocol_ReturnsFail(string apiUrl)
    {
        var options = new WayForPayOptions
        {
            MerchantAccount = "test_merchant",
            MerchantDomainName = "test.example.com",
            MerchantSecretKey = "secret_key_12345",
            ApiBaseUrl = apiUrl
        };

        var result = _sut.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("HTTP or HTTPS");
    }

    [Theory]
    [InlineData("https://api.wayforpay.com/api")]
    [InlineData("https://test.wayforpay.com/api")]
    public void Validate_WithValidApiBaseUrl_ReturnsSuccess(string apiUrl)
    {
        var options = new WayForPayOptions
        {
            MerchantAccount = "test_merchant",
            MerchantDomainName = "test.example.com",
            MerchantSecretKey = "secret_key_12345",
            ApiBaseUrl = apiUrl
        };

        var result = _sut.Validate(null, options);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithHttpUrl_ReturnsFail()
    {
        var options = new WayForPayOptions
        {
            MerchantAccount = "test_merchant",
            MerchantDomainName = "test.example.com",
            MerchantSecretKey = "secret_key_12345",
            ApiBaseUrl = "http://api.wayforpay.com/api"
        };

        var result = _sut.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("insecure");
    }

    [Fact]
    public void Validate_WithHttpUrl_AndAllowInsecureHttp_ReturnsSuccess()
    {
        var options = new WayForPayOptions
        {
            MerchantAccount = "test_merchant",
            MerchantDomainName = "test.example.com",
            MerchantSecretKey = "secret_key_12345",
            ApiBaseUrl = "http://localhost:5000/api",
            AllowInsecureHttp = true
        };

        var result = _sut.Validate(null, options);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithMultipleErrors_ReturnsAllErrors()
    {
        var options = new WayForPayOptions
        {
            MerchantAccount = "",
            MerchantDomainName = "",
            MerchantSecretKey = "",
            ApiBaseUrl = "invalid",
            TimeoutSeconds = 0
        };

        var result = _sut.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("MerchantAccount");
        result.FailureMessage.Should().Contain("MerchantDomainName");
        result.FailureMessage.Should().Contain("MerchantSecretKey");
        result.FailureMessage.Should().Contain("TimeoutSeconds");
        result.FailureMessage.Should().Contain("ApiBaseUrl");
    }
}
