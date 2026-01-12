using FluentAssertions;
using WayForPaySDK.Domain;
using WayForPaySDK.Requests;

namespace WayForPaySDK.Tests.Unit;

public class RequestTests
{
    [Fact]
    public void ChargeRequest_WithSplits_HasSplitsProperty()
    {
        var request = new ChargeRequest
        {
            MerchantAccount = "merch",
            MerchantSignature = "sig",
            MerchantDomainName = "dom",
            OrderReference = "ref",
            OrderDate = 123,
            Amount = 100,
            Currency = "UAH",
            ProductName = new[] { "p" },
            ProductPrice = new[] { 100m },
            ProductCount = new[] { 1 },
            Splits = new[]
            {
                new Split { Id = "1", Type = "flat", Value = "50" }
            }
        };

        request.Splits.Should().HaveCount(1);
        request.Splits!.First().Id.Should().Be("1");
    }
}
