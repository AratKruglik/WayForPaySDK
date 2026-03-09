using System.Text.Json;
using FluentAssertions;
using WayForPaySDK.Responses;
using WayForPaySDK.Serialization;

namespace WayForPaySDK.Tests.Unit;

public class MmsResponseTests
{
    private static readonly JsonSerializerOptions JsonOptions = WayForPayJsonContext.Default.Options;

    [Theory]
    [InlineData(1100, true)]
    [InlineData(1101, false)]
    [InlineData(4100, false)]
    [InlineData(0, false)]
    public void AddPartnerResponse_IsSuccess_ReturnsExpected(int reasonCode, bool expected)
    {
        var json = $$"""{"reasonCode":{{reasonCode}},"reason":"test"}""";

        var response = JsonSerializer.Deserialize<AddPartnerResponse>(json, JsonOptions)!;

        response.IsSuccess.Should().Be(expected);
    }

    [Fact]
    public void AddPartnerResponse_Deserialize_MapsAllProperties()
    {
        const string json = """{"reasonCode":1100,"reason":"Ok","partnerCode":"seller-001"}""";

        var response = JsonSerializer.Deserialize<AddPartnerResponse>(json, JsonOptions)!;

        response.ReasonCode.Should().Be(1100);
        response.ReasonMessage.Should().Be("Ok");
        response.PartnerCode.Should().Be("seller-001");
        response.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void PartnerInfoResponse_Deserialize_MapsAllProperties()
    {
        const string json = """
            {
                "reasonCode": 1100,
                "reason": "Ok",
                "merchantAccount": "merch",
                "partnerCode": "seller-001",
                "site": "https://example.com",
                "phone": "+380501234567",
                "email": "test@test.com",
                "compensation": "card",
                "partnerStatus": "Active",
                "createDate": "01.01.2025"
            }
            """;

        var response = JsonSerializer.Deserialize<PartnerInfoResponse>(json, JsonOptions)!;

        response.ReasonCode.Should().Be(1100);
        response.ReasonMessage.Should().Be("Ok");
        response.MerchantAccount.Should().Be("merch");
        response.PartnerCode.Should().Be("seller-001");
        response.Site.Should().Be("https://example.com");
        response.Phone.Should().Be("+380501234567");
        response.Email.Should().Be("test@test.com");
        response.Compensation.Should().Be("card");
        response.PartnerStatus.Should().Be("Active");
        response.CreateDate.Should().Be("01.01.2025");
        response.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(1100, true)]
    [InlineData(1101, false)]
    public void PartnerInfoResponse_IsSuccess_ReturnsExpected(int reasonCode, bool expected)
    {
        var json = $$"""{"reasonCode":{{reasonCode}},"reason":"test"}""";

        var response = JsonSerializer.Deserialize<PartnerInfoResponse>(json, JsonOptions)!;

        response.IsSuccess.Should().Be(expected);
    }

    [Fact]
    public void UpdatePartnerResponse_Deserialize_MapsAllProperties()
    {
        const string json = """{"reasonCode":1100,"reason":"update","partnerCode":"seller-001","secretKey":"abc123"}""";

        var response = JsonSerializer.Deserialize<UpdatePartnerResponse>(json, JsonOptions)!;

        response.ReasonCode.Should().Be(1100);
        response.ReasonMessage.Should().Be("update");
        response.PartnerCode.Should().Be("seller-001");
        response.SecretKey.Should().Be("abc123");
        response.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(1100, true)]
    [InlineData(4100, false)]
    public void UpdatePartnerResponse_IsSuccess_ReturnsExpected(int reasonCode, bool expected)
    {
        var json = $$"""{"reasonCode":{{reasonCode}},"reason":"test"}""";

        var response = JsonSerializer.Deserialize<UpdatePartnerResponse>(json, JsonOptions)!;

        response.IsSuccess.Should().Be(expected);
    }

    [Fact]
    public void AddMerchantResponse_Deserialize_MapsAllProperties()
    {
        const string json = """{"reasonCode":1100,"reason":"Ok","merchantAccount":"shop_123","secretKey":"key123"}""";

        var response = JsonSerializer.Deserialize<AddMerchantResponse>(json, JsonOptions)!;

        response.ReasonCode.Should().Be(1100);
        response.ReasonMessage.Should().Be("Ok");
        response.MerchantAccount.Should().Be("shop_123");
        response.SecretKey.Should().Be("key123");
        response.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(1100, true)]
    [InlineData(1101, false)]
    public void AddMerchantResponse_IsSuccess_ReturnsExpected(int reasonCode, bool expected)
    {
        var json = $$"""{"reasonCode":{{reasonCode}},"reason":"test"}""";

        var response = JsonSerializer.Deserialize<AddMerchantResponse>(json, JsonOptions)!;

        response.IsSuccess.Should().Be(expected);
    }

    [Fact]
    public void MerchantInfoResponse_Deserialize_MapsAllProperties()
    {
        const string json = """
            {
                "reasonCode": 1100,
                "reason": "Ok",
                "merchantAccount": "shop_123",
                "site": "https://shop.com",
                "phone": "+380501234567",
                "email": "shop@test.com",
                "compensation": "account",
                "status": "Active",
                "createDate": "01.01.2025"
            }
            """;

        var response = JsonSerializer.Deserialize<MerchantInfoResponse>(json, JsonOptions)!;

        response.ReasonCode.Should().Be(1100);
        response.ReasonMessage.Should().Be("Ok");
        response.MerchantAccount.Should().Be("shop_123");
        response.Site.Should().Be("https://shop.com");
        response.Phone.Should().Be("+380501234567");
        response.Email.Should().Be("shop@test.com");
        response.Compensation.Should().Be("account");
        response.Status.Should().Be("Active");
        response.CreateDate.Should().Be("01.01.2025");
        response.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(1100, true)]
    [InlineData(4100, false)]
    public void MerchantInfoResponse_IsSuccess_ReturnsExpected(int reasonCode, bool expected)
    {
        var json = $$"""{"reasonCode":{{reasonCode}},"reason":"test"}""";

        var response = JsonSerializer.Deserialize<MerchantInfoResponse>(json, JsonOptions)!;

        response.IsSuccess.Should().Be(expected);
    }

    [Fact]
    public void MerchantBalanceResponse_Deserialize_MapsAllProperties()
    {
        const string json = """{"reasonCode":1100,"reason":"Ok","merchantAccount":"shop_123","balance_UAH":1523.45}""";

        var response = JsonSerializer.Deserialize<MerchantBalanceResponse>(json, JsonOptions)!;

        response.ReasonCode.Should().Be(1100);
        response.ReasonMessage.Should().Be("Ok");
        response.MerchantAccount.Should().Be("shop_123");
        response.BalanceUah.Should().Be(1523.45m);
        response.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(1100, true)]
    [InlineData(1101, false)]
    public void MerchantBalanceResponse_IsSuccess_ReturnsExpected(int reasonCode, bool expected)
    {
        var json = $$"""{"reasonCode":{{reasonCode}},"reason":"test"}""";

        var response = JsonSerializer.Deserialize<MerchantBalanceResponse>(json, JsonOptions)!;

        response.IsSuccess.Should().Be(expected);
    }
}
