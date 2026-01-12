using FluentAssertions;
using WayForPaySDK.Requests;
using WayForPaySDK.Tests.Fixtures;

namespace WayForPaySDK.Tests.Unit;

public class P2PTests
{
    [Fact]
    public void P2PCreditRequest_GetSignatureFields_ReturnsCorrectFields()
    {
        // Arrange
        var request = new P2PCreditRequest
        {
            MerchantAccount = "test_merchant",
            MerchantSignature = "",
            OrderReference = "ORDER123",
            Amount = 100.50m,
            Currency = "UAH",
            CardBeneficiary = "4111111111111111"
        };

        // Act
        var fields = request.GetSignatureFields().ToList();

        // Assert
        fields.Should().HaveCount(5);
        fields[0].Should().Be("test_merchant");
        fields[1].Should().Be("ORDER123");
        fields[2].Should().Be("100.5"); // Decimal formatting
        fields[3].Should().Be("UAH");
        fields[4].Should().Be("4111111111111111");
    }

    [Fact]
    public void P2PAccountRequest_GetSignatureFields_ReturnsCorrectFields()
    {
        // Arrange
        var request = new P2PAccountRequest
        {
            MerchantAccount = "test_merchant",
            MerchantSignature = "",
            OrderReference = "ORDER123",
            Amount = 100.50m,
            Currency = "UAH",
            Iban = "UA26000000000000000000",
            Okpo = "12345678",
            AccountName = "Test FOP",
            Description = "Payment for services"
        };

        // Act
        var fields = request.GetSignatureFields().ToList();

        // Assert
        fields.Should().HaveCount(8);
        fields[0].Should().Be("test_merchant");
        fields[1].Should().Be("ORDER123");
        fields[2].Should().Be("100.5");
        fields[3].Should().Be("UAH");
        fields[4].Should().Be("UA26000000000000000000");
        fields[5].Should().Be("12345678");
        fields[6].Should().Be("Test FOP");
        fields[7].Should().Be("Payment for services");
    }
}
