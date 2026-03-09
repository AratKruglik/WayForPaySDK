using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using WayForPaySDK.Crypto;
using WayForPaySDK.Exceptions;
using WayForPaySDK.Options;
using WayForPaySDK.Responses;
using WayForPaySDK.Services;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WayForPaySDK.Tests.Unit;

public class MmsClientTests : IDisposable
{
    private readonly WireMockServer _server;
    private readonly Mock<ISignatureGenerator> _signatureGeneratorMock;
    private readonly MmsClient _client;

    public MmsClientTests()
    {
        _server = WireMockServer.Start();

        var options = Microsoft.Extensions.Options.Options.Create(new WayForPayOptions
        {
            MerchantAccount = "test",
            MerchantDomainName = "test.com",
            MerchantSecretKey = "secret",
            ApiBaseUrl = $"{_server.Url}/api",
            AllowInsecureHttp = true
        });

        _signatureGeneratorMock = new Mock<ISignatureGenerator>();
        _signatureGeneratorMock
            .Setup(x => x.GenerateSignature(It.IsAny<IEnumerable<string>>()))
            .Returns("test_signature");

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri($"{_server.Url}/api")
        };

        _client = new MmsClient(httpClient, options, _signatureGeneratorMock.Object);
    }

    public void Dispose()
    {
        _server.Stop();
        _server.Dispose();
    }

    [Fact]
    public async Task AddPartnerAsync_SendsRequestToCorrectUrl()
    {
        _server
            .Given(Request.Create().WithPath("/mms/addPartner.php").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"reasonCode":1100,"reason":"Ok","partnerCode":"seller-001"}"""));

        var result = await _client.AddPartnerAsync("seller-001", "https://site.com", "+380501234567", "test@test.com");

        result.Should().NotBeNull();
        result.PartnerCode.Should().Be("seller-001");
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task AddPartnerAsync_GeneratesSignatureWithCorrectFields()
    {
        _server
            .Given(Request.Create().WithPath("/mms/addPartner.php").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"reasonCode":1100,"reason":"Ok","partnerCode":"seller-001"}"""));

        await _client.AddPartnerAsync("seller-001", "https://site.com", "+380501234567", "test@test.com");

        _signatureGeneratorMock.Verify(
            x => x.GenerateSignature(It.Is<IEnumerable<string>>(fields =>
                fields.SequenceEqual(new[] { "test", "seller-001", "+380501234567", "test@test.com" }))),
            Times.Once);
    }

    [Fact]
    public async Task GetMerchantBalanceAsync_SendsRequestToCorrectUrl()
    {
        _server
            .Given(Request.Create().WithPath("/mms/merchantBalance.php").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"reasonCode":1100,"reason":"Ok","merchantAccount":"test","balance_UAH":1523.45}"""));

        var result = await _client.GetMerchantBalanceAsync();

        result.Should().NotBeNull();
        result.MerchantAccount.Should().Be("test");
        result.BalanceUah.Should().Be(1523.45m);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetMerchantBalanceAsync_GeneratesSignatureWithCorrectFields()
    {
        _server
            .Given(Request.Create().WithPath("/mms/merchantBalance.php").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"reasonCode":1100,"reason":"Ok","merchantAccount":"test","balance_UAH":0}"""));

        await _client.GetMerchantBalanceAsync();

        _signatureGeneratorMock.Verify(
            x => x.GenerateSignature(It.Is<IEnumerable<string>>(fields =>
                fields.SequenceEqual(new[] { "test" }))),
            Times.Once);
    }

    [Fact]
    public async Task SendMmsRequest_HttpError_ThrowsApiException()
    {
        _server
            .Given(Request.Create().WithPath("/mms/addPartner.php").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(500)
                .WithBody("Internal Server Error"));

        var act = () => _client.AddPartnerAsync("p1", "https://site.com", "+380501234567", "test@test.com");

        await act.Should().ThrowAsync<ApiException>();
    }

    [Fact]
    public async Task SendMmsRequest_InvalidJson_ThrowsJsonParseException()
    {
        _server
            .Given(Request.Create().WithPath("/mms/merchantBalance.php").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("not-valid-json"));

        var act = () => _client.GetMerchantBalanceAsync();

        await act.Should().ThrowAsync<JsonParseException>();
    }

    [Fact]
    public async Task SendMmsRequest_NullResponse_ThrowsJsonParseException()
    {
        _server
            .Given(Request.Create().WithPath("/mms/merchantBalance.php").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("null"));

        var act = () => _client.GetMerchantBalanceAsync();

        await act.Should().ThrowAsync<JsonParseException>();
    }

    [Fact]
    public async Task GetPartnerInfoAsync_SendsRequestToCorrectUrl()
    {
        _server
            .Given(Request.Create().WithPath("/mms/partnerInfo.php").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"reasonCode":1100,"reason":"Ok","partnerCode":"seller-001"}"""));

        var result = await _client.GetPartnerInfoAsync("seller-001");

        result.Should().NotBeNull();
        result.PartnerCode.Should().Be("seller-001");
    }

    [Fact]
    public async Task GetMerchantInfoAsync_SendsRequestToCorrectUrl()
    {
        _server
            .Given(Request.Create().WithPath("/mms/merchantInfo.php").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"reasonCode":1100,"reason":"Ok","merchantAccount":"shop_123","status":"Active"}"""));

        var result = await _client.GetMerchantInfoAsync();

        result.Should().NotBeNull();
        result.MerchantAccount.Should().Be("shop_123");
        result.Status.Should().Be("Active");
    }
}
