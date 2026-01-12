using FluentAssertions;
using WayForPaySDK.Requests;

namespace WayForPaySDK.Tests.Unit;

public class CreateQrTests
{
    [Fact]
    public void CreateQrRequest_TransactionType_IsCreateQr()
    {
        var request = new CreateQrRequest
        {
            MerchantAccount = "test_merch",
            MerchantSignature = "sig",
            MerchantDomainName = "domain.com",
            OrderReference = "REF",
            OrderDate = 1234567890,
            Amount = 100,
            Currency = "UAH",
            ProductName = new[] { "Prod" },
            ProductPrice = new[] { 100m },
            ProductCount = new[] { 1 }
        };

        request.TransactionType.Should().Be("CREATE_QR");
    }

    [Fact]
    public void CreateQrRequest_GetSignatureFields_ReturnsCorrectOrder()
    {
        var request = new CreateQrRequest
        {
            MerchantAccount = "merch",
            MerchantSignature = "sig",
            MerchantDomainName = "domain",
            OrderReference = "REF",
            OrderDate = 1234567890,
            Amount = 100.50m,
            Currency = "UAH",
            ProductName = new[] { "P1", "P2" },
            ProductPrice = new[] { 50.25m, 50.25m },
            ProductCount = new[] { 1, 1 }
        };

        var fields = request.GetSignatureFields().ToList();

        fields.Should().ContainInOrder(
            "merch",
            "domain",
            "REF",
            "1234567890",
            "100.5",
            "UAH",
            "P1", "P2",
            "1", "1",
            "50.25", "50.25"
        );
    }
}
