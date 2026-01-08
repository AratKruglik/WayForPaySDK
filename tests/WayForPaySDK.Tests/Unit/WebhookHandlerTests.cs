using System.Text;
using System.Text.Json;
using FluentAssertions;
using WayForPaySDK.Crypto;
using WayForPaySDK.Exceptions;
using WayForPaySDK.Handlers;
using WayForPaySDK.Serialization;
using WayForPaySDK.Tests.Fixtures;

namespace WayForPaySDK.Tests.Unit;

public class WebhookHandlerTests
{
    private readonly WebhookHandler _sut;
    private readonly SignatureGenerator _signatureGenerator;
    private readonly JsonSerializerOptions _jsonOptions;

    public WebhookHandlerTests()
    {
        _signatureGenerator = new SignatureGenerator(TestOptions.CreateOptions());
        _sut = new WebhookHandler(_signatureGenerator);
        _jsonOptions = new JsonSerializerOptions
        {
            TypeInfoResolver = WayForPayJsonContext.Default
        };
    }

    [Fact]
    public async Task ParseAsync_WithValidPayload_ReturnsWebhookPayload()
    {
        // Arrange
        var payload = CreateValidWebhookPayload();
        var json = JsonSerializer.Serialize(payload, _jsonOptions);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var result = await _sut.ParseAsync(stream);

        // Assert
        result.Should().NotBeNull();
        result.OrderReference.Should().Be(payload.OrderReference);
        result.Amount.Should().Be(payload.Amount);
    }

    [Fact]
    public async Task ParseAsync_WithInvalidSignature_ThrowsSignatureException()
    {
        // Arrange
        var payload = CreateValidWebhookPayload() with { MerchantSignature = "invalid_signature" };
        var json = JsonSerializer.Serialize(payload, _jsonOptions);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var act = async () => await _sut.ParseAsync(stream);

        // Assert
        await act.Should().ThrowAsync<SignatureException>();
    }

    [Fact]
    public async Task ParseAsync_WithEmptyStream_ThrowsJsonException()
    {
        // Arrange
        var stream = new MemoryStream();

        // Act
        var act = async () => await _sut.ParseAsync(stream);

        // Assert
        // Empty stream causes deserialization to return null, which throws JsonParseException
        await act.Should().ThrowAsync<JsonException>();
    }

    [Fact]
    public async Task ParseAsync_WithMalformedJson_ThrowsJsonException()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("{ invalid json "));

        // Act
        var act = async () => await _sut.ParseAsync(stream);

        // Assert
        await act.Should().ThrowAsync<JsonException>();
    }

    [Fact]
    public void Parse_WithValidJson_ReturnsWebhookPayload()
    {
        // Arrange
        var payload = CreateValidWebhookPayload();
        var json = JsonSerializer.Serialize(payload, _jsonOptions);

        // Act
        var result = _sut.Parse(json);

        // Assert
        result.Should().NotBeNull();
        result.OrderReference.Should().Be(payload.OrderReference);
    }

    [Fact]
    public void Parse_WithInvalidSignature_ThrowsSignatureException()
    {
        // Arrange
        var payload = CreateValidWebhookPayload() with { MerchantSignature = "invalid" };
        var json = JsonSerializer.Serialize(payload, _jsonOptions);

        // Act
        var act = () => _sut.Parse(json);

        // Assert
        act.Should().Throw<SignatureException>();
    }

    [Fact]
    public void Parse_WithNullJson_ThrowsArgumentException()
    {
        // Act
        var act = () => _sut.Parse(null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Parse_WithEmptyJson_ThrowsArgumentException()
    {
        // Act
        var act = () => _sut.Parse(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateResponse_WithAcceptStatus_ReturnsAcceptResponse()
    {
        // Arrange
        var payload = CreateValidWebhookPayload();

        // Act
        var response = _sut.CreateResponse(payload, WebhookStatus.Accept);

        // Assert
        response.Should().NotBeNull();
        response.OrderReference.Should().Be(payload.OrderReference);
        response.Status.Should().Be("accept");
        response.Time.Should().BeGreaterThan(0);
        response.Signature.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void CreateResponse_WithDeclineStatus_ReturnsDeclineResponse()
    {
        // Arrange
        var payload = CreateValidWebhookPayload();

        // Act
        var response = _sut.CreateResponse(payload, WebhookStatus.Decline);

        // Assert
        response.Status.Should().Be("decline");
    }

    [Fact]
    public void CreateResponse_WithValidSignature_CanBeVerified()
    {
        // Arrange
        var payload = CreateValidWebhookPayload();

        // Act
        var response = _sut.CreateResponse(payload, WebhookStatus.Accept);

        // Assert
        var expectedSignature = _signatureGenerator.GenerateSignature(new[]
        {
            response.OrderReference,
            response.Status,
            response.Time.ToString()
        });

        _signatureGenerator.VerifySignature(expectedSignature, response.Signature).Should().BeTrue();
    }

    [Fact]
    public void CreateResponse_WithNullPayload_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _sut.CreateResponse(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SerializeResponse_WithValidResponse_ReturnsJson()
    {
        // Arrange
        var payload = CreateValidWebhookPayload();
        var response = _sut.CreateResponse(payload);

        // Act
        var json = _sut.SerializeResponse(response);

        // Assert
        json.Should().NotBeNullOrWhiteSpace();
        json.Should().Contain("orderReference");
        json.Should().Contain(response.OrderReference);
    }

    [Fact]
    public void SerializeResponse_WithNullResponse_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _sut.SerializeResponse(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WebhookPayload_HelperProperties_WorkCorrectly()
    {
        // Test all helper properties
        var approvedPayload = CreateWebhookPayloadWithStatus("Approved", 1100);
        approvedPayload.IsSuccess.Should().BeTrue();
        approvedPayload.IsApproved.Should().BeTrue();
        approvedPayload.IsDeclined.Should().BeFalse();

        var declinedPayload = CreateWebhookPayloadWithStatus("Declined", 1101);
        declinedPayload.IsSuccess.Should().BeFalse();
        declinedPayload.IsApproved.Should().BeFalse();
        declinedPayload.IsDeclined.Should().BeTrue();

        var refundedPayload = CreateWebhookPayloadWithStatus("Refunded", 1100);
        refundedPayload.IsRefunded.Should().BeTrue();

        var inProcessingPayload = CreateWebhookPayloadWithStatus("InProcessing", 1100);
        inProcessingPayload.IsInProcessing.Should().BeTrue();

        var voidedPayload = CreateWebhookPayloadWithStatus("Voided", 1100);
        voidedPayload.IsVoided.Should().BeTrue();
    }

    private WebhookPayload CreateValidWebhookPayload()
    {
        var merchantAccount = TestOptions.TestMerchantAccount;
        var orderReference = "ORDER123";
        var amount = 100.50m;
        var currency = "UAH";
        var authCode = "AUTH123";
        var cardPan = "411111******1111";
        var transactionStatus = "Approved";
        var reasonCode = 1100;

        // Calculate valid signature
        var signatureFields = new[]
        {
            merchantAccount,
            orderReference,
            amount.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture),
            currency,
            authCode,
            cardPan,
            transactionStatus,
            reasonCode.ToString(System.Globalization.CultureInfo.InvariantCulture)
        };

        var signature = _signatureGenerator.GenerateSignature(signatureFields);

        return new WebhookPayload
        {
            MerchantAccount = merchantAccount,
            OrderReference = orderReference,
            Amount = amount,
            Currency = currency,
            AuthCode = authCode,
            CardPan = cardPan,
            TransactionStatus = transactionStatus,
            ReasonCode = reasonCode,
            MerchantSignature = signature
        };
    }

    private WebhookPayload CreateWebhookPayloadWithStatus(string status, int reasonCode)
    {
        var basePayload = CreateValidWebhookPayload();

        // Recalculate signature with new values
        var signatureFields = new[]
        {
            basePayload.MerchantAccount,
            basePayload.OrderReference,
            basePayload.Amount.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture),
            basePayload.Currency,
            basePayload.AuthCode ?? string.Empty,
            basePayload.CardPan ?? string.Empty,
            status,
            reasonCode.ToString(System.Globalization.CultureInfo.InvariantCulture)
        };

        var signature = _signatureGenerator.GenerateSignature(signatureFields);

        return basePayload with
        {
            TransactionStatus = status,
            ReasonCode = reasonCode,
            MerchantSignature = signature
        };
    }
}
